using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Streams;

using GrainInterfaces.GameAction;
using GrainInterfaces.GameAction.Messages;
using GrainInterfaces.GameState;

namespace GrainImplementations
{
	public class GameActionMessageHandlerGrain : Grain, IGameActionMessageHandler
	{
		private readonly ILogger logger;

		private IGameState state;

		private Dictionary<Type, Func<GameActionMessage, Task>> handlers;

		public GameActionMessageHandlerGrain(ILogger<GameGrain> logger)
		{
			this.logger = logger;
		}

		public override Task OnActivateAsync()
		{
			Guid gameId = this.GetPrimaryKey();

			// Get game state for this game
			state = GrainFactory.GetGrain<IGameState>(gameId);

			handlers = new Dictionary<Type, Func<GameActionMessage, Task>>();

			BuildHandlers(handlers);

			return base.OnActivateAsync();
		}

		public async Task ProcessMessage(GameActionMessage message)
		{
			Type type = message.GetType();
			if (!handlers.ContainsKey(type))
			{
				this.logger.LogInformation($"GameActionMessageHandler message type not recognised");
				return;
			}

			Func<GameActionMessage, Task> handler = handlers[type];
			await handler(message);

			this.logger.LogInformation($"GameActionMessage Processed: {message}");
		}

		private void BuildHandlers(Dictionary<Type, Func<GameActionMessage, Task>> handlers)
		{
			handlers.Add(typeof(MoveMessage), async (msg) => await state.PlayerMove(msg));
			handlers.Add(typeof(ShootMessage), async (msg) => await state.PlayerShoot(msg));
		}
	}
}
