using System;
using System.Collections.Generic;

using Orleans.Concurrency;

namespace GrainInterfaces.Game.Messages.Setup
{
	[Immutable]
	public class PlayerJoinedMessage : GameMessage
	{
		public string PlayerId { get; set; }
	}
}
