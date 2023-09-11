using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
    public class RegistrationRepository : Repository<Registration>, IRegistrationRepository
    {
        private readonly ApplicationDbContext _db;

        public RegistrationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        

        public async Task<Registration> UpdateAsync(Registration registration)
        {
            _db.Registration.Update(registration);
            await _db.SaveChangesAsync();

            return registration;
        }
    }
}
