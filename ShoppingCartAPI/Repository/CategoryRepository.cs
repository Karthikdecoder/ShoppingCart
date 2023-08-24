using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
    public class CategoryRepository : Repository<CategoryMaster>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<CategoryMaster> UpdateAsync(CategoryMaster entity)
        {
            _db.CategoryMasterTable.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
