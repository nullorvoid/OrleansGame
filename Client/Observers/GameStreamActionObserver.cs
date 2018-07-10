using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Orleans.Streams;

using GrainInterfaces.Game.Messages;
using GrainInterfaces.Game.Messages.Actions;

namespace Client.Observers
{
	public class GameStreamActionObserver : IAsyncObserver<GameMessage>
	{
		private ILogger logger;

		private Dictionary<Type, Action<GameMessage>> handlers;

		public GameStreamActionObserver(ILogger logger)
		{
			this.logger = logger;

			handlers = new Dictionary<Type, Action<GameMessage>>();

			handlers.Add(typeof(MoveMessage), (msg) => HandleMoveMessage(msg));
			handlers.Add(typeof(ShootMessage), (msg) => HandleShootMessage(msg));
		}

		public Task OnCompletedAsync()
		{
			this.logger.LogInformation("Game message stream received stream completed event");
			return Task.CompletedTask;
		}

		public Task OnErrorAsync(Exception ex)
		{
			this.logger.LogInformation($"Game message stream is experiencing message delivery failure, ex :{ex}");
			return Task.CompletedTask;
		}

		public Task OnNextAsync(GameMessage item, StreamSequenceToken token = null)
		{
			Type type = item.GetType();
			if (!handlers.ContainsKey(type))
			{
				this.logger.LogInformation($"GameStreamActionObserver message type not recognised");
				// TODO: Update this with suitable error handles
				return Task.CompletedTask;
			}

			Action<GameMessage> handler = handlers[type];

			handler(item);

			return Task.CompletedTask;
		}

		private void HandleMoveMessage(GameMessage msg)
		{
			MoveMessage moveMsg = msg as MoveMessage;
			this.logger.LogInformation($"{moveMsg.PlayerId} moved in direction {moveMsg.Direction}");
		}

		private void HandleShootMessage(GameMessage msg)
		{
			ShootMessage shootMsg = msg as ShootMessage;
			this.logger.LogInformation($"{shootMsg.PlayerId} shot in direction {shootMsg.Direction}");
		}
	}
}
