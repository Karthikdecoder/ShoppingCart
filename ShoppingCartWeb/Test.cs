//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using ShoppingCartWeb.Utililty;

//namespace ShoppingCartWeb
//{
//    public class Test
//    {
//        public async Task<IActionResult> CreateRoleMenu(RoleMenuVM obj, string selectedMenuData)
//        {
//            try
//            {
//                List<RoleMenuMapping> list = new();

//                List<CustomSelectListItem> selectedMenusList = JsonConvert.DeserializeObject<List<CustomSelectListItem>>(selectedMenuData);

//                var response = await _rolemenuService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

//                if (response != null && response.IsSuccess)
//                {
//                    list = JsonConvert.DeserializeObject<List<RoleMenuMapping>>(Convert.ToString(response.Result));

//                }
//                if (ModelState.IsValid)
//                {
//                    foreach (var selectedItem in selectedMenusList)
//                    {

//                        var matchingItem = list.FirstOrDefault( item => item.MenuId.ToString() == selectedItem.Value && item.RoleId == obj.menu.RoleId );

//                        if (matchingItem != null)
//                        {
//                            if (selectedItem.Selected && matchingItem.StatusFlag == true)
//                            {
//                                var update = await _rolemenuService.UpdateStatus<APIResponse>(matchingItem.Id, HttpContext.Session.GetString(SD.SessionToken));
//                            }
//                            else if (selectedItem.Selected == false && matchingItem.StatusFlag == false)
//                            {
//                                var delete = await _rolemenuService.DeleteAsync<APIResponse>(matchingItem.Id, HttpContext.Session.GetString(SD.SessionToken));
//                            }
//                        }

//                        else
//                        {
//                            if (selectedItem.Selected)
//                            {
//                                obj.menu.MenuId = int.Parse(selectedItem.Value);
//                                APIResponse create = await _rolemenuService.CreateAsync<APIResponse>(obj.menu, HttpContext.Session.GetString(SD.SessionToken));
//                            }
//                        }

//                    }

//                    TempData["success"] = "Updated successfully";
//                    return RedirectToAction(nameof(CreateRoleMenu));

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
