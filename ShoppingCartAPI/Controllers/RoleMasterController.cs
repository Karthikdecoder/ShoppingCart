using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository.IRepository;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/RoleMaster")]
    [ApiController]
    public class RoleMasterController : ControllerBase  
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IRoleMasterRepository _dbRoles;
        private readonly IMenuRoleMappingRepository _dbMenuRoleMapping;
        private string _userId;

        public RoleMasterController(IRoleMasterRepository roleMasterRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor,IMenuRoleMappingRepository dbMenuRoleMapping)
        {
            _dbRoles = roleMasterRepository;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _dbMenuRoleMapping = dbMenuRoleMapping;
        }

        [HttpGet]
        [Route("GetRoles")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetRoles()
        {
            try
            {
                IEnumerable<RoleMaster> roleList = await _dbRoles.GetAllAsync();
                _response.Result = _mapper.Map<List<RoleMasterDTO>>(roleList);
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
        [Route("GetRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetRole(int roleId)
        {
            try
            {
                if (roleId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var role = await _dbRoles.GetAsync(u => u.RoleId == roleId && u.IsDeleted == false);

                if (role == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<RoleMasterDTO>(role);
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
        [Authorize(Roles = "Admin")]
        [Route("CreateRole")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateRole([FromBody] RoleMasterDTO roleMasterDTO)
        {
            try
            {
                if (await _dbRoles.GetAsync(u => u.RoleName == roleMasterDTO.RoleName) != null)
                {
                    //ModelState.AddModelError("ErrorMessages", "Role already exists!");
                    //return BadRequest(ModelState);
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (roleMasterDTO == null)
                {
                    return BadRequest(roleMasterDTO);
                }

                RoleMaster roleMaster = _mapper.Map<RoleMaster>(roleMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                roleMaster.CreatedOn = DateTime.Now;
                roleMaster.CreatedBy = int.Parse(_userId);
                roleMaster.UpdatedOn = DateTime.Now;
                roleMaster.UpdatedBy = int.Parse(_userId);
                roleMaster.IsDeleted = false;
                await _dbRoles.CreateAsync(roleMaster);

                RoleMaster createdRole = await _dbRoles.GetAsync(u => u.RoleName == roleMasterDTO.RoleName);

                // Now you can access the RoleId
                int RoleIdfromDb = createdRole.RoleId;

                MenuRoleMapping menuRoleMapping = new()
                {
                    RoleId = RoleIdfromDb,
                    MenuId = 12,
                    CreatedOn = DateTime.Now,
                    CreatedBy = int.Parse(_userId),
                    UpdatedOn = DateTime.Now,
                    UpdatedBy = int.Parse(_userId),
                    IsDeleted = false
                };

                await _dbMenuRoleMapping.CreateAsync(menuRoleMapping);

                _response.Result = _mapper.Map<RoleMasterDTO>(roleMaster);
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
        [Route("RemoveRole")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveRole(int roleId)
        {
            try
            {
                if (roleId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var role = await _dbRoles.GetAsync(u => u.RoleId == roleId && u.IsDeleted == false);

                if (role == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                role.IsDeleted = true;
                await _dbRoles.UpdateAsync(role);

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
        [Route("UpdateRole")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateRole([FromBody] RoleMasterDTO roleMasterDTO)
        {
            try
            {
                if (roleMasterDTO == null)
                {
                    return BadRequest();
                }

                if (await _dbRoles.GetAsync(u => u.RoleId == roleMasterDTO.RoleId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Role ID is Invalid!");
                    return BadRequest(ModelState);
                }

                if (await _dbRoles.GetAsync(u => u.RoleName == roleMasterDTO.RoleName && u.RoleId != roleMasterDTO.RoleId && u.RoleId != roleMasterDTO.RoleId) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                RoleMaster model = _mapper.Map<RoleMaster>(roleMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _dbRoles.UpdateAsync(model);

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
        [Route("EnableRole")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnableRole(int roleId)
        {
            try
            {
                if (roleId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var roleMasterDTO = await _dbRoles.GetAsync(u => u.RoleId == roleId && u.IsDeleted == true);

                if (roleMasterDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                RoleMaster model = _mapper.Map<RoleMaster>(roleMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _dbRoles.UpdateAsync(model);

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
