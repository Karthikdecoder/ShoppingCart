using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        private readonly ApplicationDbContext _db;

        public MenuRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Menu> UpdateAsync(Menu entity)
        {
            _db.Menu.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
