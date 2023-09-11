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
    [Route("api/Menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IMenuRepository _dbmenu;
        private string _userId;

        public MenuController(IMenuRepository _menuRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbmenu = _menuRepository;
            _mapper = mapper;
            _response = new();
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        [Route("GetAllmenu")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllmenu()
        {
            try
            {
                IEnumerable<Menu> roleList = await _dbmenu.GetAllAsync();
                _response.Result = _mapper.Map<List<MenuDTO>>(roleList);
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
        [Route("Getmenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Getmenu(int MenuId)
        {
            try
            {
                if (MenuId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var Menu = await _dbmenu.GetAsync(u => u.MenuId == MenuId && u.IsDeleted == false);

                if (Menu == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<MenuDTO>(Menu);
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
        [Route("Createmenu")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Createmenu([FromBody] MenuDTO MenuDTO)
        {
            try
            {
                if (await _dbmenu.GetAsync(u => u.MenuName == MenuDTO.MenuName) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                if (MenuDTO == null)
                {
                    return BadRequest(MenuDTO);
                }

                Menu Menu = _mapper.Map<Menu>(MenuDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                Menu.CreatedOn = DateTime.Now;
                Menu.CreatedBy = int.Parse(_userId);
                Menu.UpdatedOn = DateTime.Now;
                Menu.UpdatedBy = int.Parse(_userId);
                Menu.IsDeleted = false;
                await _dbmenu.CreateAsync(Menu);

                _response.Result = _mapper.Map<MenuDTO>(Menu);
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
        [Route("Removemenu")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Removemenu(int MenuId)
        {
            try
            {
                if (MenuId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var menu = await _dbmenu.GetAsync(u => u.MenuId == MenuId && u.IsDeleted == false);

                if (menu == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                menu.IsDeleted = true;
                await _dbmenu.UpdateAsync(menu);

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
        [Route("Updatemenu")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Updatemenu([FromBody] MenuDTO MenuDTO)
        {
            try
            {
                if (MenuDTO == null)
                {
                    return BadRequest();
                }

                int MenuId = MenuDTO.MenuId;

                if (await _dbmenu.GetAsync(u => u.MenuId == MenuId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "menu ID is Invalid!");
                    return BadRequest(ModelState);
                }

                if (await _dbmenu.GetAsync(u => u.MenuName == MenuDTO.MenuName && u.MenuId != MenuDTO.MenuId) != null)
                {
                    _response.ResponseMessage = new List<string>() { "Already Exists" };
                    return BadRequest(_response);
                }

                Menu model = _mapper.Map<Menu>(MenuDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = int.Parse(_userId);
                model.IsDeleted = false;
                await _dbmenu.UpdateAsync(model);

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
        [Route("Enablemenu")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Enablemenu(int MenuId)
        {
            try
            {
                if (MenuId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var MenuDTO = await _dbmenu.GetAsync(u => u.MenuId == MenuId && u.IsDeleted == true);

                if (MenuDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                Menu model = _mapper.Map<Menu>(MenuDTO);

                if (_userId == null)
                {
                    _userId = "0";
                }

                model.IsDeleted = false;
                await _dbmenu.UpdateAsync(model);

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
