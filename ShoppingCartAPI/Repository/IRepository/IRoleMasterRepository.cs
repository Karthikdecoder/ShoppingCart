using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface IRoleMasterRepository : IRepository<RoleMaster>
    {
        Task<RoleMaster> UpdateAsync(RoleMaster entity); 
    }
}
