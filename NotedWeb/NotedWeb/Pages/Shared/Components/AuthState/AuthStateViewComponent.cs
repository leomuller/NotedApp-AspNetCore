using Microsoft.AspNetCore.Mvc;
using NotedWeb.Services.Auth;

namespace NotedWeb.Pages.Shared.Components.AuthState
{
	public class AuthStateViewModel
	{
		public bool IsLoggedIn { get; set; }
	}

	public class AuthStateViewComponent : ViewComponent
	{
		private readonly AuthService _authManager;

		public AuthStateViewComponent(AuthService authManager)
		{
			_authManager = authManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var session = await _authManager.GetSessionAsync();
			var viewModel = new AuthStateViewModel { IsLoggedIn = session.IsLoggedIn };
			return View(viewModel);
		}
	}
}
