using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Orleans;

using GrainInterfaces.Inventory;
using GrainInterfaces.Inventory.Messages;

namespace WebServer.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class InventoryController
    {
        private IClusterClient client;

		public InventoryController(IClusterClient client)
        {
            this.client = client;
        }

		[HttpGet()]
		public async Task<ItemList> Get()
		{
			IInventory inventory = client.GetGrain<IInventory>(1);
			return await inventory.GetItems();
		}

		[HttpGet("put/{item}")]
		public async Task AddItem(string item)
		{
			IInventory inventory = client.GetGrain<IInventory>(1);
			await inventory.AddItem(item);
		}
    }
}
