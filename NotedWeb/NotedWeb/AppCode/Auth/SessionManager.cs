using NotedWeb.AppCode.Config;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace NotedWeb.AppCode.Auth
{
	public class SessionManager
	{

		public SessionManager()
		{
		}

		public class Session
		{
			public long SessionID { get; set; }
			public int LoginID { get; set; }
			public int AppID { get; set; }
			public string SessionCode { get; set; } = "";
			public DateTime DateCreated { get; set; }
			public DateTime DateUpdated { get; set; }
			public bool IsLoggedIn { get; set; } = false;
		}


		public class SessionCodeInfo
		{
			public long SessionID { get; set; }
			public int LoginID { get; set; }
			public int AppID { get; set; }
			public string Guid { get; set; }
			public string Timestamp { get; set; }
			public string DecryptedSessionCode { get; set; }
		}


		public async Task<Session> GetSessionBySessionCodeAsync(int appID, string sessionCode, int renewTimeSeconds, int maxAgeSeconds)
		{
			var curSession = new Session();

			// 1. Converted named parameters to positional ($1, $2)
			var sqlQuery = @"
				SELECT session_id, login_id, app_id, session_code, date_created, date_updated 
				FROM sec_sessions 
				WHERE app_id = $1 AND session_code = $2;
			";

			var parameters = new NpgsqlParameter[]
			{
				/* $1 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = appID },
				/* $2 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Text,    Value = mCore.DB.ValueConverter.ToDB.NullToDBNull(sessionCode) }
			};

			try
			{
				using (var reader = await mCore.DB.PgSqlHelper.ExecuteReaderAsync(MyProps.NoteDb, CommandType.Text, sqlQuery, parameters))
				{
					if (await reader.ReadAsync())
					{
						curSession.SessionID = Convert.ToInt64(reader["session_id"]);
						curSession.LoginID = mCore.DB.ValueConverter.FromDB.IntValue(reader["login_id"], true);
						curSession.AppID = mCore.DB.ValueConverter.FromDB.IntValue(reader["app_id"], true);
						curSession.SessionCode = mCore.DB.ValueConverter.FromDB.StringValue(reader["session_code"], true);
						curSession.DateCreated = mCore.DB.ValueConverter.FromDB.DateTimeValue(reader["date_created"], true);
						curSession.DateUpdated = mCore.DB.ValueConverter.FromDB.DateTimeValue(reader["date_updated"], true);
						curSession.IsLoggedIn = true;

						DateTime currentDateTime = DateTime.UtcNow;

						if ((currentDateTime - curSession.DateUpdated).TotalSeconds > maxAgeSeconds)
						{
							curSession = new Session(); // Session expired
						}
						else if ((currentDateTime - curSession.DateUpdated).TotalSeconds > renewTimeSeconds)
						{
							await UpdateSessionTimeAsync(appID, sessionCode);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Log exception if required
			}

			return curSession;
		}

		public async Task UpdateSessionTimeAsync(int appID, string sessionCode)
		{
			// 4. Fixed the time mismatch by passing DateTime.UtcNow as parameter $1
			var sqlQuery = @"
				UPDATE sec_sessions 
				SET date_updated = $1 
				WHERE app_id = $2 AND session_code = $3;
			";

			var parameters = new NpgsqlParameter[]
			{
				/* $1 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.TimestampTz, Value = DateTime.UtcNow },
				/* $2 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer,     Value = appID },
				/* $3 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Text,        Value = mCore.DB.ValueConverter.ToDB.NullToDBNull(sessionCode) }
			};

			await mCore.DB.PgSqlHelper.ExecuteNonQueryAsync(MyProps.NoteDb, CommandType.Text, sqlQuery, parameters);
		}

		public async Task CreateSessionAsync(int appID, int loginID, string sessionCode)
		{
			// 5. Replaced SCOPE_IDENTITY() with RETURNING session_id
			// 6. Passed a single UTC timestamp variable from C# to populate both date fields
			var sqlQuery = @"
				INSERT INTO sec_sessions 
				(login_id, app_id, session_code, date_created, date_updated) 
				VALUES 
				($1, $2, $3, $4, $4)
				RETURNING session_id;
			";

			var utcNow = DateTime.UtcNow;

			var parameters = new NpgsqlParameter[]
			{
				/* $1 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer,     Value = loginID },
				/* $2 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer,     Value = appID },
				/* $3 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Text,        Value = mCore.DB.ValueConverter.ToDB.NullToDBNull(sessionCode) },
				/* $4 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.TimestampTz, Value = utcNow }
			};

			object? retVal = await mCore.DB.PgSqlHelper.ExecuteScalarAsync(MyProps.NoteDb, CommandType.Text, sqlQuery, parameters);

			if (retVal != null && retVal != DBNull.Value)
			{
				long newSessionId = Convert.ToInt64(retVal);
				//_logger.LogInformation("Created session, ID: {SessionId}", newSessionId);
			}
		}

		public async Task DeleteSessionAsync(int appID, string sessionCode)
		{
			var sqlQuery = @"
				DELETE FROM sec_sessions 
				WHERE app_id = $1 AND session_code = $2;
			";

			var parameters = new NpgsqlParameter[]
			{
				/* $1 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = appID },
				/* $2 */ new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Text,    Value = mCore.DB.ValueConverter.ToDB.NullToDBNull(sessionCode) }
			};

			await mCore.DB.PgSqlHelper.ExecuteNonQueryAsync(MyProps.NoteDb, CommandType.Text, sqlQuery, parameters);
		}

		// 7. Pure C# Logic — Left completely untouched since it has no database dependencies!
		public string CreateEncryptedSessionCode(int loginId, int appId)
		{
			string guid = Guid.NewGuid().ToString();
			string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ff");

			string rawSessionData = $"{loginId}_{appId}_{timestamp}_{guid}";
			string encryptedSessionCode = Encrypt(rawSessionData);

			return encryptedSessionCode;
		}

		public SessionCodeInfo GetDecryptedSessionCode(string encryptedSessionCode)
		{
			var clsSessionCodeInfo = new SessionCodeInfo();

			try
			{
				// Decrypt the session code
				string decryptedData = Decrypt(encryptedSessionCode);
				clsSessionCodeInfo.DecryptedSessionCode = decryptedData;

				//for debug / additional data:
				var parts = decryptedData.Split('_');
				if (parts.Length >= 4)
				{
					// Parse the components
					clsSessionCodeInfo.LoginID = int.Parse(parts[0]);
					clsSessionCodeInfo.AppID = int.Parse(parts[1]);
					clsSessionCodeInfo.Timestamp = parts[2];
					clsSessionCodeInfo.Guid = parts[3];

					return clsSessionCodeInfo;
				}

			}
			catch (Exception ex)
			{
				//log error

				//delete session?

			}

			throw new Exception("Invalid session data.");
		}

		private string Encrypt(string rawData)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(MyProps.SessionEncryptionKey); // Key should be 16, 24, or 32 bytes (128/192/256 bit key)
																	 //aesAlg.IV = new byte[16]; // Initialization Vector (use a random IV in a real-world case)
				aesAlg.IV = Encoding.UTF8.GetBytes("InitVectorAcuAqz");

				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				byte[] rawDataBytes = Encoding.UTF8.GetBytes(rawData);
				byte[] encryptedDataBytes = encryptor.TransformFinalBlock(rawDataBytes, 0, rawDataBytes.Length);

				// Return the base64-encoded encrypted session code
				return Convert.ToBase64String(encryptedDataBytes);
			}
		}



		private string Decrypt(string encryptedData)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(MyProps.SessionEncryptionKey);
				//aesAlg.IV = new byte[16]; // Should use the same IV used during encryption
				aesAlg.IV = Encoding.UTF8.GetBytes("InitVectorAcuAqz");

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				byte[] encryptedDataBytes = Convert.FromBase64String(encryptedData);
				byte[] decryptedDataBytes = decryptor.TransformFinalBlock(encryptedDataBytes, 0, encryptedDataBytes.Length);

				return Encoding.UTF8.GetString(decryptedDataBytes);
			}
		}


	}
}
