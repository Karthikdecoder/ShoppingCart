//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using ShoppingCartWeb.Utililty;

//namespace ShoppingCartWeb
//{
//    public class Test2
//    {
//        [Authorize]
//        [HttpGet]
//        public async Task<IActionResult> CreateModuleMapping()
//        {

//            ModuleMappinVM roleMenu = new();

//            var response = await _roleService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SeesionToken));
//            if (response != null)
//            {
//                roleMenu.RoleList = JsonConvert.DeserializeObject<List<RoleDetails>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
//                {
//                    Text = i.RoleName,
//                    Value = i.RoleId.ToString(),
//                });
//            }


//            return View(roleMenu);
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetMenusByRole(int roleId)
//        {
//            var result = await _moduleRoleMappingService.GetMenuAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SeesionToken));

//            List<ModuleDTO> menus = JsonConvert.DeserializeObject<List<ModuleDTO>>(Convert.ToString(result.Result));

//            ModuleMappinVM roleMenu = new();
//            // Create a list of CustomSelectListItem as shown in the previous answer

//            var menu = await _moduleService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SeesionToken));

//            if (menu != null)
//            {
//                roleMenu.MenuList = JsonConvert.DeserializeObject<List<ModuleDTO>>(Convert.ToString(menu.Result)).Select(i => new CustomSelectedItem
//                {
//                    Text = i.Menus,
//                    Value = i.ModuleId.ToString(),
//                    Selected = false,
//                    ParentId = i.ParentId ?? 0,
//                    ParentName = i.ParentName

//                }).ToList();
//                foreach (var item in roleMenu.MenuList)
//                {
//                    foreach (var men in menus)
//                    {
//                        if (men.ModuleId.ToString() == item.Value)
//                        {
//                            item.Selected = true;
//                        }
//                    }
//                }
//            }

//            List<CustomSelectedItem> roleMenulist = new();

//            foreach (var item in roleMenu.MenuList)
//            {
//                if (item.ParentId == 0)
//                {
//                    roleMenulist.Add(item);
//                }
//            }

//            foreach (var item in roleMenu.MenuList)
//            {
//                if (item.ParentId == 0)
//                {
//                    foreach (var subitem in roleMenu.MenuList)
//                    {
//                        if (subitem.ParentId.ToString() == item.Value)
//                        {
//                            roleMenulist.Add(subitem);
//                        }

//                    }
//                }
//            }

//            return Json(roleMenulist); ; // Return the menus as JSON
//        }


//        [Authorize]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> CreateModuleMapping(ModuleMappinVM obj, string selectedMenuData)
//        {
//            try
//            {
//                List<ModuleRoleMappingDTO> list = new();
//                List<CustomSelectedItem> selectedMenusList = JsonConvert.DeserializeObject<List<CustomSelectedItem>>(selectedMenuData);
//                var response = await _moduleRoleMappingService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SeesionToken));
//                if (response != null && response.IsSuccess)
//                {
//                    list = JsonConvert.DeserializeObject<List<ModuleRoleMappingDTO>>(Convert.ToString(response.Result));

//                }
//                if (ModelState.IsValid)
//                {
//                    foreach (var selectedItem in selectedMenusList)
//                    {

//                        var matchingItem = list.FirstOrDefault(item =>
//                            item.ModuleId.ToString() == selectedItem.Value &&
//                            item.RoleId == obj.ModuleMapping.RoleId
//                        );
//                        if (matchingItem != null)
//                        {
//                            if (selectedItem.Selected && matchingItem.StatusFlag == true)
//                            {
//                                var update = await _moduleRoleMappingService.RecoverAsync<APIResponse>(matchingItem.RoleMapId, HttpContext.Session.GetString(SD.SeesionToken));
//                            }
//                            else if (selectedItem.Selected == false && matchingItem.StatusFlag == false)
//                            {
//                                var delete = await _moduleRoleMappingService.DeleteAsync<APIResponse>(matchingItem.RoleMapId, HttpContext.Session.GetString(SD.SeesionToken));
//                            }
//                        }
//                        else
//                        {
//                            if (selectedItem.Selected)
//                            {
//                                obj.ModuleMapping.ModuleId = int.Parse(selectedItem.Value);
//                                APIResponse create = await _moduleRoleMappingService.CreateAsync<APIResponse>(obj.ModuleMapping, HttpContext.Session.GetString(SD.SeesionToken));
//                            }
//                        }
//                    }

//                    TempData["success"] = "Updated successfully";
//                    return RedirectToAction(nameof(CreateModuleMapping));

//                }
//            }
//            catch (Exception e)
//            {
//                TempData["error"] = e.Message;
//            }
//            TempData["error"] = "Error encountered";
//            return View(obj);
//        }



//    }
//}
