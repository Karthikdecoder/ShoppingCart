using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository;
using ShoppingCartAPI.Repository.IRepository;
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
        private readonly IMenuRoleMappingRepository _dbmenuRoleMapping;
        private string _userId;

        public MenuRoleMappingController(IMenuRoleMappingRepository _menuRoleMappingRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbmenuRoleMapping = _menuRoleMappingRepository;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        [Route("GetAllmenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllmenuRoleMapping()
        {
            try
            {
                IEnumerable<MenuRoleMapping> roleList = await _dbmenuRoleMapping.GetAllAsync();
                _response.Result = _mapper.Map<List<MenuRoleMappingDTO>>(roleList);
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
        [Route("GetmenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetmenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuRoleMapping = await _dbmenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == false);

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
        [Route("CreatemenuRoleMapping")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreatemenuRoleMapping([FromBody] MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            try
            {
                //if (await _dbmenuRoleMapping.GetAsync(u => u.menuRoleMappingName == MenuRoleMappingDTO.menuRoleMappingName) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (MenuRoleMappingDTO == null)
                {
                    return BadRequest(MenuRoleMappingDTO);
                }

                MenuRoleMapping MenuRoleMapping = _mapper.Map<MenuRoleMapping>(MenuRoleMappingDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                MenuRoleMapping.CreatedOn = DateTime.Now;
                MenuRoleMapping.CreatedBy = int.Parse(_userId);
                MenuRoleMapping.UpdatedOn = DateTime.Now;
                MenuRoleMapping.UpdatedBy = int.Parse(_userId);
                MenuRoleMapping.IsDeleted = false;
                await _dbmenuRoleMapping.CreateAsync(MenuRoleMapping);

                _response.Result = _mapper.Map<MenuRoleMappingDTO>(MenuRoleMapping);
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
        [Route("RemovemenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemovemenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var menuRoleMapping = await _dbmenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == false);

                if (menuRoleMapping == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                menuRoleMapping.IsDeleted = true;
                await _dbmenuRoleMapping.UpdateAsync(menuRoleMapping);

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
        [Route("UpdatemenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdatemenuRoleMapping([FromBody] MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            try
            {
                if (MenuRoleMappingDTO == null)
                {
                    return BadRequest();
                }

                int MenuRoleMappingId = MenuRoleMappingDTO.MenuRoleMappingId;

                if (await _dbmenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "menuRoleMapping ID is Invalid!");
                    return BadRequest(ModelState);
                }

                //if (await _dbmenuRoleMapping.GetAsync(u => u.menuRoleMappingName == MenuRoleMappingDTO.menuRoleMappingName && u.MenuRoleMappingId != MenuRoleMappingDTO.MenuRoleMappingId) != null)
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
                await _dbmenuRoleMapping.UpdateAsync(model);

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
        [Route("EnablemenuRoleMapping")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnablemenuRoleMapping(int MenuRoleMappingId)
        {
            try
            {
                if (MenuRoleMappingId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuRoleMappingDTO = await _dbmenuRoleMapping.GetAsync(u => u.MenuRoleMappingId == MenuRoleMappingId && u.IsDeleted == true);

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

                model.IsDeleted = false;
                await _dbmenuRoleMapping.UpdateAsync(model);

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
