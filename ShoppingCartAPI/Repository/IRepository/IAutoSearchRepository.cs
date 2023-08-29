using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface IAutoSearchRepository
    {
        Task<List<Registration>> AutoComplete();
    }
}
