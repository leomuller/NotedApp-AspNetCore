using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotedData.NotedApp.Repositories
{
	public interface INoteRepository
	{
		Task<IEnumerable<Models.Note>> GetByLoginIdAsync(int loginId);

		Task<Models.Note?> GetByIdAsync(int noteId, int loginId);

		Task<int> CreateAsync(Models.Note note);

		Task UpdateAsync(Models.Note note);

		Task SoftDeleteAsync(int noteId, int loginId);
	}
}
