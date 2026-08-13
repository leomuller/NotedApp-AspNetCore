namespace NotedWeb.AppCode.Config
{
	public static class MyProps
	{
		private static IConfiguration _config;

		public static void Initialize(IConfiguration config)
		{
			_config = config;
		}

		public static string NoteDb
		{
			get
			{
				return _config.GetConnectionString("NoteDb") ?? string.Empty;
			}
		}

		public static string AppVersion
		{
			get
			{
				return _config["AppSettings:AppVersion"] ?? string.Empty;
			}
		}

		public static int MstAppID
		{
			get
			{
				string val = _config["AppSettings:MstAppID"];
				return Convert.ToInt32(val);
			}
		}

		public static string AuthCookieName
		{
			get
			{
				return _config["AppSettings:AuthCookieName"];
			}
		}

		public static string SessionEncryptionKey
		{
			get
			{
				return _config["AppSettings:SessionEncryptionKey"];
			}
		}
	}
}
