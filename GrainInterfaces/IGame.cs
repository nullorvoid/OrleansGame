using Orleans;
using System.Threading.Tasks;

namespace GrainInterfaces
{
	// The game is constructed via this interface, it allows players to join and leave the game
	// and know when the game has been started
	public interface IGame : IGrainWithGuidKey
	{
		// Join the game as a player
		Task Join(string playerId);

		// Leave the game as a player
		Task Leave(string playerId);
	}
}
