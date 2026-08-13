using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotedWeb.AppCode.Auth;
using NotedWeb.AppCode.Config;

namespace NotedWeb.Pages
{
    public class LogoutModel : PageModel
    {
		public async Task<IActionResult> OnGetAsync()
		{
			var clsSessionManager = new SessionManager();
			var sessionCode = Request.Cookies[MyProps.AuthCookieName];

			//delete session from DB
			if (string.IsNullOrEmpty(sessionCode) == false)
			{
				await clsSessionManager.DeleteSessionAsync(MyProps.MstAppID, sessionCode);
			}

			//remove cookie
			Response.Cookies.Delete(MyProps.AuthCookieName);

			return Page();
		}
	}
}
