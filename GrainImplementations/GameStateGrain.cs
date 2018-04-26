using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Streams;

using GrainInterfaces.GameState;
using GrainInterfaces.GameAction.Messages;

namespace GrainImplementations
{
	public class GameStateGrain : Grain, IGameState
	{
		private readonly ILogger logger;

		public GameStateGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public Task PlayerMove(GameActionMessage message)
		{
			MoveMessage moveMsg = message as MoveMessage;

			if (moveMsg == null)
			{
				throw new Exception($"Player move recieved a message that was not of type {typeof(MoveMessage)} but was {message.GetType()}");
			}

			logger.LogInformation($"Player {moveMsg.PlayerId} moved in direction {moveMsg.Direction}");

			return Task.CompletedTask;
		}

		public Task PlayerShoot(GameActionMessage message)
		{
			ShootMessage shootMsg = message as ShootMessage;

			if (shootMsg == null)
			{
				throw new Exception($"Player shoot recieved a message that was not of type {typeof(ShootMessage)} but was {message.GetType()}");
			}

			logger.LogInformation($"Player {shootMsg.PlayerId} shot in direction {shootMsg.Direction}");

			return Task.CompletedTask;
		}
	}
}
