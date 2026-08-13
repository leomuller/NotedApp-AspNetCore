using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotedData.Auth.AuthApp.Repositories
{
	public interface IAuthAppRepository
	{
		Task<IEnumerable<Models.AuthApp>> GetAllAsync();

		Task<Models.AuthApp?> GetByIdAsync(int appId);
	}
}
