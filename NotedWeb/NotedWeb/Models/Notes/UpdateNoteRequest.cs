namespace NotedWeb.Models.Notes
{
	public class UpdateNoteRequest
	{
		public string NoteTitle { get; set; } = string.Empty;
		public string NoteText { get; set; } = string.Empty;
		public bool IsPinned { get; set; }
	}
}
