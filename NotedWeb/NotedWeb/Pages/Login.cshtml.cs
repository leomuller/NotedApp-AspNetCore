using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotedWeb.AppCode.Auth;
using NotedWeb.AppCode.Config;
using System.ComponentModel.DataAnnotations;

namespace NotedWeb.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; } = new LoginInput();

        [TempData]
        public string? ErrorMessage { get; set; }

        public string? SuccessMessage { get; set; }

        public bool ShowSuccess { get; set; } = false;

        public string ReturnUrl { get; set; } = "/";

        public void OnGet()
        {
            if (string.IsNullOrEmpty(ErrorMessage) == false)
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
			var clsLoginManager = new LoginManager();

			if (ModelState.IsValid == false)
            {
                return Page();
            }

			//reset message:
			ErrorMessage = "";

			//check login and password:
			var clsLoginVerifyResult = await clsLoginManager.VerifyPasswordForLoginText(MyProps.MstAppID, Input.LoginName, Input.Password);

			if (clsLoginVerifyResult.PasswordCorrect == true && clsLoginVerifyResult.LoginID.HasValue)
			{
				//login success, create session:
				var clsSessionManager = new SessionManager();
				string sessionCode = clsSessionManager.CreateEncryptedSessionCode(clsLoginVerifyResult.LoginID.Value, MyProps.MstAppID);
				await clsSessionManager.CreateSessionAsync(MyProps.MstAppID, clsLoginVerifyResult.LoginID.Value, sessionCode);

				//save last login:
				await clsLoginManager.UpdateLastLoginDateAsync(MyProps.MstAppID, clsLoginVerifyResult.LoginID.Value);

				//save login cookie:
				SetSessionCookie(sessionCode);

				SuccessMessage = "Welcome! You have successfully signed in.";
				ShowSuccess = true;

				return Page();
			}

				// if you got here, Login failed
			ErrorMessage = "Invalid login credentials..";
			return Page();

        }

		private void SetSessionCookie(string sessionCode)
		{
			var options = new CookieOptions
			{
				HttpOnly = true,  // Prevent client-side JavaScript from accessing the cookie
				Secure = true,    // Only send cookie over HTTPS (in production)
				SameSite = SameSiteMode.Strict,  // Prevent CSRF attacks
				Expires = DateTime.UtcNow.AddDays(1) // Set expiration date (optional)
			};

			// Set the cookie with the encrypted session code
			Response.Cookies.Append(MyProps.AuthCookieName, sessionCode, options);
		}
	}

    public class LoginInput
    {
        [Required]
		[DataType(DataType.Text)]
        public string LoginName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

       
    }
}
