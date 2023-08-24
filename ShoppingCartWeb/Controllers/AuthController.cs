using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppingCartWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private string _Role;
        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService)
        {
            _authService = authService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO obj = new();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(obj);

            if (response != null && response.IsSuccess)
            {
                LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));
                
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.Token);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value)); 
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(SD.SessionToken, model.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
                return View(obj);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            CategoryMasterVM categoryMaster = new();

            var response = await _categoryService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess) 
            {
                categoryMaster.CategoryList = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
                    {
                        Text = i.CategoryName,
                        Value = i.CategoryId.ToString()
                    }); 
            }

            return View(categoryMaster);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RoleMasterCreateVM obj)
        {

            APIResponse result = await _authService.RegisterAsync<APIResponse>(obj.Registration, HttpContext.Session.GetString(SD.SessionToken));

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserRegister()
        {
            UserRegisterationVM userRegister = new();

            var registrationResponse = await _authService.GetAllRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (registrationResponse != null && registrationResponse.IsSuccess)
            {
                userRegister.RegistrationList = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(registrationResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.Email,
                    Value = i.UserRegistrationId.ToString()
                });
            }

            var roleResponse = await _roleService.GetAllRolesAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (roleResponse != null && roleResponse.IsSuccess)
            {

                userRegister.RoleList = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(roleResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.RoleName,
                    Value = i.RoleId.ToString()
                });
            }

            return View(userRegister);
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserRegister(UserRegisterationVM obj)
        {

            APIResponse result = await _authService.UserRegisterAsync<APIResponse>(obj.User, HttpContext.Session.GetString(SD.SessionToken));

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken, "");
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}
