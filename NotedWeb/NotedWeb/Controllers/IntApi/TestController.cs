using Microsoft.AspNetCore.Mvc;
using NotedWeb.Services.Auth;

namespace NotedWeb.Controllers.IntApi
{

	[Route("intapi/[controller]")]
	[ApiController]
	public class TestController : Controller
	{
		private readonly AuthService _authService;

		public TestController(AuthService authService)
		{
			_authService = authService;
		}

		[HttpGet]
		[Route("first")]
		public IActionResult Index()
		{
			return Ok("test worked");
		}

		[HttpGet]
		[Route("getdata1")]
		public async Task<IActionResult> GetTestData1()
		{
			var repository = new NotedData.Auth.AuthApp.Repositories.AuthAppRepository(AppCode.Config.MyProps.NoteDb);

			var res = await repository.GetAllAsync();

			return Ok(res);
		}

		[HttpGet]
		[Route("testauthentication")]
		public async Task<IActionResult> TestAuthentication()
		{
			var session = await _authService.GetSessionAsync();

			return Ok(session);
		}


	}
}
