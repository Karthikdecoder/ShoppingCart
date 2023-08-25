
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
    public class RoleMasterRepository : Repository<RoleMaster>, IRoleMasterRepository
    {
        private readonly ApplicationDbContext _db;

        public RoleMasterRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<RoleMaster> UpdateAsync(RoleMaster entity)
        {
            _db.RoleMaster.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
