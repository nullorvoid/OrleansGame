using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;

using GrainInterfaces.Game;
using GrainInterfaces.Player;

namespace GrainImplementations
{
	public class GameGrain : Grain, IGame
	{
		private readonly ILogger logger;

		private GameInfo info;

		public GameGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public override Task OnActivateAsync()
		{
			info = new GameInfo { Key = this.GetPrimaryKey(), Players = new List<IPlayer>() };
			return base.OnActivateAsync();
		}

		public Task Join(IPlayer player)
		{
			logger.LogInformation($"Player {player.GetPrimaryKeyString()} joined the game");

			if (info.Players.Contains(player))
			{
				logger.LogInformation($"Player {player.GetPrimaryKeyString()} was already in the game!");
				throw new Exception("Player was already in this game!");
			}

			info.Players.Add(player);

			logger.LogInformation($"Game {info.Key} now has {info.Players.Count} players");

			return Task.CompletedTask;
		}

		public Task Leave(IPlayer player)
		{
			logger.LogInformation($"Player {player.GetPrimaryKeyString()} left the game");

			info.Players.Remove(player);

			logger.LogInformation($"Game {info.Key} now has {info.Players.Count} players");

			return Task.CompletedTask;
		}
	}
}
