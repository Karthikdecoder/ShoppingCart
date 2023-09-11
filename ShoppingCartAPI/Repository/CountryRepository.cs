using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
	public class CountryRepository : Repository<CountryMaster>, ICountryRepository
	{
		private readonly ApplicationDbContext _db;

		public CountryRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public async Task<CountryMaster> UpdateAsync(CountryMaster Country)
		{
			_db.CountryMaster.Update(Country);
			await _db.SaveChangesAsync();

			return Country;
		}
	}
}
