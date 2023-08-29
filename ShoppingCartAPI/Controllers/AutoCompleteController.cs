using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository.IRepository;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/AutoComplete")]
    [ApiController]
    public class AutoCompleteController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IRoleMasterRepository _dbRoles;
        private string _userId;
        private readonly ApplicationDbContext _db;

        public AutoCompleteController(IRoleMasterRepository roleMasterRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ApplicationDbContext db)
        {
            _dbRoles = roleMasterRepository;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> AutoComplete(string prefix)
        {
            var result = _db.Registration.Where(u => u.FirstName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(u => u.FirstName);

            return Ok(result);
        }

    }
}
