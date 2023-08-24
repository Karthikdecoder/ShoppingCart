using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository.IRepository;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoppingCartAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration, IMapper mapper) 
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            var userList = _db.UserTable.Where( u => u.IsDeleted == false).ToList();

            return userList;
        }

        public async Task<List<Registration>> GetAllRegistrationAsync()
        {
            var registrationList = _db.RegistrationTable.Where(u => u.IsDeleted == false).ToList();

            return registrationList;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.RegistrationTable.FirstOrDefault(u => u.Email == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var userLoginFromDb = _db.UserTable.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() && u.Password.ToLower() == loginRequestDTO.Password.ToLower());

            string Role = string.Empty;

            Role = _db.UserTable.Where(u => u.RegistrationId == userLoginFromDb.RegistrationId).Select(u => u.RoleMaster.RoleName).FirstOrDefault();

            var userRegistrationFromDb = _db.RegistrationTable.FirstOrDefault(u => u.RegistrationId == userLoginFromDb.RegistrationId);

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

        public async Task<Registration> Register(RegistrationDTO registrationRequestDTO, string userId)
        {
            Registration userRegistration = new()
            {
                FirstName = registrationRequestDTO.FirstName,
                LastName = registrationRequestDTO.LastName,
                Gender = registrationRequestDTO.Gender,
                Email = registrationRequestDTO.Email,
                DateOfBirth = registrationRequestDTO.DateOfBirth,
                ContactNo = registrationRequestDTO.ContactNo,
                Address = registrationRequestDTO.Address,
                PostalCode = registrationRequestDTO.PostalCode,
                CategoryId = registrationRequestDTO.CategoryId,
                CreatedBy = int.Parse(userId),
                CreatedOn = DateTime.Now,
                UpdatedBy = int.Parse(userId),
                UpdatedOn = DateTime.Now,
                IsDeleted = false
            };

            _db.RegistrationTable.Add(userRegistration);
            await _db.SaveChangesAsync();

            //user.Password = "";
            return userRegistration;
        }

        public async Task<User> UserRegister(UserDTO userRegisterDTO, string userId)
        {
            var registrationFromDb = _db.RegistrationTable.FirstOrDefault(u => u.RegistrationId == userRegisterDTO.RegistrationId);

            var roleFromDb = _db.RoleMasterTable.FirstOrDefault(u => u.RoleId == userRegisterDTO.RoleId);

            int userRegistrationId = 0;
            userRegistrationId = _db.RegistrationTable.Where(u => u.RegistrationId == registrationFromDb.RegistrationId).Select(u => u.RegistrationId).FirstOrDefault();

            string userName = _db.RegistrationTable.Where(u => u.RegistrationId == registrationFromDb.RegistrationId).Select(u => u.Email).FirstOrDefault();

            int roleId = _db.RoleMasterTable.Where(u => u.RoleId == roleFromDb.RoleId).Select(u => u.RoleId).FirstOrDefault();

            User userLogin = new()
            {
                UserName = userName,
                Password = userRegisterDTO.Password,
                RoleId = roleId,
                RegistrationId = userRegistrationId,
                CreatedBy = int.Parse(userId),
                CreatedOn = DateTime.Now,
                UpdatedBy = int.Parse(userId),
                UpdatedOn = DateTime.Now,
                IsDeleted = false
            };

            _db.UserTable.Add(userLogin);
            await _db.SaveChangesAsync();

            return userLogin;
        }

        
    }
}
