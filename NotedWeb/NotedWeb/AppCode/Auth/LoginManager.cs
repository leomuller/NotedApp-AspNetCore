using mCore.DB;
using mCore.DB.ValueConverter;
using NotedWeb.AppCode.Config;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace NotedWeb.AppCode.Auth
{
	public class LoginManager
	{

		public LoginManager()
		{

		}

		//data model
		public class LoginDetail
		{
			public int LoginID { get; set; }
			public int AppID { get; set; }
			public string LoginText { get; set; } = "";
			public DateTime? LastLogin { get; set; }
			//public string AppName { get; set; }
			public string PasswordHash { get; set; } = "";

		}

		public class LoginVerifyResult
		{
			public int? LoginID { get; set; }
			public int? AppID { get; set; }
			public bool PasswordCorrect = false;
		}



		//private string HashPassword(string password)
		//{
		//	//password = _myProps.PwdSaltPrefix + password + _myProps.PwdSaltSuffix;
		//	return BCrypt.Net.BCrypt.HashPassword(password);
		//}

		//public async Task<bool> VerifyPasswordForLoginID(int appID, long loginID, string password)
		//{
		//	string passwordHash = "";

		//	// 1. Swapped named parameters for positional parameters ($1, $2)
		//	var sqlQuery = "SELECT password_hash FROM auth_credentials WHERE app_id = $1 AND login_id = $2;";

		//	var parameters = new NpgsqlParameter[]
		//	{
		//		/* $1 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = appID },
		//		/* $2 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Bigint,  Value = loginID }
		//	};

		//	// 2. Updated to PgSqlHelper
		//	using (var reader = await PgSqlHelper.ExecuteReaderAsync(MyProps.NoteDb, CommandType.Text, sqlQuery, parameters))
		//	{
		//		if (await reader.ReadAsync())
		//		{
		//			// 3. Cleanly extraction using your FromDB converter
		//			passwordHash = FromDB.StringValue(reader["password_hash"], true);
		//		}
		//	}

		//	return await VerifyPassword(password, passwordHash);
		//}


		public async Task<LoginVerifyResult> VerifyPasswordForLoginText(int appID, string loginText, string password)
		{
			var clsLoginResult = new LoginVerifyResult();
			string passwordHash = "";
			bool verifyResult = false;

			// 1. Swapped named parameters for positional parameters ($1, $2)
			var sqlQuery = "SELECT login_id, app_id, password_hash FROM auth_credentials WHERE app_id = $1 AND login_text = $2;";

			var parameters = new NpgsqlParameter[]
			{
				/* $1 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = appID },
				/* $2 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Text,    Value = ToDB.NullToDBNull(loginText) }
			};

			// 2. Updated to PgSqlHelper
			using (var reader = await PgSqlHelper.ExecuteReaderAsync(MyProps.NoteDb, CommandType.Text, sqlQuery, parameters))
			{
				if (await reader.ReadAsync())
				{
					// 3. Leveraged FromDB helpers to cleanly extract data and protect against strict casting bugs
					clsLoginResult.LoginID = FromDB.IntValue(reader["login_id"], true);
					clsLoginResult.AppID = FromDB.IntValue(reader["app_id"], true);
					passwordHash = FromDB.StringValue(reader["password_hash"], true);
				}
			}

			verifyResult = await VerifyPassword(password, passwordHash);

			if (verifyResult)
			{
				clsLoginResult.PasswordCorrect = true;
			}

			return clsLoginResult;
		}



		private async Task<bool> VerifyPassword(string enteredPassword, string storedHash)
		{
			try
			{
				// Try to verify the entered password against the stored hash
				return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
			}
			catch (ArgumentException ex)
			{
				// If an invalid hash or salt is provided, an ArgumentException will be thrown
				// Log the error or handle it as needed
				Console.WriteLine($"Error verifying password: {ex.Message}");
				return false;  // Return false because the hash is invalid
			}
			catch (Exception ex)
			{
				// Catch any other unexpected exceptions
				Console.WriteLine($"Unexpected error: {ex.Message}");
				return false;  // Return false in case of any unexpected exception
			}
			return false;

		}

		public async Task UpdateLastLoginDateAsync(int appID, long loginID)
		{
			var sqlQuery = @"
				UPDATE auth_credentials 
				SET last_login_date = $1 
				WHERE app_id = $2 AND login_id = $3;
			";

			var parameters = new NpgsqlParameter[]
			{
				/* $1 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.TimestampTz, Value = DateTime.UtcNow },
				/* $2 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer,     Value = appID },
				/* $3 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Bigint,      Value = loginID }
			};

			await PgSqlHelper.ExecuteNonQueryAsync(MyProps.NoteDb, CommandType.Text, sqlQuery, parameters);
		}


	}
}
