using Orleans;
using System.Threading.Tasks;

namespace GrainInterfaces.Player
{
	// Player functionality in the system, what the player is doing, user profile, etc.
	public interface IPlayer : IGrainWithStringKey
	{
		// Set the name of the player, not unique.
		Task SetName(string name);
	}
}
