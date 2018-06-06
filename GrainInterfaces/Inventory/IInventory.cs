using System.Threading.Tasks;

using Orleans;

using GrainInterfaces.Inventory.Messages;

namespace GrainInterfaces.Inventory
{
    public interface IInventory : IGrainWithIntegerKey
    {
        Task<ItemList> GetItems();
		Task AddItem(string item);
    }
}
