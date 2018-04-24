using System.Threading.Tasks;

using Orleans;

using GrainInterfaces.Player;

namespace GrainInterfaces.Game
{
	// The game is constructed via this interface, it allows players to join and leave the game
	// and know when the game has been started
	public interface IGame : IGrainWithGuidKey
	{
		// Join the game as a player
		Task Join(IPlayer player);

		// Leave the game as a player
		Task Leave(IPlayer player);
	}
}
