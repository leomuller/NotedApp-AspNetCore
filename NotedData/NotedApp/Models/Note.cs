using System;
using System.Collections.Generic;
using System.Text;

namespace NotedData.NotedApp.Models
{
	public class Note
	{
		public int NoteId { get; set; }
		public int LoginId { get; set; }
		public string NoteTitle { get; set; } = string.Empty;
		public string NoteText { get; set; } = string.Empty;
		public bool IsPinned { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }
		public DateTime? DateDeleted { get; set; }
	}
}
