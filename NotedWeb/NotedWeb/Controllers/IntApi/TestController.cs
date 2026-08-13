using Microsoft.AspNetCore.Mvc;

namespace NotedWeb.Controllers.IntApi
{

	[Route("intapi/[controller]")]
	[ApiController]
	public class TestController : Controller
	{
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
	}
}
