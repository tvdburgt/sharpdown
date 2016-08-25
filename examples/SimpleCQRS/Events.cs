using System;
namespace SimpleCQRS
{
    public class Event : Message
    {
        public int Version;
    }
    
    public class InventoryItemDeactivated : Event {
        public readonly Guid Id;

        public InventoryItemDeactivated(Guid id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Creates a new inventory item.
    /// </summary>
    public class InventoryItemCreated : Event {
        public readonly Guid Id;

        /// <summary>
        /// Informative description of the product you want to create.
        /// </summary>
        public readonly string Name;
        public InventoryItemCreated(Guid id, string name) {
            Id = id;
            Name = name;
        }
    }

    /// <summary>
    /// Renames an item in the inventory.
    /// </summary>
    public class InventoryItemRenamed : Event
    {
        public readonly Guid Id;
        public readonly string NewName;
 
        public InventoryItemRenamed(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }

    public class ItemsCheckedInToInventory : Event
    {
        public Guid Id;
        public readonly int Count;
 
        public ItemsCheckedInToInventory(Guid id, int count) {
            Id = id;
            Count = count;
        }
    }

    public class ItemsRemovedFromInventory : Event
    {
        public Guid Id;
        public readonly int Count;
 
        public ItemsRemovedFromInventory(Guid id, int count) {
            Id = id;
            Count = count;
        }
    }
}

