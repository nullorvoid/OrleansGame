using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Orleans;

using GrainInterfaces.Gacha;
using GrainInterfaces.Gacha.Messages;

namespace WebServer.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class GachaController
    {
        private IClusterClient client;

		public GachaController(IClusterClient client)
        {
            this.client = client;
        }

		[HttpGet()]
		public async Task<GachaResult> Get()
		{
			IGacha gacha = client.GetGrain<IGacha>(1);
			return await gacha.Roll();
		}
    }
}
