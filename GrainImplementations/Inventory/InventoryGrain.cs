using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;

using GrainInterfaces.Inventory;
using GrainInterfaces.Inventory.Messages;

namespace GrainImplementations.Inventory
{
    public class InventoryGrain : Grain, IInventory
    {
		private ItemList items;
		private readonly ILogger logger;

        public InventoryGrain(ILogger<InventoryGrain> logger)
		{
			this.logger = logger;
			items = new ItemList();
		}

        public Task AddItem(string item)
        {
            if (items.Contains(item))
			{
				return Task.CompletedTask;
			}

			items.Add(item);

			return Task.CompletedTask;
        }

        public Task<ItemList> GetItems()
        {
			return Task.FromResult(items);
        }
    }
}
