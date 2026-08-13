using Dapper;
using NotedData.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotedData.NotedApp.Repositories
{
	public class NoteRepository : DapperContext, INoteRepository
	{
		public NoteRepository(string dbConString) : base(dbConString)
		{
		}

		public async Task<IEnumerable<Models.Note>> GetByLoginIdAsync(int loginId)
		{
			const string sql = """
				SELECT
					note_id AS NoteId,
					login_id AS LoginId,
					note_title AS NoteTitle,
					note_text AS NoteText,
					is_pinned AS IsPinned,
					is_deleted AS IsDeleted,
					date_created AS DateCreated,
					date_updated AS DateUpdated,
					date_deleted AS DateDeleted
				FROM ntd_notes
				WHERE login_id = @LoginId
					AND is_deleted = false
				ORDER BY is_pinned DESC, date_updated DESC;
				""";

			await using var connection = await DataSource.OpenConnectionAsync();

			return await connection.QueryAsync<Models.Note>(sql, new { LoginId = loginId });
		}

		public async Task<Models.Note?> GetByIdAsync(int noteId, int loginId)
		{
			const string sql = """
				SELECT
					note_id AS NoteId,
					login_id AS LoginId,
					note_title AS NoteTitle,
					note_text AS NoteText,
					is_pinned AS IsPinned,
					is_deleted AS IsDeleted,
					date_created AS DateCreated,
					date_updated AS DateUpdated,
					date_deleted AS DateDeleted
				FROM ntd_notes
				WHERE note_id = @NoteId
					AND login_id = @LoginId;
				""";

			await using var connection = await DataSource.OpenConnectionAsync();

			return await connection.QuerySingleOrDefaultAsync<Models.Note>(sql, new { NoteId = noteId, LoginId = loginId });
		}

		public async Task<int> CreateAsync(Models.Note note)
		{
			const string sql = """
				INSERT INTO ntd_notes
				(
					login_id,
					note_title,
					note_text,
					is_pinned,
					is_deleted,
					date_created,
					date_updated,
					date_deleted
				)
				VALUES
				(
					@LoginId,
					@NoteTitle,
					@NoteText,
					@IsPinned,
					false,
					@DateCreated,
					@DateUpdated,
					NULL
				)
				RETURNING note_id;
				""";

			var utcNow = DateTime.UtcNow;

			await using var connection = await DataSource.OpenConnectionAsync();

			return await connection.QuerySingleAsync<int>(sql, new
			{
				note.LoginId,
				note.NoteTitle,
				note.NoteText,
				note.IsPinned,
				DateCreated = utcNow,
				DateUpdated = utcNow
			});
		}

		public async Task UpdateAsync(Models.Note note)
		{
			const string sql = """
				UPDATE ntd_notes
				SET
					note_title = @NoteTitle,
					note_text = @NoteText,
					is_pinned = @IsPinned,
					date_updated = @DateUpdated
				WHERE note_id = @NoteId
					AND login_id = @LoginId;
				""";

			await using var connection = await DataSource.OpenConnectionAsync();

			await connection.ExecuteAsync(sql, new
			{
				note.NoteId,
				note.LoginId,
				note.NoteTitle,
				note.NoteText,
				note.IsPinned,
				DateUpdated = DateTime.UtcNow
			});
		}

		public async Task SoftDeleteAsync(int noteId, int loginId)
		{
			const string sql = """
				UPDATE ntd_notes
				SET
					is_deleted = true,
					date_deleted = @DateDeleted,
					date_updated = @DateUpdated
				WHERE note_id = @NoteId
					AND login_id = @LoginId;
				""";

			var utcNow = DateTime.UtcNow;

			await using var connection = await DataSource.OpenConnectionAsync();

			await connection.ExecuteAsync(sql, new
			{
				NoteId = noteId,
				LoginId = loginId,
				DateDeleted = utcNow,
				DateUpdated = utcNow
			});
		}
	}
}
