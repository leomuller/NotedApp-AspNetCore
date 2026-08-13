using System;

namespace NotedData.NotedApp.Models
{
	public class NoteListItem
	{
		public int NoteId { get; set; }
		public int LoginId { get; set; }
		public string NoteTitle { get; set; } = string.Empty;
		public string NoteText { get; set; } = string.Empty;
		public bool IsPinned { get; set; }
		public DateTime DateUpdated { get; set; }
	}
}
