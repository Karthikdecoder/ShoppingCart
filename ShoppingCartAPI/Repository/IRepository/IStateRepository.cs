using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repository.IRepository
{
	public interface IStateRepository : IRepository<StateMaster>
	{
		Task<StateMaster> UpdateAsync(StateMaster State);
	}
}
