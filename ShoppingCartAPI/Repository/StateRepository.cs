using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
	public class StateRepository : Repository<StateMaster>, IStateRepository
	{
		private readonly ApplicationDbContext _db;

		public StateRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public async Task<StateMaster> UpdateAsync(StateMaster Country)
		{
			_db.StateMaster.Update(Country);
			await _db.SaveChangesAsync();

			return Country;
		}
	}
}
