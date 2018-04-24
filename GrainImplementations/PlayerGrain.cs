using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Orleans;

using GrainInterfaces.Player;

namespace GrainImplementations
{
	public class PlayerGrain : Grain, IPlayer
	{
		private readonly ILogger logger;

		private PlayerInfo info;

		public PlayerGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public override Task OnActivateAsync()
		{
			info = new PlayerInfo { Key = this.GetPrimaryKeyString(), Name = string.Empty };
			return base.OnActivateAsync();
		}

		public Task SetName(string name)
		{
			info.Name = name;
			logger.LogInformation($"Player {info.Key} set name to {info.Name}");
			return Task.CompletedTask;
		}
	}
}
