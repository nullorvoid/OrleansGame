using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;

using GrainInterfaces.Gacha;
using GrainInterfaces.Gacha.Messages;

using GrainInterfaces.Inventory;

namespace GrainImplementations.Inventory
{
    public class GachaGrain : Grain, IGacha
    {
		private IList<string> items;

		private readonly ILogger logger;

        public GachaGrain(ILogger<GachaGrain> logger)
		{
			this.logger = logger;
			items = new List<String>
			{
				"Tuna",
				"Salmon",
				"Shark",
				"Whale"
			};
		}

        public async Task<GachaResult> Roll()
        {
            var rng = new Random();
			int num = rng.Next(0, items.Count);
			string item = items[num];

			IInventory inventory = GrainFactory.GetGrain<IInventory>(1);
			await inventory.AddItem(item);

			return new GachaResult(item);
        }
    }
}
