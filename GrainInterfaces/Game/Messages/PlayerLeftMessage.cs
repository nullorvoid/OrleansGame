using System;
using System.Collections.Generic;

using Orleans.Concurrency;

namespace GrainInterfaces.Game.Messages
{
	[Immutable]
	public class PlayerLeftMessage : GameMessage
	{
		public string PlayerId { get; set; }
	}
}
