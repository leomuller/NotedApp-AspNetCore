namespace NotedWeb.Models.Auth
{
	//DTO
	public class SessionResponse
	{
		public long SessionID { get; set; }
		public int LoginID { get; set; }
		public string SessionCode { get; set; } = "";
		public DateTime DateUpdated { get; set; }
		public bool IsLoggedIn { get; set; } = false;
	}
}
