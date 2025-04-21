using System;
using System.Linq;
using UnityEngine;
using MelonLoader;
using S1API.Items;
using S1API.Money;
using S1API.Storages;
using S1API.DeadDrops;
using S1API.Quests;
using S1API.Products;
using S1API.Saveables;
using S1API.NPCs;
using System.Collections.Generic;
#if IL2CPP
using Il2CppScheduleOne.Economy;
#else
using ScheduleOne.Economy;
#endif
using Random = UnityEngine.Random;

namespace SilkRoad.Quests
{
    public class QuestDelivery : Quest
    {
        [SaveableField("DeliveryData")]
        public DeliverySaveData Data = new DeliverySaveData();

        private DeadDropInstance deliveryDrop;
        private DeadDropInstance rewardDrop;
        public static HashSet<string> CompletedQuestKeys = new HashSet<string>();

        private QuestEntry deliveryEntry;
        private QuestEntry rewardEntry;
        public static bool QuestActive = false;
        public static event Action OnQuestCompleted;

        internal override void CreateInternal()
        {
            base.CreateInternal();
            QuestActive = true;

            MelonLogger.Msg($"🔍 QuestDelivery CreateInternal: ProductID={Data?.ProductID}, Initialized={Data?.Initialized}");

            if (Data == null)
            {
                MelonLogger.Error("❌ QuestDelivery.Data is null!");
                Data = new DeliverySaveData();
            }

            if (!Data.Initialized)
            {
                var drops = DeadDropManager.All?.ToList();
                if (drops == null || drops.Count < 6)
                {
                    MelonLogger.Error("❌ Not enough dead drops to assign delivery/reward.");
                    return;
                }

                deliveryDrop = drops[Random.Range(0, DeadDrop.DeadDrops.Count)];
                rewardDrop = drops[Random.Range(0, DeadDrop.DeadDrops.Count)];

                Data.DeliveryDropGUID = deliveryDrop.GUID;
                Data.RewardDropGUID = rewardDrop.GUID;
                Data.Initialized = true;

                foreach (var drop in DeadDropManager.All)
                    MelonLogger.Msg($"🗺️ Available drop: {drop.name} ({drop.GUID})");
            }
            else
            {
                deliveryDrop = DeadDropManager.All.FirstOrDefault(d => d.GUID == Data.DeliveryDropGUID);
                rewardDrop = DeadDropManager.All.FirstOrDefault(d => d.GUID == Data.RewardDropGUID);

                if (deliveryDrop == null || rewardDrop == null)
                {
                    MelonLogger.Warning("⚠️ Failed to resolve saved DeadDrops. Reassigning...");
                    var drops = DeadDropManager.All.ToList();

                    if (drops.Count >= 2)
                    {
                        deliveryDrop = drops[0];
                        rewardDrop = drops[1];
                        Data.DeliveryDropGUID = deliveryDrop.GUID;
                        Data.RewardDropGUID = rewardDrop.GUID;
                    }
                    else
                    {
                        MelonLogger.Error("❌ Not enough DeadDrops to reassign.");
                        return;
                    }
                    S1Quest.onQuestBegin?.Invoke();

                }
            }

            deliveryEntry = AddEntry($"Deliver {Data.RequiredAmount}x bricks of {Data.ProductID} to {deliveryDrop.name}");
            deliveryEntry.POIPosition = deliveryDrop.Position;
            deliveryEntry.Begin();

            rewardEntry = AddEntry($"Collect your reward from {rewardDrop.name}");
            rewardEntry.POIPosition = rewardDrop.Position;
            rewardEntry.SetState(QuestState.Inactive);

            deliveryDrop.Storage.OnClosed += CheckDelivery;

            Contacts.Buyer?.SendDeliveryAccepted(Data.ProductID, (int)Data.RequiredAmount);

            MelonLogger.Msg("📦 QuestDelivery started with drop locations assigned.");
        }

        private void CheckDelivery()
        {
            var total = deliveryDrop.Storage.Slots
                .Where(slot => slot.ItemInstance is ProductInstance product &&
                               product.AppliedPackaging.S1PackagingDefinition.ID == "brick" &&
                               product.Definition.Name == Data.ProductID)
                .Sum(slot => slot.Quantity);

            if (total < Data.RequiredAmount)
            {
                MelonLogger.Msg($"❌ Not enough bricks: {total}/{Data.RequiredAmount}");
                return;
            }

            uint toRemove = Data.RequiredAmount;
            foreach (var slot in deliveryDrop.Storage.Slots)
            {
                if (slot.ItemInstance is ProductInstance product &&
                    product.AppliedPackaging.S1PackagingDefinition.ID == "brick" &&
                    product.Definition.Name == Data.ProductID)
                {
                    int remove = (int)Mathf.Min(slot.Quantity, toRemove);
                    slot.AddQuantity(-remove);
                    toRemove -= (uint)remove;
                    if (toRemove == 0) break;
                }
            }

            deliveryEntry.Complete();
            rewardEntry.SetState(QuestState.Active);
            rewardDrop.Storage.OnOpened += GiveReward;

            Contacts.Buyer?.SendDeliverySuccess(Data.ProductID);

            MelonLogger.Msg("✅ Delivery complete. Reward entry now active.");
        }

        private void GiveReward()
        {
            if (deliveryEntry == null || rewardEntry == null)
                return;

            // 💡 Avoid duplicate reward delivery
            if (rewardEntry.State == QuestState.Completed)
            {
                MelonLogger.Warning("⛔ Reward already collected. Ignoring repeated stash opening.");
                rewardDrop.Storage.OnOpened -= GiveReward;
                return;
            }

            if (deliveryEntry.State != QuestState.Completed)
            {
                MelonLogger.Warning("⛔ Cannot give reward. Delivery not complete.");
                return;
            }

            var cashDef = ItemManager.GetItemDefinition("cash") as CashDefinition;
            var cash = cashDef?.CreateInstance() as CashInstance;
            if (cash == null)
            {
                MelonLogger.Error("❌ Failed to create cash instance.");
                return;
            }

            cash.SetQuantity(Data.Reward);

            if (!rewardDrop.Storage.CanItemFit(cash))
            {
                MelonLogger.Warning("📦 Not enough room in reward stash for cash.");
                return;
            }
            QuestActive = false;
            string key = $"{Data.ProductID}_{Data.RequiredAmount}";
            CompletedQuestKeys.Add(key);
            rewardDrop.Storage.AddItem(cash);
            rewardEntry.Complete();
            deliveryEntry.Complete();
            Complete();
            QuestManager.Quests.Remove(this);
            OnQuestCompleted?.Invoke();

            Contacts.Buyer?.SendRewardDropped();

            MelonLogger.Msg($"💰 Reward of ${Data.Reward} inserted into DeadDrop.");

            rewardDrop.Storage.OnOpened -= GiveReward;
        }

        protected override string Title =>
            Data?.ProductID != null ? $"Deliver {Data.ProductID}" : "Silkroad Delivery";

        protected override string Description =>
            Data?.ProductID != null && Data.RequiredAmount > 0
                ? $"Deliver {Data.RequiredAmount}x bricks of {Data.ProductID} to the drop point."
                : "Deliver the assigned product to the stash location.";


    }
}