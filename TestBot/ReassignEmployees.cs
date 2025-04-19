using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.VoiceOver;
#else
using ScheduleOne.Dialogue;
using ScheduleOne.Employees;
using ScheduleOne.Property;
using ScheduleOne.VoiceOver;
#endif

using UnityEngine;
using MelonLoader;

namespace ScheduleOneEnhanced.Mods
{
    [HarmonyPatch]
    public class ReassignEmployees
    {
        private static MelonPreferences_Entry<bool> _enabled = null!;
        private static DialogueHandler? _currentHandler;
        private static Employee? _currentEmployee;

        public static void Initialize()
        {
            var cat = MelonPreferences.CreateCategory("SOE_ReassignEmployees");
            _enabled = cat.CreateEntry("Enabled", true, "Enable Reassignment");
            MelonLogger.Msg("[ReassignEmployees] Initialized.");
        }

        private static void ShowAssignDialogue()
        {
            MelonLogger.Msg("[ReassignEmployees] ShowAssignDialogue called.");

            if (_currentHandler == null || _currentEmployee == null)
            {
                MelonLogger.Warning("[ReassignEmployees] Handler or Employee is null.");
                return;
            }

            // Build a fresh DialogueContainer
            var container = ScriptableObject.CreateInstance<DialogueContainer>();
            container.DialogueNodeData = new Il2CppSystem.Collections.Generic.List<DialogueNodeData>();

            var node = new DialogueNodeData
            {
                DialogueNodeLabel = "ASSIGN_PROPERTY",
                DialogueText = "Where would you like me to go boss?",
                VoiceLine = EVOLineType.Think
            };

            // IL2CPP-safe: build property choices manually
            var choicesList = new List<DialogueChoiceData>();
            foreach (var property in Property.OwnedProperties)
            {
                if (property == null) continue;
                if (property == _currentEmployee.AssignedProperty) continue;
                if (property.PropertyCode == "rv" || property.PropertyCode == "motelroom") continue;

                choicesList.Add(new DialogueChoiceData
                {
                    ChoiceText = property.PropertyName,
                    ChoiceLabel = "ASSIGNED_TO_" + property.PropertyCode
                });
            }

            node.choices = choicesList.ToArray();
            container.DialogueNodeData.Add(node);

            // Call the correct InitializeDialogue method
            var initMethod = AccessTools.Method(
                typeof(DialogueHandler),
                "InitializeDialogue",
                new[] { typeof(DialogueContainer), typeof(bool), typeof(string) }
            );

            if (initMethod != null)
            {
                MelonLogger.Msg("[ReassignEmployees] Starting ASSIGN_PROPERTY dialogue.");
                initMethod.Invoke(_currentHandler, new object[] { container, true, "ASSIGN_PROPERTY" });
            }
            else
            {
                MelonLogger.Error("[ReassignEmployees] Failed to find InitializeDialogue method.");
            }
        }

        private static UnityEngine.Events.UnityAction? _assignDialogueAction = null;

        [HarmonyPatch(typeof(Employee), "Start")]
        [HarmonyPostfix]
        public static void EmployeeStart(Employee __instance)
        {
            if (!_enabled.Value) return;

            var controller = __instance.dialogueHandler.GetComponent<DialogueController>();
            var choice = new DialogueController.DialogueChoice
            {
                ChoiceText = "I need you to work for me somewhere else.",
                Enabled = true
            };

            void AssignDialogueWrapper()
            {
                _currentHandler = __instance.dialogueHandler;
                _currentEmployee = __instance;
                ShowAssignDialogue();
            }

            if (_assignDialogueAction == null)
                _assignDialogueAction = (UnityEngine.Events.UnityAction)AssignDialogueWrapper;

            choice.onChoosen.AddListener(_assignDialogueAction);
            controller.AddDialogueChoice(choice);

            MelonLogger.Msg($"[ReassignEmployees] Reassign choice added to {__instance.name}");
        }

        [HarmonyPatch(typeof(DialogueController), "ChoiceCallback")]
        [HarmonyPostfix]
        public static void ChoiceCallback(DialogueController __instance, string choiceLabel)
        {
            if (!choiceLabel.StartsWith("ASSIGNED_TO_")) return;

            var employee = __instance.GetComponentInParent<Employee>();
            if (employee == null)
            {
                MelonLogger.Warning("[ReassignEmployees] Could not find Employee in ChoiceCallback.");
                return;
            }

            var targetCode = choiceLabel.Substring("ASSIGNED_TO_".Length);
            var property = PropertyManager.Instance.GetProperty(targetCode);
            if (property == null)
            {
                MelonLogger.Warning($"[ReassignEmployees] Invalid property: {targetCode}");
                return;
            }

            employee.AssignedProperty.DeregisterEmployee(employee);
            var assignMethod = AccessTools.Method(typeof(Employee), "AssignProperty");
            assignMethod?.Invoke(employee, new object[] { property });

            MelonLogger.Msg($"[ReassignEmployees] {employee.name} reassigned to {property.PropertyName}");
        }
    }
}
