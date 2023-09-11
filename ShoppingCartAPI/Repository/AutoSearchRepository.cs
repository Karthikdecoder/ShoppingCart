using AutoMapper;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;

namespace ShoppingCartAPI.Repository
{
    public class AutoSearchRepository : Repository<Registration>, IAutoSearchRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        private readonly IMapper _mapper;

        public AutoSearchRepository(ApplicationDbContext db, IConfiguration configuration, IMapper mapper) : base(db)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
        }
        public Task<List<Registration>> AutoComplete()
        {
            return null;
        }
    }
}
