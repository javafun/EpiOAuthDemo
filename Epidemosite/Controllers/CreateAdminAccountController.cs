using EPiServer.Personalization;
using EPiServer.Shell.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;

namespace Epidemosite.Controllers
{
    public class CreateAdminAccountController : Controller
    {
        private readonly UIUserProvider _uIUserProvider;
        private readonly UIRoleProvider _uIRoleProvider;

        public CreateAdminAccountController(UIUserProvider uIUserProvider, UIRoleProvider uIRoleProvider)
        {
            _uIUserProvider = uIUserProvider;
            _uIRoleProvider = uIRoleProvider;
        }
        // GET: CreateAdminAccount
        public ActionResult Index()
        {
            var admin = _uIUserProvider.GetUser("Admin");

            return View(admin);
        }

        public ActionResult CreateAdmin()
        {
            string storedRoleName = "WebAdmins";
            string userName = "Admin";
            string password = "Welcome1234$";
            string email = "vincent.yang@episerver.com";

            var admin = _uIUserProvider.GetUser(userName);

            if(admin == null)
            {
                UIUserCreateStatus status;

                IEnumerable<string> errors = Enumerable.Empty<string>();

                _uIUserProvider.CreateUser(userName, password, email, null, null, true, out status, out errors);

                _uIRoleProvider.CreateRole(storedRoleName);

                _uIRoleProvider.AddUserToRoles(userName, new string[] { storedRoleName });

                if (ProfileManager.Enabled)
                {
                    var profile = EPiServerProfile.Wrap(ProfileBase.Create(userName));

                    profile.Email = email;
                    profile.Save();
                }
            }

            return RedirectToAction("Index");
        }
    }
}