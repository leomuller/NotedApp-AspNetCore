using System;
using System.Collections.Generic;
using System.Text;

namespace NotedData.Auth.AuthApp.Models
{
	public class AuthApp
	{
		public int AppId { get; set; }
		public string AppName { get; set; } = string.Empty;
		public string AppDescription { get; set; } = string.Empty;
	}
}
