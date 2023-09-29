using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository;
using ShoppingCartAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/MenuRoleMapping")]
    [ApiController]
    public class MenuRoleMappingController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IMenuRoleMappingRepository _dbMenuRoleMapping;
        private readonly IMenuRepository _dbmenu;
        private string _userId;
        private string _roleId;

        public MenuRoleMappingController(IMenuRoleMappingRepository _MenuRoleMappingRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMenuRepository dbmenu)
        {
            _dbMenuRoleMapping = _MenuRoleMappingRepository;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _roleId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            _dbmenu = dbmenu;
        }

        [HttpGet]
        [Route("GetAllMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllMenuRoleMapping()
        {
            try
            {
                IEnumerable<MenuRoleMapping> menuRoleMappingList = await _dbMenuRoleMapping.GetAllAsync(includeProperties: "RoleMaster,Menu");
                _response.Result = _mapper.Map<List<MenuRoleMappingDTO>>(menuRoleMappingList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpGet]
        [Route("GetAllMenuByRoleId")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllMenuByRoleId()
        {
            try
            {
                int roleId = int.Parse(_roleId);

                IEnumerable<MenuRoleMapping> menuRoleMappingList = await _dbMenuRoleMapping.GetAllAsync(u => u.RoleId == roleId && u.IsDeleted == false, includeProperties: "RoleMaster,Menu");
                _response.Result = _mapper.Map<List<MenuRoleMappingDTO>>(menuRoleMappingList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpGet]
        [Route("GetMenuIdByRoleId")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetMenuIdByRoleId(int roleId)
        {
            try
            {
                IEnumerable<MenuRoleMapping> menuRoleMappingList = await _dbMenuRoleMapping.GetAllAsync(u => u.RoleId == roleId && u.IsDeleted == false && u.Menu.IsDeleted == false && u.RoleMaster.IsDeleted == false, includeProperties: "RoleMaster,Menu");
                _response.Result = _mapper.Map<List<MenuRoleMappingDTO>>(menuRoleMappingList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpGet]
        [Route("GetMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetMenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuRoleMapping = await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == false);

                if (MenuRoleMapping == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<MenuRoleMappingDTO>(MenuRoleMapping);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }

            return _response;
        }




        [HttpPost]
        [Route("CreateMenuRoleMapping")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateMenuRoleMapping([FromBody] MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            try
            {
                if (await _dbMenuRoleMapping.GetAsync(u => u.MenuId == MenuRoleMappingDTO.MenuId && u.RoleId == MenuRoleMappingDTO.RoleId) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (MenuRoleMappingDTO == null)
                {
                    return BadRequest(MenuRoleMappingDTO);
                }

                MenuRoleMapping model = _mapper.Map<MenuRoleMapping>(MenuRoleMappingDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.CreatedOn = DateTime.Now;
                model.CreatedBy = int.Parse(_userId);
                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _dbMenuRoleMapping.CreateAsync(model);

                _response.Result = _mapper.Map<MenuRoleMappingDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete]
        [Authorize]
        [Route("RemoveMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveMenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var menuRoleMappingModelFromDb = await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == false);


                if (menuRoleMappingModelFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                var menuId = menuRoleMappingModelFromDb.MenuId;
                var roleId = menuRoleMappingModelFromDb.RoleId; // Get the RoleId from menuRoleMappingModelFromDb

                // Retrieve the associated menu from the Menu table
                var menu = await _dbmenu.GetAsync(u => u.MenuId == menuId);

                if (menu != null && menu.ParentId == 0)
                {
                    // Retrieve all child menus with ParentId = menuId
                    var childMenus = await _dbmenu.GetAllAsync(u => u.ParentId == menuId);

                    // Update IsDeleted property for child menu role mappings in the MenuRoleMapping table
                    var childMenuIds = childMenus.Select(child => child.MenuId).ToList();

                    // Filter child menu role mappings by MenuId and RoleId
                    var childMenuRoleMappings = await _dbMenuRoleMapping.GetAllAsync(u => childMenuIds.Contains(u.MenuId) && u.RoleId == roleId);

                    foreach (var childMenuRoleMapping in childMenuRoleMappings)
                    {
                        childMenuRoleMapping.IsDeleted = true;
                        await _dbMenuRoleMapping.UpdateAsync(childMenuRoleMapping);
                    }
                }

                // Soft delete the current menu role mapping
                menuRoleMappingModelFromDb.IsDeleted = true;
                await _dbMenuRoleMapping.UpdateAsync(menuRoleMappingModelFromDb);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut]
        [Authorize]
        [Route("UpdateMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateMenuRoleMapping([FromBody] MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            try
            {
                if (MenuRoleMappingDTO == null)
                {
                    return BadRequest();
                }

                int MenuRoleMappingId = MenuRoleMappingDTO.MenuRoleMappingId;

                if (await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "MenuRoleMapping ID is Invalid!");
                    return BadRequest(ModelState);
                }

                if (await _dbMenuRoleMapping.GetAsync(u => u.MenuId == MenuRoleMappingDTO.MenuId && u.RoleId == MenuRoleMappingDTO.RoleId) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                MenuRoleMapping model = _mapper.Map<MenuRoleMapping>(MenuRoleMappingDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _dbMenuRoleMapping.UpdateAsync(model);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut]
        [Authorize]
        [Route("EnableMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnableMenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuRoleMappingDTO = await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == true);

                if (MenuRoleMappingDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                MenuRoleMapping model = _mapper.Map<MenuRoleMapping>(MenuRoleMappingDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                // Enable the current MenuRoleMapping
                model.IsDeleted = false;
                await _dbMenuRoleMapping.UpdateAsync(model);

                // Check if the associated menu has ParentId = 0
                var menu = await _dbmenu.GetAsync(u => u.MenuId == model.MenuId);

                if (menu != null && menu.ParentId == 0)
                {
                    // Retrieve all child menus with ParentId = model.MenuId
                    var childMenus = await _dbmenu.GetAllAsync(u => u.ParentId == model.MenuId);

                    // Extract the MenuIds of child menus
                    var childMenuIds = childMenus.Select(child => child.MenuId).ToList();

                    // Retrieve and enable child menu role mappings for the same RoleId
                    var childMenuRoleMappings = await _dbMenuRoleMapping.GetAllAsync(u => childMenuIds.Contains(u.MenuId) && u.RoleId == model.RoleId);

                    foreach (var childMenuRoleMapping in childMenuRoleMappings)
                    {
                        childMenuRoleMapping.IsDeleted = false; // Set IsDeleted to false (enabled)
                        await _dbMenuRoleMapping.UpdateAsync(childMenuRoleMapping);
                    }
                }


                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ResponseMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
