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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [Route("UpdateMenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateMenuRoleMapping([FromBody] MenuRoleMappingDTO menuRoleMappingDTO)
        {
            try
            {
                if (menuRoleMappingDTO == null)
                {
                    return BadRequest();
                }

                int MenuRoleMappingId = menuRoleMappingDTO.MenuRoleMappingId;

                //if (await _dbMenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId) == null)
                //{
                //    ModelState.AddModelError("ErrorMessages", "MenuRoleMapping ID is Invalid!");
                //    return BadRequest(ModelState);
                //}

                if (await _dbMenuRoleMapping.GetAsync(u => u.MenuId == menuRoleMappingDTO.MenuId && u.RoleId == menuRoleMappingDTO.RoleId) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (menuRoleMappingDTO.SelectedMenuIds.Count == 0)
                {
                    var uniquePairs = menuRoleMappingDTO.SelectedMenuIds
                        .Select(menuId => new { MenuId = menuId, RoleId = menuRoleMappingDTO.RoleId })
                        .Distinct()
                        .ToList();

                    // Get all MenuRoleMapping records with IsDeleted set to 1 for the given RoleId
                    // Get all MenuRoleMapping records with the same RoleId
                    var allRoleMappingsForRole = await _dbMenuRoleMapping.GetAllAsync(u => u.RoleId == menuRoleMappingDTO.RoleId && u.RoleMaster.IsDeleted == false && u.Menu.IsDeleted == false && u.IsDeleted == false, includeProperties: "RoleMaster,Menu");

                    foreach (var roleMapping in allRoleMappingsForRole)
                    {
                        // Check if the MenuId-RoleId pair of the current record exists in the uniquePairs list
                        var existsInSelectedIds = uniquePairs.Any(pair => pair.MenuId == roleMapping.MenuId);

                        if (!existsInSelectedIds)
                        {
                            // If the pair doesn't exist in the selected IDs, mark IsDeleted as 1
                            roleMapping.IsDeleted = true;
                            await _dbMenuRoleMapping.UpdateAsync(roleMapping);
                        }
                    }
                }

                foreach (int menuId in menuRoleMappingDTO.SelectedMenuIds)
                {


                    MenuRoleMapping model = _mapper.Map<MenuRoleMapping>(menuRoleMappingDTO);

                    model.MenuId = menuId;


                    if (_userId == null)
                    {
                        _userId = "0";
                    }

                    //// Assume you have retrieved menu IDs from the parameter into a List<int> parameterMenuIds
                    //var parentMenuIds = await _dbmenu.GetAsync(u => u.ParentId == 0);

                    //// Check if there are any parent menus in the parameter
                    //var parentMenusInParam = parameterMenuIds.Intersect(parentMenuIds).Any();

                    //if (parentMenusInParam)
                    //{
                    //    // If parent menus are found, check if there are any child menu IDs in the parameter
                    //    var childMenusInParam = parameterMenuIds.Except(parentMenuIds).Any();

                    //    if (childMenusInParam)
                    //    {
                    //        // Throw an error indicating that child menus cannot be passed without their parent menus
                    //        throw new Exception("Child menus cannot be passed without their parent menus.");
                    //    }
                    //}


                    // Create a list of unique MenuId-RoleId pairs from the selected menuRoleMappingDTO
                    var uniquePairs = menuRoleMappingDTO.SelectedMenuIds
                        .Select(menuId => new { MenuId = menuId, RoleId = model.RoleId })
                        .Distinct()
                        .ToList();

                    // Get all MenuRoleMapping records with the same RoleId
                    var allRoleMappingsForRole = await _dbMenuRoleMapping.GetAllAsync(u => u.RoleId == model.RoleId && u.RoleMaster.IsDeleted == false && u.Menu.IsDeleted == false && u.IsDeleted == false, includeProperties: "RoleMaster,Menu");

                    foreach (var roleMapping in allRoleMappingsForRole)
                    {
                        // Check if the MenuId-RoleId pair of the current record exists in the uniquePairs list
                        var existsInSelectedIds = uniquePairs.Any(pair => pair.MenuId == roleMapping.MenuId);

                        if (!existsInSelectedIds)
                        {
                            // If the pair doesn't exist in the selected IDs, mark IsDeleted as 1
                            roleMapping.IsDeleted = true;
                            await _dbMenuRoleMapping.UpdateAsync(roleMapping);
                        }
                    }

                    // Get all MenuRoleMapping records with IsDeleted set to 1 for the given RoleId
                    var markedForDeletion = await _dbMenuRoleMapping.GetAllAsync(u => u.RoleId == model.RoleId && u.RoleMaster.IsDeleted == false && u.Menu.IsDeleted == false, includeProperties: "RoleMaster,Menu");

                    foreach (var roleMapping in markedForDeletion)
                    {
                        // Check if the MenuId of the current record exists in the selected menuRoleMappingDTO
                        if (menuRoleMappingDTO.SelectedMenuIds.Contains(roleMapping.MenuId))
                        {
                            // If the MenuId is in the selected IDs, mark IsDeleted as 0
                            roleMapping.IsDeleted = false;
                            await _dbMenuRoleMapping.UpdateAsync(roleMapping);
                        }
                    }


                    // Check if a MenuRoleMapping record with the same MenuId and RoleId already exists
                    var existingRecord = await _dbMenuRoleMapping.GetAsync(u => u.MenuId == menuId && u.RoleId == model.RoleId);

                    if (existingRecord != null)
                    {
                        // Skip the update if the record already exists
                        continue;
                    }

                    model.UpdatedOn = DateTime.Now;
                    model.UpdatedBy = int.Parse(_userId);
                    model.IsDeleted = false;
                    await _dbMenuRoleMapping.UpdateAsync(model);
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

        [HttpPut]
        [Authorize(Roles = "Admin")]
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
