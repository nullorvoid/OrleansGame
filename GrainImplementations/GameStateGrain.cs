using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Streams;

using GrainInterfaces.Game;
using GrainInterfaces.Game.Messages;
using GrainInterfaces.Game.Messages.Actions;

namespace GrainImplementations
{
	public class GameStateGrain : Grain, IGameState
	{
		private readonly ILogger logger;

		private IAsyncStream<GameMessage> stream;

		public GameStateGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public override Task OnActivateAsync()
		{
			Guid gameId = this.GetPrimaryKey();

			// Get game stream with no namespace for general messages
			IStreamProvider streamProvider = GetStreamProvider("GameStream");
			stream = streamProvider.GetStream<GameMessage>(gameId, "actions");

			return base.OnActivateAsync();
		}

		public async Task PlayerMove(GameMessage message)
		{
			MoveMessage moveMsg = message as MoveMessage;

			if (moveMsg == null)
			{
				throw new Exception($"Player move recieved a message that was not of type {typeof(MoveMessage)} but was {message.GetType()}");
			}

			logger.LogInformation($"Player {moveMsg.PlayerId} moved in direction {moveMsg.Direction}");

			await stream.OnNextAsync(message);
		}

		public async Task PlayerShoot(GameMessage message)
		{
			ShootMessage shootMsg = message as ShootMessage;

			if (shootMsg == null)
			{
				throw new Exception($"Player shoot recieved a message that was not of type {typeof(ShootMessage)} but was {message.GetType()}");
			}

			logger.LogInformation($"Player {shootMsg.PlayerId} shot in direction {shootMsg.Direction}");

			await stream.OnNextAsync(message);
		}
	}
}
