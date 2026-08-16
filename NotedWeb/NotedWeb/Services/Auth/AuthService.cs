using Microsoft.AspNetCore.Http;
using NotedWeb.AppCode.Auth;
using NotedWeb.AppCode.Config;

namespace NotedWeb.Services.Auth
{
	public class AuthService
	{

		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<bool> IsAuthenticatedAsync()
		{
			return false;
		}


		public async Task<bool> IsAuthorizedAsync(string requiredRole)
		{
			return false;
		}

		public async Task<string> GetMessage()
		{
			return _httpContextAccessor.HttpContext!.Request.Path;
		}


		public async Task<SessionManager.Session> GetSessionAsync()
		{
			//had as parameters HttpContext context

			var sessionCode = _httpContextAccessor.HttpContext.Request.Cookies[MyProps.AuthCookieName];
			var clsSession = new SessionManager.Session();

			if (string.IsNullOrEmpty(sessionCode) == false)
			{

				//tbd add try/catch, and if fails return empty.

				//check session:
				var clsSessionManager = new SessionManager();
				var CurSessionInfo = clsSessionManager.GetDecryptedSessionCode(sessionCode);

				//check session in DB:
				clsSession = await clsSessionManager.GetSessionBySessionCodeAsync(MyProps.MstAppID, sessionCode, 60, 1800);

			}

			return clsSession;

		}


		public async Task<String> LoginAsync(string login, string password)
		{
			var clsLoginManager = new LoginManager();
			string sessionCode = string.Empty;


			//check login and password:
			var clsLoginVerifyResult = await clsLoginManager.VerifyPasswordForLoginText(MyProps.MstAppID, login, password);

			if (clsLoginVerifyResult.PasswordCorrect == true && clsLoginVerifyResult.LoginID.HasValue)
			{
				//login success, create session:
				var clsSessionManager = new SessionManager();
				sessionCode = clsSessionManager.CreateEncryptedSessionCode(clsLoginVerifyResult.LoginID.Value, MyProps.MstAppID);
				await clsSessionManager.CreateSessionAsync(MyProps.MstAppID, clsLoginVerifyResult.LoginID.Value, sessionCode);

				//save last login:
				await clsLoginManager.UpdateLastLoginDateAsync(MyProps.MstAppID, clsLoginVerifyResult.LoginID.Value);
			}

			return sessionCode;
		}

		



	}	
}
