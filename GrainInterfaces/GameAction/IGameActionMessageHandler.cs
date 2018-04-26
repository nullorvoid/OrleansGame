using System.Threading.Tasks;

using Orleans;

using GrainInterfaces.GameAction.Messages;

namespace GrainInterfaces.GameAction
{
	public interface IGameActionMessageHandler : IGrainWithGuidKey
	{
		Task ProcessMessage(GameActionMessage message);
	}
}
