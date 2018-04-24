 using System;

 using Orleans.Concurrency;

 namespace GrainInterfaces.Player
 {
 	[Immutable]
	public class PlayerInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
	}
 }
