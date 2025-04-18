#if IL2CPP
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Storage;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;



#elif MONO
using System.Collections.Generic;
using ScheduleOne.PlayerScripts;
using ScheduleOne.Storage;
using ScheduleOne.Quests;
using ScheduleOne.Economy;
using ScheduleOne.DevUtilities;
using ScheduleOne.ItemFramework;

#endif

using UnityEngine;
using MelonLoader;

namespace SOE
{
    public class DeadDropRewardHandler
    {
        private DeadDrop rewardDrop;
        private int rewardAmount;
        private bool rewardGiven;
        private QuestEntry questStep;

        public DeadDropRewardHandler(DeadDrop drop, int amount)
        {
            rewardDrop = drop;
            rewardAmount = amount;
            rewardGiven = false;
        }

        public void Update()
        {
            if (rewardGiven || rewardDrop == null || rewardDrop.Storage == null)
                return;

            if (rewardDrop.Storage.IsOpened)
            {
                InsertReward();
            }
        }
        private void InsertReward()
        {
            if (PlayerSingleton<PlayerInventory>.Instance == null)
            {
                MelonLogger.Error("🚫 PlayerInventory singleton is null.");
                return;
            }

            var playerCash = PlayerSingleton<PlayerInventory>.Instance.cashInstance;
            if (playerCash == null)
            {
                MelonLogger.Error("🚫 PlayerInventory.cashInstance is null.");
                return;
            }

#if IL2CPP
            CashInstance rewardCash = playerCash.GetCopy().Cast<CashInstance>();
#else
    CashInstance rewardCash = (CashInstance)playerCash.GetCopy();
#endif

            if (rewardCash == null)
            {
                MelonLogger.Error("🚫 rewardCash copy is null.");
                return;
            }

            if (rewardDrop == null)
            {
                MelonLogger.Error("🚫 rewardDrop is null.");
                return;
            }

            if (rewardDrop.Storage == null)
            {
                MelonLogger.Error("🚫 rewardDrop.Storage is null.");
                return;
            }

            rewardCash.SetBalance(rewardAmount);

            if (!rewardDrop.Storage.CanItemFit(rewardCash))
            {
                MelonLogger.Warning("⚠️ Not enough space in DeadDrop to insert reward.");
                return;
            }

            rewardDrop.Storage.InsertItem(rewardCash);

            if (questStep != null)
                questStep.Complete();

            rewardGiven = true;

            MelonLogger.Msg($"💰 Reward of ${rewardAmount} inserted into DeadDrop.");
            MelonLogger.Msg("🏁 Quest complete!");
        }

    }
}
