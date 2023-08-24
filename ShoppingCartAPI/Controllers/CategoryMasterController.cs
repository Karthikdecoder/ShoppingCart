using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repository.IRepository;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/CategoryMaster")]
    [ApiController]
    public class CategoryMasterController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _dbCategory;
        private string _userID;

        public CategoryMasterController(ICategoryRepository _categoryRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbCategory = _categoryRepository;
            _mapper = mapper;
            _response = new();
            _userID = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber);
        }

        [HttpGet]
        [Route("GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetCategory()
        {
            try
            {
                IEnumerable<CategoryMaster> roleList = await _dbCategory.GetAllAsync(u => u.IsDeleted == false);
                _response.Result = _mapper.Map<List<CategoryMasterDTO>>(roleList);
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

        //[HttpGet]
        //[Route("GetRole")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<APIResponse>> GetRole(int roleId)
        //{
        //    try
        //    {
        //        if (roleId == 0)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(_response);
        //        }

        //        var role = await _dbCategory.GetAsync(u => u.RoleId == roleId && u.IsDeleted == false);

        //        if (role == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(_response);
        //        }

        //        _response.Result = _mapper.Map<RoleMasterDTO>(role);
        //        _response.StatusCode = HttpStatusCode.OK;
        //        return Ok(_response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ResponseMessage = new List<string>() { ex.ToString() };
        //    }

        //    return _response;
        //}

        //[HttpPost]
        //[Route("CreateRole")]
        ////[Authorize(Roles = "admin")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<APIResponse>> CreateRole([FromBody] RoleMasterDTO roleMasterDTO)
        //{
        //    try
        //    {
        //        if (await _dbCategory.GetAsync(u => u.RoleName == roleMasterDTO.RoleName && u.IsDeleted == false) != null)
        //        {
        //            ModelState.AddModelError("ErrorMessages", "Role already exists!");
        //            return BadRequest(ModelState);
        //        }
                
        //        if (roleMasterDTO == null)
        //        {
        //            return BadRequest(roleMasterDTO);
        //        }

        //        RoleMaster roleMaster = _mapper.Map<RoleMaster>(roleMasterDTO);

        //        roleMaster.CreatedOn = DateTime.Now;
        //        roleMaster.UpdatedOn = DateTime.Now;
        //        roleMaster.IsDeleted = false;
        //        await _dbCategory.CreateAsync(roleMaster);

        //        _response.Result = _mapper.Map<RoleMasterDTO>(roleMaster);
        //        _response.StatusCode = HttpStatusCode.Created;

        //        return CreatedAtRoute("GetRole", new { id = roleMaster.RoleId }, _response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ResponseMessage = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}

        ////[Authorize(Roles = "admin")]
        //[HttpDelete]
        //[Route("RemoveRole")]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<APIResponse>> RemoveRole(int roleId)
        //{
        //    try
        //    {
        //        if (roleId == 0)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(_response);
        //        }

        //        var role = await _dbCategory.GetAsync(u => u.RoleId == roleId && u.IsDeleted == false);

        //        if (role == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(_response);
        //        }

        //        role.IsDeleted = true;
        //        await _dbCategory.UpdateAsync(role);

        //        _response.StatusCode = HttpStatusCode.NoContent;
        //        _response.IsSuccess = true;
        //        return Ok(_response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ResponseMessage = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}

        //[HttpPut]
        //[Route("UpdateRole")]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<APIResponse>> UpdateRole([FromBody] RoleMasterDTO roleMasterDTO)
        //{
        //    try
        //    {
        //        if (roleMasterDTO == null)
        //        {
        //            return BadRequest();
        //        }

        //        if (await _dbCategory.GetAsync(u => u.RoleId == roleMasterDTO.RoleId) == null)
        //        {
        //            ModelState.AddModelError("ErrorMessages", "Role ID is Invalid!");
        //            return BadRequest(ModelState);
        //        }

        //        RoleMaster model = _mapper.Map<RoleMaster>(roleMasterDTO);

        //        model.UpdatedOn = DateTime.Now;
        //        model.IsDeleted = false;
        //        await _dbCategory.UpdateAsync(model);

        //        _response.StatusCode = HttpStatusCode.NoContent;
        //        _response.IsSuccess = true;
        //        return Ok(_response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ResponseMessage = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}
    }
}
