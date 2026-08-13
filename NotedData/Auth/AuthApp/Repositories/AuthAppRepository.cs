using Dapper;
using NotedData.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotedData.Auth.AuthApp.Repositories
{
	public class AuthAppRepository : DapperContext, IAuthAppRepository
	{
		public AuthAppRepository(string dbConString) : base(dbConString)
		{
		}

		public async Task<IEnumerable<Models.AuthApp>> GetAllAsync()
		{
			const string sql = """
                SELECT
                    app_id AS AppId,
                    app_name AS AppName,
                    app_description AS AppDescription
                FROM auth_apps
                ORDER BY app_id;
                """;

			await using var connection = await DataSource.OpenConnectionAsync();

			return await connection.QueryAsync<Models.AuthApp>(sql);
		}

		public async Task<Models.AuthApp?> GetByIdAsync(int appId)
		{
			const string sql = """
				SELECT
					app_id AS AppId,
					app_name AS AppName,
					app_description AS AppDescription
				FROM auth_apps
				WHERE app_id = @AppId;
				""";

			await using var connection = await DataSource.OpenConnectionAsync();


			return await connection.QuerySingleOrDefaultAsync<Models.AuthApp>(sql, new {AppId = appId});

		}
	}
}
