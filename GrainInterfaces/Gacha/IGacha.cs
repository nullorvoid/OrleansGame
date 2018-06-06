using System.Threading.Tasks;

using Orleans;

using GrainInterfaces.Gacha.Messages;

namespace GrainInterfaces.Gacha
{
    public interface IGacha : IGrainWithIntegerKey
    {
         Task<GachaResult> Roll();
    }
}
