using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Models;
using BugTracker.Extensions;
using BugTracker.Models.ViewModels;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTFileService _fileService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _companyInfoService;

        public ProjectsController(IBTRolesService rolesService,
                                  IBTLookupService lookupsService,
                                  IBTFileService fileService,
                                  IBTProjectService projectService,
                                  UserManager<BTUser> userManager,
                                  IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _lookupService = lookupsService;
            _fileService = fileService;
            _projectService = projectService;
            _userManager = userManager;
            _companyInfoService = companyInfoService;
        }
        //GET: Projects/My Projects
        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);

            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);
            return View(projects);
        }

        //GET: Projects/All Projects
        public async Task<IActionResult> AllProjects()
        {

            List<Project> projects = new();

            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            }

            return View(projects);
        }

        //GET: Projects/Archived Projects
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetArchivedProjectsByCompanyAsync(companyId);
            return View(projects);
        }

        //GET: Projects/Unassigned Projects
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = new();

            projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);
        }

        //GET: Projects/Assign Project Manager
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AssignPM(int projectId)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            AssignPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(projectId, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(Roles.ProjectManager), companyId), "Id", "FullName");

            return View(model);
        }

        //POST: Projects/Assign Project Manager
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);

                return RedirectToAction(nameof(Details), new { id = model.Project.Id });
            }
            return RedirectToAction(nameof(AssignPM), new { projectId = model.Project.Id });
        }

        //GET: Projects/Assing Members
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignMembers(int id)
        {
            ProjectMembersViewModel model = new();

            int companyId = User.Identity.GetCompanyId().Value;

            model.Project = await _projectService.GetProjectByIdAsync(id, companyId);
            List<BTUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Developer), companyId);
            List<BTUser> submitters = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Submitter), companyId);

            List<BTUser> companyMembers = developers.Concat(submitters).ToList();

            List<string> projectMembers = model.Project.Members.Select(m => m.Id).ToList();
            model.Users = new MultiSelectList(companyMembers, "Id", "FullName", projectMembers);

            return View(model);
        }

        //POST: Projects/Assign Members
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            if (model.SelectedUsers != null)
            {
                List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id)).Select(m => m.Id).ToList();

                //Remove current members
                foreach (string member in memberIds)
                {
                    await _projectService.RemoveUserFromProjectAsync(member, model.Project.Id);
                }

                //Add selected members
                foreach (string member in model.SelectedUsers)
                {
                    await _projectService.AddUserToProjectAsync(member, model.Project.Id);
                }

                //go to project details
                return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
            }
            return RedirectToAction(nameof(AssignMembers), new { id = model.Project.Id });
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;

            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            //Add ViewModel instance "AddProjectWithPMViewModel
            AddProjectWithPMViewModel model = new();

            //Load SelectLists with data ie. PMList & PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                int companyId = User.Identity.GetCompanyId().Value;

                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;

                    await _projectService.AddNewProjectAsync(model.Project);

                    //Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                }
                catch (Exception)
                {

                    throw;
                }

                //TODO: redirect to All Projects
                return RedirectToAction("Index");

            }

            return RedirectToAction("Create");
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            //Add ViewModel instance "AddProjectWithPMViewModel
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            //Load SelectLists with data ie. PMList & PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(model.Project);

                    //Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                }
                catch (Exception)
                {
                    if(!await ProjectExistsAsync(model.Project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");

            }
            return RedirectToAction("Edit");
        }

        // GET: Projects/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Restore/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProjectExistsAsync(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    }
}
