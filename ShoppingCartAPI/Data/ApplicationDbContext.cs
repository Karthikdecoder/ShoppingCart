using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

        public DbSet<Registration> RegistrationTable { get; set; }
        public DbSet<RoleMaster> RoleMasterTable { get; set; }
        public DbSet<CategoryMaster> CategoryMasterTable { get; set; }
        public DbSet<User> UserTable { get; set; }

    }
}
