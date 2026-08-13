using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NotedWeb.Services.Auth;

namespace NotedWeb.Pages
{
	public class AuthPageAnonymousModel : PageModel
	{
		private readonly AuthService _authManager;

		public AuthPageAnonymousModel(AuthService authManager)
		{
			_authManager = authManager;
		}

		public string msg { get; set; }

		public async Task OnGetAsync()
		{

			var session = await _authManager.GetSessionAsync();

			msg = "Auth check: <br />";
			msg += String.Format("Is Authenticated: {0} <br />", session.IsLoggedIn);
			msg += String.Format("Authentication Session: {0} <br />", session.SessionCode);
			msg += String.Format("Is Authorized: {0} <br />", await _authManager.IsAuthorizedAsync("my role"));	
			msg += String.Format("Test message: {0} <br />", await _authManager.GetMessage());
		}
	}
}
