using System;
using System.Collections.Generic;

using Orleans.Concurrency;

namespace GrainInterfaces.GameAction.Messages
{
	[Immutable]
	public class ShootMessage : GameActionMessage
	{
		public string PlayerId { get; set; }
		public string Direction { get; set; }
	}
}
