using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService,
                                   IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            //Add an instance of the ViewModel as a list (model)
            List<ManageUserRolesViewModel> model = new();

            //Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Get all Company Users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            //Loop over the users to populate the ViewModel
            foreach (BTUser user in users)
            {
                // - instantiate ViewModel
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;

                // - use _roleService
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);

                // - Creaet multi-selects
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                //add each ViewModel to list
                model.Add(viewModel);
            }

            //return the model to the view
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            //Get the company Id
            int companyId = User.Identity.GetCompanyId().Value;

            //Instantiate the BTUser
            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);

            //Get Roles for the User
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);

            //Grab the selected role
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (string.IsNullOrEmpty(userRole))
            {
                //remove user from their role
                if (await _rolesService.RemoveUserFromRolesAsync(btUser, roles))
                {
                    //add user to the new role
                    await _rolesService.AddUserToRoleAsync(btUser, userRole);
                }
            }

            //navigate back to the view
            return RedirectToAction(nameof(ManageUserRoles));
        }




    }
}
