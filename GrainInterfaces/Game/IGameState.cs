using System.Threading.Tasks;

using Orleans;

using GrainInterfaces.Game.Messages;

namespace GrainInterfaces.Game
{
	public interface IGameState : IGrainWithGuidKey
	{
		Task PlayerMove(GameMessage message);
		Task PlayerShoot(GameMessage message);
	}
}
