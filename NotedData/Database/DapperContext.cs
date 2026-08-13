using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace NotedData.Database
{
	//class is abstract because it doesn't do anything on its own. So only for inheritance. 
	public abstract class DapperContext : IAsyncDisposable
	{
		private readonly NpgsqlDataSource _dataSource;

		public DapperContext(string dbConString)
		{
			_dataSource = NpgsqlDataSource.Create(dbConString);
		}

		public NpgsqlDataSource DataSource
		{
			get
			{
				return _dataSource;
			}
		}

		public async ValueTask DisposeAsync()
		{
			await _dataSource.DisposeAsync();
		}
	}
}
