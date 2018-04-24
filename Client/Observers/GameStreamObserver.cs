using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Orleans.Streams;

using GrainInterfaces.Game.Messages;

namespace Client.Observers
{
    public class GameStreamObserver : IAsyncObserver<GameMessage>
    {
        private ILogger logger;

        private Dictionary<Type, Action<GameMessage>> handlers;

        public GameStreamObserver(ILogger logger)
        {
            this.logger = logger;

            handlers = new Dictionary<Type, Action<GameMessage>>();

            handlers.Add(typeof(PlayerJoinedMessage), (msg) => HandleJoinMessage(msg));
            handlers.Add(typeof(PlayerLeftMessage), (msg) => HandleLeftMessage(msg));
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
                this.logger.LogInformation($"GameStreamObserver message type not recognised");
            }

            Action<GameMessage> handler = handlers[type];

            handler(item);

            return Task.CompletedTask;
        }

        private void HandleJoinMessage(GameMessage msg)
        {
            PlayerJoinedMessage joinMsg = msg as PlayerJoinedMessage;
            this.logger.LogInformation($"{joinMsg.PlayerId} joined the server");
        }

        private void HandleLeftMessage(GameMessage msg)
        {
            PlayerLeftMessage leftMsg = msg as PlayerLeftMessage;
            this.logger.LogInformation($"{leftMsg.PlayerId} left the server");
        }
    }
}
