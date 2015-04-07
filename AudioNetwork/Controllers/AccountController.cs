using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AudioNetwork.Helpers;
using AudioNetwork.Services;
using DataLayer.Models;
using DataLayer.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using AudioNetwork.Models;


namespace AudioNetwork.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private IUserRepository _userRepository;
        //public AccountController()
        //{
        //}

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        //  [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if (user.EmailConfirmed == false)
                    {
                        return Json(new LoginResult { Success = false, Message = "Регистрация по почте не подтверждена" });
                    }
                    await SignInAsync(user, model.RememberMe);
                    return Json(new LoginResult { Success = true });
                    ///return new JsonResult(new {});
                    //  return RedirectToLocal(returnUrl);
                }
                else
                {
                    return Json(new LoginResult { Success = false, Message = "Неверный логин или пароль" });
                }
            }
            return Json(new LoginResult { Success = false, Message = ModelState.Values.ToString() });
        }

        [HttpPost]
        public ActionResult CheckLogin()
        {
            var a = Request;
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(User.Identity.Name) == false)
            {
                var userView = ModelConverters.ToUserViewModel(_userRepository.GetUser(User.Identity.GetUserId()));
                userView.LoggedIn = true;
                //var userView = new UserViewModel
                //{
                //    Id = User.Identity.GetUserId(),
                //    UserName = User.Identity.Name,
                //    LoggedIn = true
                //};

                return Json(userView);
            }

            return Json(new { LogedIn = false });
        }
        [HttpPost]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    LastActivity = DateTime.Now,
                    Email = model.Email,
                    AvatarFilePath = FilePathContainer.ImagePathRelative + "DefaultAvatar.png",
                    EmailConfirmed = false
                };
                try
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        User.Identity.GetUserId();
                        SendEmailMessage(user);

                        return Json(new LoginResult { Success = true, EmailSended = true });
                    }
                    return Json(new LoginResult { Success = false, Message = "Ошибка при регистрации" });
                }
                catch (Exception exp)
                {
                    return Json(new LoginResult { Success = false, Message = exp.Message });
                }
            }
            return Json(new LoginResult { Success = false, Message = ModelState.Values.ToString() });
        }

        private void SendEmailMessage(ApplicationUser user)
        {
            MailAddress from = new MailAddress("krasnovandr@mail.ru", "Web Registration");
            // кому отправляем
            MailAddress to = new MailAddress(user.Email);
            // создаем объект сообщения
            MailMessage mailMessage = new MailMessage(@from, to);
            // тема письма
            mailMessage.Subject = "Подтверждение регистрации по Email";
            // текст письма - включаем в него ссылку
            mailMessage.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
                                             "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                Request.Url.Authority + "#/Register?" + "Id=" + user.Id + "&email=" + user.Email);
            // Url.Action("ConfirmEmail", "Account", new { Token = user.Id, Email = user.Email }, Request.Url.Scheme));
            mailMessage.IsBodyHtml = true;
            // адрес smtp-сервера, с которого мы и будем отправлять письмо
            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.mail.ru", 25);
            // логин и пароль
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("krasnovandr@mail.ru", "3323876may1993");
            smtp.Send(mailMessage);
        }

        [AllowAnonymous]
        public string Confirm(string Email)
        {
            return "На почтовый адрес " + Email + " Вам высланы дальнейшие" +
                    "инструкции по завершению регистрации";
        }

        [AllowAnonymous]
        public async Task<JsonResult> ConfirmEmail(string Token, string Email)
        {
            ApplicationUser user = this.UserManager.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.EmailConfirmed = true;
                    await UserManager.UpdateAsync(user);
                    await SignInAsync(user, isPersistent: false);
                    return Json(new { Success = true });
                }
                return Json(new { Success = false, Message = "Почтовые адреса не совпадают" });
            }
            return Json(new { Success = false, Message = "Такой пользователь не найден" });
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<bool> Login(LoginViewModel model)
        //{
        //    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return true;
        //        default:
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //            return false;
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<bool> Register(RegisterViewModel model)
        //{
        //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //    var result = await UserManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded) return false;
        //    await SignInManager.SignInAsync(user, false, false);
        //    return true;
        //}

        //  //
        //  // POST: /Account/Login

        ////  [AllowAnonymous]
        ////  [ValidateAntiForgeryToken]


        //  //
        //  // GET: /Account/Register
        //  [AllowAnonymous]
        //  public ActionResult Register()
        //  {
        //      return View();
        //  }

        //  //
        // POST: /Account/Register

        //  //
        //  // POST: /Account/Disassociate
        //  [HttpPost]
        //  [ValidateAntiForgeryToken]
        //  public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //  {
        //      ManageMessageId? message = null;
        //      IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //      if (result.Succeeded)
        //      {
        //          message = ManageMessageId.RemoveLoginSuccess;
        //      }
        //      else
        //      {
        //          message = ManageMessageId.Error;
        //      }
        //      return RedirectToAction("Manage", new { Message = message });
        //  }

        //  //
        //  // GET: /Account/Manage
        //  public ActionResult Manage(ManageMessageId? message)
        //  {
        //      ViewBag.StatusMessage =
        //          message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //          : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //          : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //          : message == ManageMessageId.Error ? "An error has occurred."
        //          : "";
        //      ViewBag.HasLocalPassword = HasPassword();
        //      ViewBag.ReturnUrl = Url.Action("Manage");
        //      return View();
        //  }

        //  //
        //  // POST: /Account/Manage
        //  [HttpPost]
        //  [ValidateAntiForgeryToken]
        //  public async Task<ActionResult> Manage(ManageUserViewModel model)
        //  {
        //      bool hasPassword = HasPassword();
        //      ViewBag.HasLocalPassword = hasPassword;
        //      ViewBag.ReturnUrl = Url.Action("Manage");
        //      if (hasPassword)
        //      {
        //          if (ModelState.IsValid)
        //          {
        //              IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //              if (result.Succeeded)
        //              {
        //                  return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //              }
        //              else
        //              {
        //                  AddErrors(result);
        //              }
        //          }
        //      }
        //      else
        //      {
        //          // User does not have a password so remove any validation errors caused by a missing OldPassword field
        //          ModelState state = ModelState["OldPassword"];
        //          if (state != null)
        //          {
        //              state.Errors.Clear();
        //          }

        //          if (ModelState.IsValid)
        //          {
        //              IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        //              if (result.Succeeded)
        //              {
        //                  return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //              }
        //              else
        //              {
        //                  AddErrors(result);
        //              }
        //          }
        //      }

        //      // If we got this far, something failed, redisplay form
        //      return View(model);
        //  }

        //  //
        // POST: /Account/ExternalLogin
        //[HttpPost]
        ////[AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));

        //}



        //[ValidateAntiForgeryToken]
        public JsonResult GetLoginProviders()
        {
            var providers = HttpContext.GetOwinContext().Authentication.GetExternalAuthenticationTypes();
            return Json(providers, JsonRequestBehavior.AllowGet);

        }

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var user = await UserManager.FindAsync(loginInfo.Login);
        //    if (user != null)
        //    {
        //        await SignInAsync(user, isPersistent: false);
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then prompt the user to create an account
        //        ViewBag.ReturnUrl = returnUrl;
        //        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
        //    }
        //}

        //  //
        //  // POST: /Account/LinkLogin
        //  [HttpPost]
        //  [ValidateAntiForgeryToken]
        //  public ActionResult LinkLogin(string provider)
        //  {
        //      // Request a redirect to the external login provider to link a login for the current user
        //      return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        //  }

        //  //
        //  // GET: /Account/LinkLoginCallback
        //  public async Task<ActionResult> LinkLoginCallback()
        //  {
        //      var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //      if (loginInfo == null)
        //      {
        //          return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //      }
        //      var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //      if (result.Succeeded)
        //      {
        //          return RedirectToAction("Manage");
        //      }
        //      return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //  }

        //  //
        //  // POST: /Account/ExternalLoginConfirmation
        //  [HttpPost]
        //  [AllowAnonymous]
        //  [ValidateAntiForgeryToken]
        //  public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //  {
        //      if (User.Identity.IsAuthenticated)
        //      {
        //          return RedirectToAction("Manage");
        //      }

        //      if (ModelState.IsValid)
        //      {
        //          // Get the information about the user from the external login provider
        //          var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //          if (info == null)
        //          {
        //              return View("ExternalLoginFailure");
        //          }
        //          var user = new ApplicationUser() { UserName = model.UserName };
        //          var result = await UserManager.CreateAsync(user);
        //          if (result.Succeeded)
        //          {
        //              result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //              if (result.Succeeded)
        //              {
        //                  await SignInAsync(user, isPersistent: false);
        //                  return RedirectToLocal(returnUrl);
        //              }
        //          }
        //          AddErrors(result);
        //      }

        //      ViewBag.ReturnUrl = returnUrl;
        //      return View(model);
        //  }

        //  //
        //  // POST: /Account/LogOff
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return Json(true);
        }

        //
        // GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && UserManager != null)
        //    {
        //        UserManager.Dispose();
        //        UserManager = null;
        //    }
        //    base.Dispose(disposing);
        //}

        //  #region Helpers
        //  // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        //  private void AddErrors(IdentityResult result)
        //  {
        //      foreach (var error in result.Errors)
        //      {
        //          ModelState.AddModelError("", error);
        //      }
        //  }

        //  private bool HasPassword()
        //  {
        //      var user = UserManager.FindById(User.Identity.GetUserId());
        //      if (user != null)
        //      {
        //          return user.PasswordHash != null;
        //      }
        //      return false;
        //  }

        //  public enum ManageMessageId
        //  {
        //      ChangePasswordSuccess,
        //      SetPasswordSuccess,
        //      RemoveLoginSuccess,
        //      Error
        //  }

        //  private ActionResult RedirectToLocal(string returnUrl)
        //  {
        //      if (Url.IsLocalUrl(returnUrl))
        //      {
        //          return Redirect(returnUrl);
        //      }
        //      else
        //      {
        //          return RedirectToAction("Index", "Home");
        //      }
        //  }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        //#endregion
    }
}