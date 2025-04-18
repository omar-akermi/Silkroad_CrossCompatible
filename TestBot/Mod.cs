#if IL2CPP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Product;


#elif (MONO)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ScheduleOne.Economy;
using ScheduleOne.Quests;
#endif
using MelonLoader;
using System.Collections.Generic;
using SOE;
using System.Threading;
using System.Runtime.CompilerServices;


[assembly: MelonInfo(typeof(MyMod), "Silk Road App", "1.0.0", "Akermi")]
[assembly: MelonGame("TVGS", "Schedule I")]


namespace SOE
{
    public class MyMod : MelonMod
    {
        private OGKushChecker deliveryChecker;

        private bool _isInGame;
        private MyApp _app;
        private DeadDropRewardHandler rewardHandler;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            bool nowInGame = sceneName != null && sceneName.Contains("Main");

            if (!_isInGame && nowInGame)
            {
                LoggerInstance.Msg("Entering game scene: " + sceneName);
                _app = new MyApp();
                _app.Init(LoggerInstance);

                DeadDrop drop = DeadDrop.DeadDrops[5];
                int rewardAmount = 2000;

                // Assume you already have a quest step
                rewardHandler = new DeadDropRewardHandler(drop, rewardAmount);

                deliveryChecker = new OGKushChecker(drop, 20);

            }
            else if (_isInGame && !nowInGame)
            {
                LoggerInstance.Msg("Exiting game scene.");
                _app = null;
            }

            _isInGame = nowInGame;
        }
        public override void OnUpdate()
        {

            deliveryChecker?.TryVerifyOnStorageClose();

            rewardHandler?.Update();

        }
    }
    public class OGKushChecker : DeadDropCheckerBase
    {
        private bool deliverySuccessLogged = false;
        private bool deliveryFailedOnce = false;

        public OGKushChecker(DeadDrop drop, int quantityRequired)
            : base(drop, "OG Kush", "brick", quantityRequired)
        {
        }

        private bool wasOpenLastFrame = false;

        public void TryVerifyOnStorageClose()
        {
            if (drop?.Storage == null) return;

            bool currentlyOpen = drop.Storage.IsOpened;

            if (!currentlyOpen && wasOpenLastFrame && !deliverySuccessLogged)
            {
                // 🔐 Storage just closed – check delivery
                PrintStorageContents();

                if (CheckIfItemDelivered())
                {
                    RemoveMatchingItemsFromStorage(requiredAmount);
                    // Then mark delivery complete...

                MelonLogger.Msg("✅ Item delivery verified!");
                    deliverySuccessLogged = true;

                    // (Optional) trigger quest complete or reward logic
                }
                else
                {
                    MelonLogger.Msg("❌ Delivery incomplete.");
                }
            }

            wasOpenLastFrame = currentlyOpen;
        }

    }
}