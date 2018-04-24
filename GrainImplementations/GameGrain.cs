using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Streams;

using GrainInterfaces.Game;
using GrainInterfaces.Game.Messages;
using GrainInterfaces.Player;

namespace GrainImplementations
{
	public class GameGrain : Grain, IGame
	{
		private readonly ILogger logger;

		private IAsyncStream<GameMessage> stream;

		private GameInfo info;

		public GameGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public override Task OnActivateAsync()
		{
			Guid gameId = this.GetPrimaryKey();
			info = new GameInfo { Key = gameId, Players = new List<IPlayer>() };

			IStreamProvider streamProvider = GetStreamProvider("GameStream");
			stream = streamProvider.GetStream<GameMessage>(gameId, null);

			return base.OnActivateAsync();
		}

		public async Task Join(IPlayer player)
		{
			logger.LogInformation($"Player {player.GetPrimaryKeyString()} joined the game");

			if (info.Players.Contains(player))
			{
				logger.LogInformation($"Player {player.GetPrimaryKeyString()} was already in the game!");
				throw new Exception("Player was already in this game!");
			}

			info.Players.Add(player);

			logger.LogInformation($"Game {info.Key} now has {info.Players.Count} players");

			// TODO: This can fail, update with correct error handling
			await stream.OnNextAsync(new PlayerJoinedMessage() { PlayerId = player.GetPrimaryKeyString() });
		}

		public async Task Leave(IPlayer player)
		{
			logger.LogInformation($"Player {player.GetPrimaryKeyString()} left the game");

			info.Players.Remove(player);

			logger.LogInformation($"Game {info.Key} now has {info.Players.Count} players");

			// TODO: This can fail, update with correct error handling
			await stream.OnNextAsync(new PlayerLeftMessage() { PlayerId = player.GetPrimaryKeyString() });
		}
	}
}
