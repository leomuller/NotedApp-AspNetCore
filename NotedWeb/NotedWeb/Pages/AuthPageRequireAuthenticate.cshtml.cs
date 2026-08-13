using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotedWeb.Services.Auth;

namespace NotedWeb.Pages
{
	public class AuthPageRequireAuthenticateModel : PageModel
	{
		private readonly AuthService _authManager;

		public AuthPageRequireAuthenticateModel(AuthService authManager)
		{
			_authManager = authManager;
		}

		public string msg { get; set; }

		public async Task OnGetAsync()
		{
			msg = "Auth check: <br />";
			msg += String.Format("Is Authenticated: {0} <br />", await _authManager.IsAuthenticatedAsync());
			msg += String.Format("Is Authorized: {0} <br />", await _authManager.IsAuthorizedAsync("my role"));
			msg += String.Format("Test message: {0} <br />", await _authManager.GetMessage());
		}
	}
}
