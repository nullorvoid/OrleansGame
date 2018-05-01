using System;
using System.Collections.Generic;

using Orleans.Concurrency;

namespace GrainInterfaces.Game.Messages.Actions
{
	[Immutable]
	public class MoveMessage : GameMessage
	{
		public string PlayerId { get; set; }
		public string Direction { get; set; }
	}
}
