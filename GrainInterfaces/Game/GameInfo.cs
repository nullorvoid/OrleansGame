using System;
using System.Collections.Generic;

using Orleans.Concurrency;

using GrainInterfaces.Player;

namespace GrainInterfaces.Game
{
	[Immutable]
	public class GameInfo
	{
		public Guid Key { get; set; }
		public List<IPlayer> Players { get; set; }
	}
}
