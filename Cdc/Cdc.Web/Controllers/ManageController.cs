using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Cdc.Web.Models;
using Cdc.Web.BLL;
using System.Collections.Generic;

namespace Cdc.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private CdcManagerService manager = new CdcManagerService();
        private CdcSystemService cdcSystem = new CdcSystemService();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public ActionResult Index(ManageMessageId? message)
        {

            var userId = User.Identity.GetUserId();
            string role = "none";
            List<string> roles =
                UserManager
                .GetRoles(userId).ToList();
            if (roles.Count > 0)
            {
                role = roles.Aggregate((s1, s2) => s1 + ";" + s2);
            }

            var model = new IndexViewModel
            {
                Email = User.Identity.Name,
                Role = role
            };
            if (roles.Contains("parent"))
            {
                Child child = cdcSystem.GetChild(User.Identity.Name);
                model.Balance = 0;
                model.BirthDate = child.BirthDate;
                model.Discount = child.Discount;
                model.FirstName = child.FirstName;
                model.LastName = child.LastName;
            }
            if (roles.Contains("teacher"))
            {
                Teacher teacher = cdcSystem.GetTeacher(User.Identity.Name);
                model.FirstName = teacher.FirstName;
                model.LastName = teacher.LastName;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult RegisterManager()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult RegisterManager(string email)
        {
            var user = UserManager.FindByEmail(email);
            if (user == null)
            {
                return new HttpNotFoundResult("user not found");
            }
            UserManager.AddToRole(user.Id, "manager");
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult RegisterChild()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult RegisterChild(RegisterChildViewModel childForm)
        {
            try
            {
                manager.RegisterChild(childForm);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult RegisterTeacher()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult RegisterTeacher(RegisterTeacherViewModel teacherForm)
        {
            try
            {
                manager.RegisterTeacher(teacherForm);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult AddSubject()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult AddSubject(AddSubjectViewModel subjectForm)
        {
            try
            {
                manager.AddSubject(subjectForm);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult AddLesson()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult AddLesson(AddLessonViewModel lessonForm)
        {
            try
            {
                manager.AddLesson(lessonForm);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult RecordChildForLesson()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult RecordChildForLesson(
            RecordChildForLessonViewModel form)
        {
            try
            {
                manager.RecordChildForLesson(form);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult SetDiscount()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult SetDiscount(SetDiscountViewModel discountForm)
        {
            try
            {
                manager.SetDiscount(discountForm.Email, discountForm.Discount);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult Statistics(StatisticsViewModel statistics)
        {
            return View(statistics);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        [ActionName("Statistics")]
        public ActionResult StatisticsPost(StatisticsViewModel statistics)
        {
            try
            {
                statistics.Income = manager.GetIncome(statistics.From, statistics.To);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Statistics", statistics);
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public ActionResult PublishNews()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult PublishNews(PublishNewsViewModel newsForm)
        {
            try
            {
                manager.PublishNews(newsForm.SubjectName,
                    newsForm.Content);
            }
            catch(Exception ex)
            {
                return new HttpNotFoundResult(ex.Message);
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}