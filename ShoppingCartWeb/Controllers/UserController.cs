using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppingCartWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private string _Role;
        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IMapper mapper, IRegistrationService registrationService)
        {
            _userService = userService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _registrationService = registrationService;
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
            APIResponse response = await _userService.LoginAsync<APIResponse>(obj);

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
                ModelState.AddModelError("", "Username or password is incorrect");
                //ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
                return View();
            }
        }

        public async Task<IActionResult> IndexUser()
        {
            List<UserDTO> list = new();

            var response = await _userService.GetAllUserAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<UserDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register()
        {
            RegisterVM registerVM = new();

            var registrationResponse = await _registrationService.GetAllRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (registrationResponse != null && registrationResponse.IsSuccess)
            {
                registerVM.registrationList = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(registrationResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.FirstName,
                    Value = i.RegistrationId.ToString()
                });
            }

            var roleResponse = await _roleService.GetAllRoleAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (roleResponse != null && roleResponse.IsSuccess)
            {
                registerVM.roleList = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(roleResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.RoleName,
                    Value = i.RoleId.ToString()
                });
            }

            return View(registerVM);
        }

        public async Task<JsonResult> GetRegistrationName(string Prefix)
        {

            RegisterVM registerVM = new();

            var nameList = new List<RegistrationDTO>();

            var nameResponse = await _userService.GetRegistrationNameServiceAsync<APIResponse>(Prefix, HttpContext.Session.GetString(SD.SessionToken));

            if (nameResponse != null)
            {
                nameList = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(nameResponse.Result));
            }

            return Json(nameList);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                APIResponse result = await _userService.RegisterAsync<APIResponse>(registerVM.User, HttpContext.Session.GetString(SD.SessionToken));

                if (result != null && result.IsSuccess)
                {
                    TempData["success"] = "Registered successfully";
                    return RedirectToAction("IndexUser", "User");
                }

                TempData["error"] = result.ResponseMessage[0].ToString();
                return RedirectToAction("Register", "User");
                //return View(registerVM);
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int userId)
        {
            UpdateUserVM updateUserVM = new();
            var response = await _userService.GetUserAsync<APIResponse>(userId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                UserDTO model = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));
                updateUserVM.User = _mapper.Map<UserDTO>(model);
            }

            response = await _registrationService.GetAllRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                updateUserVM.registrationList = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
                {
                    Text = i.FirstName,
                    Value = i.RegistrationId.ToString()
                });
                //return View(updateUserVM);
            }

            response = await _roleService.GetAllRoleAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                updateUserVM.roleList = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
                {
                    Text = i.RoleName,
                    Value = i.RoleId.ToString()
                });
                return View(updateUserVM);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserVM updateUserVM)
        {
            if (ModelState.IsValid)
            {
                var response = await _userService.UpdateUserAsync<APIResponse>(updateUserVM.User, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction(nameof(IndexUser));
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexUser));
            }
            return View(updateUserVM);
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> RemoveUser(int userID)
        //{
        //    RemoveUserVM removeUserVM = new();
        //    var response = await _userService.GetUserAsync<APIResponse>(userID, HttpContext.Session.GetString(SD.SessionToken));

        //    if (response != null && response.IsSuccess)
        //    {
        //        UserDTO model = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));
        //        removeUserVM.User = model;
        //    }

        //    response = await _registrationService.GetAllRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

        //    if (response != null && response.IsSuccess)
        //    {
        //        removeUserVM.registrationList = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
        //        {
        //            Text = i.FirstName,
        //            Value = i.RegistrationId.ToString()
        //        });
        //        return View(removeUserVM);
        //    }

        //    response = await _roleService.GetAllRoleAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

        //    if (response != null && response.IsSuccess)
        //    {
        //        removeUserVM.roleList = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
        //        {
        //            Text = i.RoleName,
        //            Value = i.RoleId.ToString()
        //        });
        //        return View(removeUserVM);
        //    }

        //    return NotFound();
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            var response = await _userService.RemoveUserAsync<APIResponse>(userId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(IndexUser));
            }

            TempData["success"] = "Error encountered";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken, "");
            return RedirectToAction("Login", "User");
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Login", "User");
        }
    }

}
