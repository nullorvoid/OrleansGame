using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Orleans;

using GrainInterfaces;

namespace GrainImplementations
{
	public class GameGrain : Grain, IGame
	{
		private readonly ILogger logger;

		public GameGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public Task Join(string playerId)
		{
			logger.LogInformation($"Player {playerId} joined the game");
			return Task.CompletedTask;
		}

		public Task Leave(string playerId)
		{
			logger.LogInformation($"Player {playerId} left the game");
			return Task.CompletedTask;
		}
	}
}
