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
        private CdcParentService parent = new CdcParentService();
        private CdcTeacherService teacher = new CdcTeacherService();

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
                model.Balance = parent.GetBalance(User.Identity.Name);
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
            manager.RegisterChild(childForm);
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
            manager.RegisterTeacher(teacherForm);
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
            manager.AddSubject(subjectForm);
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
            manager.AddLesson(lessonForm);
            
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
            manager.RecordChildForLesson(form);
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
            manager.SetDiscount(discountForm.Email, discountForm.Discount);
            
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
            statistics.Income = manager.GetIncome(statistics.From, statistics.To);
            
            return RedirectToAction("Statistics", statistics);
        }

        [HttpGet]
        [Authorize(Roles = "manager,teacher")]
        public ActionResult PublishNews()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "manager,teacher")]
        public ActionResult PublishNews(PublishNewsViewModel newsForm)
        {
            manager.PublishNews(newsForm.SubjectName,
                    newsForm.Content);
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Authorize(Roles = "parent")]
        public ActionResult GetSchedule()
        {
            return View(parent.GetSchedule(User.Identity.Name));
        }

        [HttpGet]
        [Authorize(Roles = "parent")]
        public ActionResult MakePayment()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "parent")]
        public ActionResult MakePayment(decimal sum)
        {
            parent.MakePayment(User.Identity.Name, sum);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult TeacherSchedule()
        {
            return View(teacher.GetSchedule(User.Identity.Name));
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