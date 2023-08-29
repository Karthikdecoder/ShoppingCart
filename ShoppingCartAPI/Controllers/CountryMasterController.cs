﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Repository.IRepository;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/CountryMaster")]
    [ApiController]
    public class CountryMasterController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepo;
        private string _userId;

        public CountryMasterController(ICountryRepository countryRepo, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _countryRepo = countryRepo;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        [Route("GetAllCountry")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllCountry()
        {
            try
            {
                IEnumerable<CountryMaster> countryList = await _countryRepo.GetAllAsync(u => u.IsDeleted == false);
                _response.Result = _mapper.Map<List<CountryMasterDTO>>(countryList);
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
        [Route("GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCountry(int countryId)
        {
            try
            {
                if (countryId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var country = await _countryRepo.GetAsync(u => u.CountryId == countryId && u.IsDeleted == false);

                if (country == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<CountryMasterDTO>(country);
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
        [Route("CreateCountry")]
        //[Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateCountry([FromBody] CountryMasterDTO countryMasterDTO)
        {
            try
            {
                if (await _countryRepo.GetAsync(u => u.CountryName == countryMasterDTO.CountryName && u.IsDeleted == false) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Country already exists!");
                    return BadRequest(ModelState);
                }

                if (countryMasterDTO == null)
                {
                    return BadRequest(countryMasterDTO);
                }

                CountryMaster countryMaster = _mapper.Map<CountryMaster>(countryMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                countryMaster.CreatedOn = DateTime.Now;
                countryMaster.CreatedBy = int.Parse(_userId);
                countryMaster.UpdatedOn = DateTime.Now;
                countryMaster.UpdatedBy = int.Parse(_userId);
                countryMaster.IsDeleted = false;
                await _countryRepo.CreateAsync(countryMaster);

                _response.Result = _mapper.Map<CountryMasterDTO>(countryMaster);
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

        ////[Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("RemoveCountry")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveCountry(int CountryId)
        {
            try
            {
                if (CountryId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var Country = await _countryRepo.GetAsync(u => u.CountryId == CountryId && u.IsDeleted == false);

                if (Country == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                Country.IsDeleted = true;
                await _countryRepo.UpdateAsync(Country);

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
        [Route("UpdateCountry")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateCountry([FromBody] CountryMasterDTO CountryMasterDTO)
        {
            try
            {
                if (CountryMasterDTO == null)
                {
                    return BadRequest();
                }

                int CountryId = CountryMasterDTO.CountryId;

                if (await _countryRepo.GetAsync(u => u.CountryId == CountryId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Country ID is Invalid!");
                    return BadRequest(ModelState);
                }

                CountryMaster model = _mapper.Map<CountryMaster>(CountryMasterDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _countryRepo.UpdateAsync(model);

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