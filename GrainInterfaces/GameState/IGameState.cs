using System.Threading.Tasks;

using Orleans;

using GrainInterfaces.GameAction.Messages;

namespace GrainInterfaces.GameState
{
	public interface IGameState : IGrainWithGuidKey
	{
		Task PlayerMove(GameActionMessage message);
		Task PlayerShoot(GameActionMessage message);
	}
}
