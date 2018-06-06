using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Orleans;

namespace WebServer.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IClusterClient client;

		public LoginController(IClusterClient client)
        {
            this.client = client;
        }

		[HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
			return await Task.FromResult("Tuna1");
        }
    }
}
