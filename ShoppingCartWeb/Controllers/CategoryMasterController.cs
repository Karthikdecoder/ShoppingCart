using AutoMapper;
using Microsoft.AspNetCore.Authentication;
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

        public async Task<IActionResult> ViewCategoryMaster(int categoryId)
        {
            CategoryMasterDTO categoryDetail = new();

            var categoryResponse = await _categoryService.GetCategoryAsync<APIResponse>(categoryId, HttpContext.Session.GetString(SD.SessionToken));

            if (categoryResponse != null && categoryResponse.IsSuccess)
            {
                CategoryMasterDTO model = JsonConvert.DeserializeObject<CategoryMasterDTO>(Convert.ToString(categoryResponse.Result));
                categoryDetail = _mapper.Map<CategoryMasterDTO>(model);
            }

            return View(categoryDetail);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategoryMaster()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategoryMaster(CategoryMasterDTO categoryMasterDTO)
        {
            if (ModelState.IsValid)
            {
                APIResponse response = await _categoryService.CreateCategoryAsync<APIResponse>(categoryMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexCategoryMaster", "CategoryMaster");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(categoryMasterDTO);
            }
            return View();

        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategoryMaster(CategoryMasterDTO categoryMasterDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.UpdateCategoryAsync<APIResponse>(categoryMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction(nameof(IndexCategoryMaster));
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexCategoryMaster));
            }

            return View(categoryMasterDTO);
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> RemoveCategoryMaster(int categoryId)
        //{
        //    var CategoryMasterResponse = await _categoryService.GetCategoryAsync<APIResponse>(categoryId, HttpContext.Session.GetString(SD.SessionToken));

        //    if (CategoryMasterResponse != null && CategoryMasterResponse.IsSuccess)
        //    {
        //        CategoryMasterDTO model = JsonConvert.DeserializeObject<CategoryMasterDTO>(Convert.ToString(CategoryMasterResponse.Result));

        //        return View(model);
        //    }


        //    return NotFound();
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveCategoryMaster(int categoryId)
        {
            var response = await _categoryService.RemoveCategoryAsync<APIResponse>(categoryId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(IndexCategoryMaster));
            }

            TempData["success"] = "Error encountered";
            return View();
        }
    }

}


