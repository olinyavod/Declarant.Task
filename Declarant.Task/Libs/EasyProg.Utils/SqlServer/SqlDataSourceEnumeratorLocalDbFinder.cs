using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Threading.Tasks;

namespace EasyProg.Utils.SqlServer
{
	public class SqlDataSourceEnumeratorLocalDbFinder : ILocalDbFinder
	{
		public IEnumerable<string> FindInstances()
		{
			foreach (DataRow row in SqlDataSourceEnumerator.Instance.GetDataSources().Rows)
			{
				var serverName = row["ServerName"];
				var instanceName = row["InstanceName"];
				yield return $@"{serverName}\{instanceName}";
			}
		}

		public Task<IEnumerable<string>> FindInstancesAsync()
		{
			return Task.Run(() => FindInstances());
		}
	}
}