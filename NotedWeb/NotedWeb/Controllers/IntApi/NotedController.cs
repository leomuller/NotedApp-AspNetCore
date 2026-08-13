using Microsoft.AspNetCore.Mvc;
//using NotedData.NotedApp.Repositories;
using NotedWeb.Models.Notes;
using NotedWeb.Services.Auth;
using NotedWeb.AppCode.Config;

namespace NotedWeb.Controllers.IntApi
{
	[Route("intapi/[controller]")]
	[ApiController]
	public class NotedController : Controller
	{
		
		private readonly AuthService _authService;

		public NotedController(AuthService authService)
		{
			_authService = authService;
		}

		[HttpGet]
		[Route("list")]
		public async Task<IActionResult> GetNotes()
		{
			var noteRepository = new NotedData.NotedApp.Repositories.NoteRepository(MyProps.NoteDb);
			var session = await _authService.GetSessionAsync();

			if (session.IsLoggedIn == false)
			{
				return Unauthorized();
			}

			var notes = await noteRepository.GetByLoginIdAsync(session.LoginID);

			return Ok(notes);
		}

		[HttpGet]
		[Route("detail/{noteId:int}")]
		public async Task<IActionResult> GetNote(int noteId)
		{
			var noteRepository = new NotedData.NotedApp.Repositories.NoteRepository(MyProps.NoteDb);
			var session = await _authService.GetSessionAsync();

			if (session.IsLoggedIn == false)
			{
				return Unauthorized();
			}

			var note = await noteRepository.GetByIdAsync(noteId, session.LoginID);

			if (note == null)
			{
				return NotFound();
			}

			return Ok(note);
		}

		[HttpPost]
		[Route("create")]
		public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
		{
			var noteRepository = new NotedData.NotedApp.Repositories.NoteRepository(MyProps.NoteDb);
			var session = await _authService.GetSessionAsync();

			if (session.IsLoggedIn == false)
			{
				return Unauthorized();
			}

			var note = new NotedData.NotedApp.Models.Note
			{
				LoginId = session.LoginID,
				NoteTitle = request.NoteTitle,
				NoteText = request.NoteText,
				IsPinned = false
			};

			var noteId = await noteRepository.CreateAsync(note);

			return Ok(new { NoteId = noteId });
		}

		[HttpPut]
		[Route("update/{noteId:int}")]
		public async Task<IActionResult> UpdateNote(int noteId, [FromBody] UpdateNoteRequest request)
		{
			var noteRepository = new NotedData.NotedApp.Repositories.NoteRepository(MyProps.NoteDb);
			var session = await _authService.GetSessionAsync();

			if (session.IsLoggedIn == false)
			{
				return Unauthorized();
			}

			var existingNote = await noteRepository.GetByIdAsync(noteId, session.LoginID);

			if (existingNote == null)
			{
				return NotFound();
			}

			existingNote.NoteTitle = request.NoteTitle;
			existingNote.NoteText = request.NoteText;
			existingNote.IsPinned = request.IsPinned;

			await noteRepository.UpdateAsync(existingNote);

			return NoContent();
		}

		[HttpDelete]
		[Route("delete/{noteId:int}")]
		public async Task<IActionResult> DeleteNote(int noteId)
		{
			var noteRepository = new NotedData.NotedApp.Repositories.NoteRepository(MyProps.NoteDb);
			var session = await _authService.GetSessionAsync();

			if (session.IsLoggedIn == false)
			{
				return Unauthorized();
			}

			await noteRepository.SoftDeleteAsync(noteId, session.LoginID);

			return NoContent();
		}
	}
}
