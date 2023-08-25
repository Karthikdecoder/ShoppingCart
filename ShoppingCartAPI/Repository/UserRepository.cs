using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoppingCartAPI.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration, IMapper mapper) : base(db)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
        }

        public bool IsUniqueUser(string UserName)
        {
            var user = _db.User.FirstOrDefault(u => u.UserName == UserName);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var userLoginFromDb = _db.User.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() && u.Password.ToLower() == loginRequestDTO.Password.ToLower());

            string Role = string.Empty;

            Role = _db.User.Where(u => u.RegistrationId == userLoginFromDb.RegistrationId).Select(u => u.RoleMaster.RoleName).FirstOrDefault();

            var userRegistrationFromDb = _db.Registration.FirstOrDefault(u => u.RegistrationId == userLoginFromDb.RegistrationId);

            if (userLoginFromDb == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    UserRegistration = null
                };
            }


            string UserId = string.Empty;
            UserId = System.Convert.ToString(userLoginFromDb.RegistrationId);

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userLoginFromDb.UserName.ToString()),
                    new Claim(ClaimTypes.Role, Role),
                    new Claim(ClaimTypes.NameIdentifier, UserId)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                UserRegistration = userRegistrationFromDb
            };

            return loginResponseDTO;
        }

        public async Task<User> RegisterAsync(UserDTO userRegisterDTO, string userId)
        {
            var registrationFromDb = _db.Registration.FirstOrDefault(u => u.RegistrationId == userRegisterDTO.RegistrationId);

            var roleFromDb = _db.RoleMaster.FirstOrDefault(u => u.RoleId == userRegisterDTO.RoleId);

            int userRegistrationId = 0;
            userRegistrationId = _db.Registration.Where(u => u.RegistrationId == registrationFromDb.RegistrationId).Select(u => u.RegistrationId).FirstOrDefault();

            string userName = _db.Registration.Where(u => u.RegistrationId == registrationFromDb.RegistrationId).Select(u => u.Email).FirstOrDefault();

            int roleId = _db.RoleMaster.Where(u => u.RoleId == roleFromDb.RoleId).Select(u => u.RoleId).FirstOrDefault();

            User user = _mapper.Map<User>(userRegisterDTO);

            user.UserName = userRegisterDTO.UserName;
            user.Password = userRegisterDTO.Password;
            user.RoleId = roleId;
            user.RegistrationId = userRegistrationId;
            user.CreatedBy = int.Parse(userId);
            user.CreatedOn = DateTime.Now;
            user.UpdatedBy = int.Parse(userId);
            user.UpdatedOn = DateTime.Now;
            user.IsDeleted = false;

            _db.User.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            var userList = await _db.User.Include(c => c.Registration).Include(u => u.RoleMaster).Where(u => u.IsDeleted == false).ToListAsync();
            return userList;
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var user = await _db.User.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted == false);
            return user;
        }

        public async Task<User> UpdateUserAsync(User User, string userId)
        {
            User.UpdatedBy = int.Parse(userId);
            _db.Update(User);
            await SaveAsync();

            return User;
        }

        public async Task RemoveAsync(User User)
        {
            User.IsDeleted = true;
            _db.Update(User);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
