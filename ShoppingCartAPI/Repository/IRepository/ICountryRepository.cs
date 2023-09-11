using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface ICountryRepository : IRepository<CountryMaster>
    {
        Task<CountryMaster> UpdateAsync(CountryMaster Country);
    }
}
