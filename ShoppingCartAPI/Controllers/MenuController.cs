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
        [Route("GetAllMenu")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllMenu()
        {
            try
            {
                IEnumerable<Menu> menuList = await _dbmenu.GetAllAsync( u => u.IsDeleted == false);
                _response.Result = _mapper.Map<List<MenuDTO>>(menuList);
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
        [Route("GetAllParentId")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllParentId()
        {
            try
            {
                IEnumerable<Menu> menuList = await _dbmenu.GetAllAsync(u => u.ParentId == 0);
                _response.Result = _mapper.Map<List<MenuDTO>>(menuList);
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
        [Route("GetMenu")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetMenu(int MenuId)
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
        [Route("CreateMenu")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateMenu([FromBody] MenuDTO MenuDTO)
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
        [Authorize]
        [Route("RemoveMenu")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> RemoveMenu(int MenuId)
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
        [Authorize]
        [Route("UpdateMenu")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateMenu([FromBody] MenuDTO MenuDTO)
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
        [Authorize]
        [Route("EnableMenu")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> EnableMenu(int MenuId)
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
