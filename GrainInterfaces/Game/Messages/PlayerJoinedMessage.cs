using System;
using System.Collections.Generic;

using Orleans.Concurrency;

namespace GrainInterfaces.Game.Messages
{
	[Immutable]
	public class PlayerJoinedMessage : GameMessage
	{
		public string PlayerId { get; set; }
	}
}
