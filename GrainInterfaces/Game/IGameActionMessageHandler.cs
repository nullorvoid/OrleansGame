using System.Threading.Tasks;

using Orleans;

using GrainInterfaces.Game.Messages;

namespace GrainInterfaces.Game
{
	public interface IGameActionMessageHandler : IGrainWithGuidKey
	{
		Task ProcessMessage(GameMessage message);
	}
}
