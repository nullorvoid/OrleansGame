using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Streams;

using GrainInterfaces.Game;
using GrainInterfaces.Game.Messages;
using GrainInterfaces.Game.Messages.Setup;
using GrainInterfaces.Player;

namespace GrainImplementations
{
	public class GameGrain : Grain, IGame
	{
		private readonly ILogger logger;

		private IAsyncStream<GameMessage> stream;

		private GameInfo info;

		private IGameActionMessageHandler actionMessageHandler;

		public GameGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public override Task OnActivateAsync()
		{
			// Create game info with players to be stored here
			Guid gameId = this.GetPrimaryKey();
			info = new GameInfo { Key = gameId, Players = new List<IPlayer>() };

			// Get game stream with no namespace for general messages
			IStreamProvider streamProvider = GetStreamProvider("GameStream");
			stream = streamProvider.GetStream<GameMessage>(gameId, "game");

			// Get game message handler which will interface with the state
			actionMessageHandler = GrainFactory.GetGrain<IGameActionMessageHandler>(gameId);

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

		public async Task ProcessActionMessage(IPlayer player, GameMessage message)
		{
			await actionMessageHandler.ProcessMessage(message);
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
