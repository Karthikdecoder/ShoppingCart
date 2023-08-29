using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;
using System.Data;
using System.Security.Claims;

namespace ShoppingCartWeb.Controllers
{
    public class CategoryMasterController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private string _Role;
        public CategoryMasterController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IMapper mapper, IRegistrationService registrationService)
        {
            _userService = userService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _registrationService = registrationService;
        }

        public async Task<IActionResult> IndexCategoryMaster()
        {
            List<CategoryMasterDTO> list = new();

            var response = await _categoryService.GetAllCategoryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCategoryMaster()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategoryMaster(CategoryMasterDTO categoryMasterDTO)
        {

            APIResponse result = await _categoryService.CreateCategoryAsync<APIResponse>(categoryMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("IndexCategoryMaster", "CategoryMaster");
            }
            return View();

        }

        public async Task<IActionResult> UpdateCategoryMaster(int categoryId)
        {
            var categoryResponse = await _categoryService.GetCategoryAsync<APIResponse>(categoryId, HttpContext.Session.GetString(SD.SessionToken));

            if (categoryResponse != null && categoryResponse.IsSuccess)
            {
                CategoryMasterDTO model = JsonConvert.DeserializeObject<CategoryMasterDTO>(Convert.ToString(categoryResponse.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategoryMaster(CategoryMasterDTO model)
        {
            var response = await _categoryService.UpdateCategoryAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexCategoryMaster));
            }
            else
            {
                if (response.ErrorMessages.Count > 0)
                {
                    ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                }
            }

            return View(model);
        }


        public async Task<IActionResult> RemoveCategoryMaster(int categoryId)
        {
            var CategoryMasterResponse = await _categoryService.GetCategoryAsync<APIResponse>(categoryId, HttpContext.Session.GetString(SD.SessionToken));

            if (CategoryMasterResponse != null && CategoryMasterResponse.IsSuccess)
            {
                CategoryMasterDTO model = JsonConvert.DeserializeObject<CategoryMasterDTO>(Convert.ToString(CategoryMasterResponse.Result));

                return View(model);
            }


            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCategoryMaster(CategoryMasterDTO CategoryMasterDTO)
        {
            var response = await _categoryService.RemoveCategoryAsync<APIResponse>(CategoryMasterDTO.CategoryId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexCategoryMaster));
            }
            return View(CategoryMasterDTO);
        }
    }

}


