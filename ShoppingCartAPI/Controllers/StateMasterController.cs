﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Data;
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
        private string _userId;

        public StateMasterController(IStateRepository stateRepo, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _stateRepo = stateRepo;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        [Route("GetAllState")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllState()
        {
            try
            {
                IEnumerable<StateMaster> stateList = await _stateRepo.GetAllAsync(includeProperties: "CountryMaster");
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

        [HttpGet]
        [Route("GetAllStateByCountryId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllStateByCountryId(int countryId)
        {
            try
            {
                if (countryId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                IEnumerable<StateMaster> stateList = await _stateRepo.GetAllAsync(u => u.CountryId == countryId && u.IsDeleted == false);

                if (stateList == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("CreateState")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateState([FromBody] StateMasterDTO stateMasterDTO)
        {
            try
            {

                if (await _stateRepo.GetAsync(u => u.StateName == stateMasterDTO.StateName && u.CountryId == stateMasterDTO.CountryId ) != null)
                {
                    //ModelState.AddModelError("ResponseMessage", "State already Exists!");
                    //return BadRequest(ModelState);

                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (stateMasterDTO == null)
                {
                    return BadRequest(stateMasterDTO);
                }

                StateMaster stateMaster = _mapper.Map<StateMaster>(stateMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                stateMaster.CountryMaster = null;
                stateMaster.CreatedOn = DateTime.Now;
                stateMaster.CreatedBy = int.Parse(_userId);
                stateMaster.UpdatedOn = DateTime.Now;
                stateMaster.UpdatedBy = int.Parse(_userId);
                stateMaster.IsDeleted = false;
                await _stateRepo.CreateAsync(stateMaster);

                _response.Result = _mapper.Map<StateMasterDTO>(stateMaster);
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
        [Route("RemoveState")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveState(int StateId)
        {
            try
            {
                if (StateId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var State = await _stateRepo.GetAsync(u => u.StateId == StateId && u.IsDeleted == false);

                if (State == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                State.IsDeleted = true;
                await _stateRepo.UpdateAsync(State);

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
        [Route("UpdateState")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateState([FromBody] StateMasterDTO stateMasterDTO)
        {
            try
            {
                if (stateMasterDTO == null)
                {
                    return BadRequest();
                }

                int StateId = stateMasterDTO.StateId;

                if (await _stateRepo.GetAsync(u => u.StateId == StateId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "State ID is Invalid!");
                    return BadRequest(ModelState);
                }

                if (await _stateRepo.GetAsync(u => u.StateName == stateMasterDTO.StateName && u.CountryId == stateMasterDTO.CountryId && u.StateId != stateMasterDTO.StateId ) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                StateMaster model = _mapper.Map<StateMaster>(stateMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _stateRepo.UpdateAsync(model);

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
        [Route("EnableState")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnableState(int stateId)
        {
            try
            {
                if (stateId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var stateMasterDTO = await _stateRepo.GetAsync(u => u.StateId == stateId && u.IsDeleted == true);

                if (stateMasterDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                StateMaster stateMaster = _mapper.Map<StateMaster>(stateMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }


                stateMaster.IsDeleted = false;
                await _stateRepo.UpdateAsync(stateMaster);

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
