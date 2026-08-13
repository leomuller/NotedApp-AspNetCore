namespace NotedWeb.Models.Notes
{
	public class CreateNoteRequest
	{
		public string NoteTitle { get; set; } = string.Empty;
		public string NoteText { get; set; } = string.Empty;
	}
}
