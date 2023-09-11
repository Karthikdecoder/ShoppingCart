using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using System.Linq.Expressions;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface IRegistrationRepository : IRepository<Registration>
    {
        Task<Registration> UpdateAsync(Registration registration);
    }
}
