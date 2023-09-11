using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

        public DbSet<RoleMaster> RoleMaster { get; set; }
        public DbSet<CategoryMaster> CategoryMaster { get; set; }
        public DbSet<CountryMaster> CountryMaster { get; set; }
        public DbSet<StateMaster> StateMaster { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Registration> Registration { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<MenuRoleMapping> MenuRoleMapping { get; set; }
    }
}
