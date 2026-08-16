using Microsoft.AspNetCore.Mvc;
using NotedWeb.AppCode.Config;
using NotedWeb.Services.Auth;

namespace NotedWeb.Controllers.IntApi
{
	[Route("intapi/[controller]")]
	[ApiController]
	public class AuthController : Controller
	{
		private readonly AuthService _authService;

		public AuthController(AuthService authService)
		{
			_authService = authService;
		}

		// GET : intapi/auth/checksession
		//test if the user is logged in and return the session info
		[HttpGet]
		[Route("checksession")]
		public async Task<IActionResult> CheckSession()
		{
			var session = await _authService.GetSessionAsync();

			//copy to DTO:
			var sessionDto = new NotedWeb.Models.Auth.SessionResponse
			{
				SessionID = session.SessionID,
				LoginID = session.LoginID,
				SessionCode = session.SessionCode,
				DateUpdated = session.DateUpdated,
				IsLoggedIn = session.IsLoggedIn
			};

			return Ok(sessionDto);
		}

		// POST: intapi/auth/login
		//login with username and password, return session info if successful
		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] NotedWeb.Models.Auth.LoginRequest req)
		{
			var clsLoginResponse = new Models.Auth.LoginResponse();

			var sessionCode = await _authService.LoginAsync(req.Login, req.Password);
			if (sessionCode == string.Empty)
			{
				return Unauthorized();
			}

			//save login cookie:
			SetSessionCookie(sessionCode);

			clsLoginResponse.AuthCookieName = MyProps.AuthCookieName;
			clsLoginResponse.SessionCode = sessionCode;

			return Ok(clsLoginResponse);
		}

		private void SetSessionCookie(string sessionCode)
		{
			var options = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddDays(1)    //real expiration is in business logic. 
			};

			// Set the cookie with the encrypted session code
			Response.Cookies.Append(MyProps.AuthCookieName, sessionCode, options);
		}




	}
}
