using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
    public class MenuRoleMappingRepository : Repository<MenuRoleMapping>, IMenuRoleMappingRepository
    {
        private readonly ApplicationDbContext _db;

        public MenuRoleMappingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<MenuRoleMapping> UpdateAsync(MenuRoleMapping entity)
        {
            _db.MenuRoleMapping.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
