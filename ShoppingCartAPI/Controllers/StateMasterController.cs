﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository.IRepository;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/StateMaster")]
    [ApiController]
    public class StateMasterController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IStateRepository _stateRepo;
        private string _userID;

        public StateMasterController(IStateRepository stateRepo, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _stateRepo = stateRepo;
            _mapper = mapper;
            _response = new();
            _userID = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber);
        }

        [HttpGet]
        [Route("GetAllState")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllState()
        {
            try
            {
                IEnumerable<StateMaster> stateList = await _stateRepo.GetAllAsync(u => u.IsDeleted == false);
                _response.Result = _mapper.Map<List<StateMasterDTO>>(stateList);
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
        [Route("GetState")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetState(int stateId)
        {
            try
            {
                if (stateId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var state = await _stateRepo.GetAsync(u => u.StateId == stateId && u.IsDeleted == false);

                if (state == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<StateMasterDTO>(state);
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

        //[HttpPost]
        //[Route("CreateCategory")]
        ////[Authorize(Roles = "admin")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CategoryMasterDTO categoryMasterDTO)
        //{
        //    try
        //    {
        //        if (await _dbCategory.GetAsync(u => u.CategoryName == categoryMasterDTO.CategoryName && u.IsDeleted == false) != null)
        //        {
        //            ModelState.AddModelError("ErrorMessages", "Category already exists!");
        //            return BadRequest(ModelState);
        //        }

        //        if (categoryMasterDTO == null)
        //        {
        //            return BadRequest(categoryMasterDTO);
        //        }

        //        CategoryMaster categoryMaster = _mapper.Map<CategoryMaster>(categoryMasterDTO);

        //        categoryMaster.CreatedOn = DateTime.Now;
        //        categoryMaster.UpdatedOn = DateTime.Now;
        //        categoryMaster.IsDeleted = false;
        //        await _dbCategory.CreateAsync(categoryMaster);

        //        _response.Result = _mapper.Map<CategoryMasterDTO>(categoryMaster);
        //        _response.StatusCode = HttpStatusCode.Created;

        //        return Ok();
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
        //[Route("RemoveCategory")]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<APIResponse>> RemoveCategory(int categoryId)
        //{
        //    try
        //    {
        //        if (categoryId == 0)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(_response);
        //        }

        //        var category = await _dbCategory.GetAsync(u => u.CategoryId == categoryId && u.IsDeleted == false);

        //        if (category == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(_response);
        //        }

        //        category.IsDeleted = true;
        //        await _dbCategory.UpdateAsync(category);

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
        //[Route("UpdateCategory")]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<APIResponse>> UpdateCategory([FromBody] CategoryMasterDTO categoryMasterDTO)
        //{
        //    try
        //    {
        //        if (categoryMasterDTO == null)
        //        {
        //            return BadRequest();
        //        }

        //        int categoryId = categoryMasterDTO.CategoryId;

        //        if (await _dbCategory.GetAsync(u => u.CategoryId == categoryId) == null)
        //        {
        //            ModelState.AddModelError("ErrorMessages", "Category ID is Invalid!");
        //            return BadRequest(ModelState);
        //        }

        //        CategoryMaster model = _mapper.Map<CategoryMaster>(categoryMasterDTO);

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
