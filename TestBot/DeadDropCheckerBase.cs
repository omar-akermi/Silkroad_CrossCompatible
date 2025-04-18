#if IL2CPP
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppSystem.Collections.Generic;
#else
using ScheduleOne.Economy;
using ScheduleOne.ItemFramework;
using ScheduleOne.Product;
using System.Collections.Generic;
using System.Linq;
#endif

using UnityEngine;
using MelonLoader;

namespace SOE
{
    public abstract class DeadDropCheckerBase
    {
        protected DeadDrop drop;
        protected string itemID;
        protected string packaging;
        protected int requiredAmount;

        public DeadDropCheckerBase(DeadDrop drop, string itemID, string packaging = "brick", int requiredAmount = 1)
        {
            this.drop = drop;
            this.itemID = itemID;
            this.packaging = packaging;
            this.requiredAmount = requiredAmount;
        }

        /// <summary>
        /// Dumps the contents of the target DeadDrop to the log.
        /// </summary>
        protected void PrintStorageContents()
        {
            if (drop?.Storage?.ItemSlots == null)
            {
                MelonLogger.Warning("📦 Storage or slots are null.");
                return;
            }

            MelonLogger.Msg($"📦 Dumping contents of DeadDrop name : '{drop.name}' id : '{drop}...");

#if IL2CPP
            List<ItemSlot> slots = drop.Storage.ItemSlots;
#else
            var slots = drop.Storage.ItemSlots;
#endif

            if (slots.Count == 0)
            {
                MelonLogger.Msg("📭 DeadDrop is empty.");
                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                var item = slot?.ItemInstance;
                if (item == null) continue;

                int quantity = slot.Quantity;
                string label = item.Name;

#if IL2CPP
                if (item.TryCast<ProductItemInstance>() is ProductItemInstance prod)
                {
                    string id = prod.Definition?.name ?? "(null)";
                    string pack = prod.PackagingID ?? "?";
                    label = $"{id} [{pack}]";
                }
                else if (item.TryCast<CashInstance>() is CashInstance cash)
                {
                    label = $"Cash: ${cash.Balance}";
                }
#else
                if (item is ProductItemInstance prod)
                {
                    string id = prod.Definition?.name ?? "(null)";
                    string pack = prod.PackagingID ?? "?";
                    label = $"{id} [{pack}]";
                }
                else if (item is CashInstance cash)
                {
                    label = $"Cash: ${cash.Balance} id : '{cash.ID}'";
                }
#endif
                MelonLogger.Msg($"   • {quantity}x {label}");
            }
        }

        /// <summary>
        /// Checks if the DeadDrop contains enough of the specified item & packaging.
        /// </summary>
        public bool CheckIfItemDelivered()
        {
            if (drop?.Storage?.ItemSlots == null)
            {
                MelonLogger.Warning("⚠️ DeadDrop or Storage is null.");
                return false;
            }

            int total = 0;

#if IL2CPP
            List<ItemSlot> slots = drop.Storage.ItemSlots;
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                var item = slot?.ItemInstance?.TryCast<ProductItemInstance>();
                if (item != null &&
                    (item.Name?.Replace(" ", "").ToLowerInvariant() ?? "") == itemID.Replace(" ", "").ToLowerInvariant())
                {
                    total += slot.Quantity;
                }
            }
#else
    var slots = drop.Storage.ItemSlots;
    total = slots
        .Where(slot => slot.ItemInstance is ProductItemInstance item &&
                       (item.PackagingID?.ToLowerInvariant() ?? "") == packaging.ToLowerInvariant() &&
                       (item.Name?.Replace(" ", "").ToLowerInvariant() ?? "") == itemID.Replace(" ", "").ToLowerInvariant())
        .Sum(slot => slot.Quantity);
#endif

            MelonLogger.Msg($"🔍 Found {total}/{requiredAmount} of '{itemID}' [{packaging}] in DeadDrop '{drop.name}'");

            return total >= requiredAmount;
        }
        protected void RemoveMatchingItemsFromStorage(int amount)
        {
            if (drop?.Storage?.ItemSlots == null)
            {
                MelonLogger.Warning("⚠️ Cannot remove items: DeadDrop or Storage is null.");
                return;
            }

            int toRemove = amount;

#if IL2CPP
    List<ItemSlot> slots = drop.Storage.ItemSlots;
    for (int i = 0; i < slots.Count; i++)
    {
        var slot = slots[i];
        var item = slot?.ItemInstance?.TryCast<ProductItemInstance>();
        if (item == null) continue;

        bool nameMatch = (item.Name?.Replace(" ", "").ToLowerInvariant() ?? "") == itemID.Replace(" ", "").ToLowerInvariant();
        bool packagingMatch = (item.PackagingID?.ToLowerInvariant() ?? "") == packaging.ToLowerInvariant();

        if (nameMatch && packagingMatch)
        {
            int removeQty = Mathf.Min(toRemove, slot.Quantity);
            slot.ChangeQuantity(-removeQty);
            toRemove -= removeQty;

            MelonLogger.Msg($"🗑️ Removed {removeQty}x {item.Name} from slot.");

            if (toRemove <= 0)
                break;
        }
    }

#else
            var matchingSlots = drop.Storage.ItemSlots
                .Where(slot => slot.ItemInstance is ProductItemInstance item &&
                               (item.PackagingID?.ToLowerInvariant() ?? "") == packaging.ToLowerInvariant() &&
                               (item.Name?.Replace(" ", "").ToLowerInvariant() ?? "") == itemID.Replace(" ", "").ToLowerInvariant())
                .ToList();

            foreach (var slot in matchingSlots)
            {
                var item = slot.ItemInstance as ProductItemInstance;
                if (item == null) continue;

                MelonLogger.Msg($"🧱 Slot with {slot.Quantity}x {item.Name} [{item.PackagingID}] found");

                int removeQty = Mathf.Min(toRemove, slot.Quantity);
                slot.ChangeQuantity(-removeQty);
                toRemove -= removeQty;

                MelonLogger.Msg($"🗑️ Removed {removeQty}x {item.Name} from slot.");

                if (toRemove <= 0)
                    break;
            }
#endif

            if (toRemove > 0)
            {
                MelonLogger.Warning($"⚠️ Could not remove full amount. {toRemove} items left unremoved.");
            }
            else
            {
                MelonLogger.Msg($"✅ Successfully removed {amount}x '{itemID}' [{packaging}] from DeadDrop.");
            }
        }


    }
}
