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
    [Route("api/CategoryMaster")]
    [ApiController]
    public class CategoryMasterController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _dbCategory;
        private string _userId;

        public CategoryMasterController(ICategoryRepository _categoryRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbCategory = _categoryRepository;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        [Route("GetAllCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllCategory()
        {
            try
            {
                IEnumerable<CategoryMaster> roleList = await _dbCategory.GetAllAsync();
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

        [HttpGet]
        [Route("GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCategory(int categoryId)
        {
            try
            {
                if (categoryId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var categoryMaster = await _dbCategory.GetAsync(u => u.CategoryId == categoryId && u.IsDeleted == false);

                if (categoryMaster == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<CategoryMasterDTO>(categoryMaster);
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
        [Route("CreateCategory")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CategoryMasterDTO categoryMasterDTO)
        {
            try
            {
                if (await _dbCategory.GetAsync(u => u.CategoryName == categoryMasterDTO.CategoryName && u.IsDeleted == false) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (categoryMasterDTO == null)
                {
                    return BadRequest(categoryMasterDTO);
                }

                CategoryMaster categoryMaster = _mapper.Map<CategoryMaster>(categoryMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                categoryMaster.CreatedOn = DateTime.Now;
                categoryMaster.CreatedBy = int.Parse(_userId);
                categoryMaster.UpdatedOn = DateTime.Now;
                categoryMaster.UpdatedBy = int.Parse(_userId);
                categoryMaster.IsDeleted = false;
                await _dbCategory.CreateAsync(categoryMaster);

                _response.Result = _mapper.Map<CategoryMasterDTO>(categoryMaster);
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
        [Route("RemoveCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveCategory(int categoryId)
        {
            try
            {
                if (categoryId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var category = await _dbCategory.GetAsync(u => u.CategoryId == categoryId && u.IsDeleted == false);

                if (category == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                category.IsDeleted = true;
                await _dbCategory.UpdateAsync(category);

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
        [Route("UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateCategory([FromBody] CategoryMasterDTO categoryMasterDTO)
        {
            try
            {
                if (categoryMasterDTO == null)
                {
                    return BadRequest();
                }

                int categoryId = categoryMasterDTO.CategoryId;

                if (await _dbCategory.GetAsync(u => u.CategoryId == categoryId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Category ID is Invalid!");
                    return BadRequest(ModelState);
                }

                if (await _dbCategory.GetAsync(u => u.CategoryName == categoryMasterDTO.CategoryName && u.CategoryId != categoryMasterDTO.CategoryId && u.IsDeleted == false) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                CategoryMaster model = _mapper.Map<CategoryMaster>(categoryMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _dbCategory.UpdateAsync(model);

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
        [Route("EnableCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnableCategory(int categoryId)
        {
            try
            {
                if (categoryId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var categoryMasterDTO = await _dbCategory.GetAsync(u => u.CategoryId == categoryId && u.IsDeleted == true);

                if (categoryMasterDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                CategoryMaster model = _mapper.Map<CategoryMaster>(categoryMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.IsDeleted = false;
                await _dbCategory.UpdateAsync(model);

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
