﻿#if (IL2CPP)
using S1Storage = Il2CppScheduleOne.Storage;
#elif (MONO)
using S1Storage = ScheduleOne.Storage;
#endif

using System;
using System.Linq;
using S1API.Items;
using UnityEngine.Events;

namespace S1API.Storages
{
    /// <summary>
    /// Represents a storage container in-game.
    /// </summary>
    public class StorageInstance
    {
        /// <summary>
        /// INTERNAL: The in-game storage instance.
        /// </summary>
        internal readonly S1Storage.StorageEntity S1Storage;
        
        /// <summary>
        /// Creates a storage instance from the in-game storage instance.
        /// </summary>
        /// <param name="storage"></param>
        internal StorageInstance(S1Storage.StorageEntity storage) => S1Storage = storage;
        
        /// <summary>
        /// An array of all slots available on the storage container.
        /// </summary>
        public ItemSlotInstance[] Slots => S1Storage.ItemSlots.ToArray()
            .Select(itemSlot => new ItemSlotInstance(itemSlot)).ToArray();

        /// <summary>
        /// Whether an item can fit inside this storage container or not.
        /// </summary>
        /// <param name="itemInstance">The item instance you want to store.</param>
        /// <param name="quantity">The quantity of item you want to store.</param>
        /// <returns>Whether the item will fit or not.</returns>
        public bool CanItemFit(ItemInstance itemInstance, int quantity = 1) => 
            S1Storage.CanItemFit(itemInstance.S1ItemInstance, quantity);
        
        /// <summary>
        /// Adds an item instance to this storage container.
        /// </summary>
        /// <param name="itemInstance">The item instance you want to store.</param>
        public void AddItem(ItemInstance itemInstance) => 
            S1Storage.InsertItem(itemInstance.S1ItemInstance);
        
        /// <summary>
        /// An action fired when the storage container is opened by the player.
        /// </summary>
        public event Action OnOpened
        {
            add => S1Storage.onOpened.AddListener((UnityAction)value.Invoke);
            remove => S1Storage.onOpened.RemoveListener((UnityAction)value.Invoke);
        }
        
        /// <summary>
        /// An action fired when the storage container is closed by the player.
        /// </summary>
        public event Action OnClosed
        {
            add => S1Storage.onClosed.AddListener((UnityAction)value.Invoke);
            remove => S1Storage.onClosed.RemoveListener((UnityAction)value.Invoke);
        }
    }
}