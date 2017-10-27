using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyProg.Utils.SqlServer
{
	public interface ILocalDbFinder
	{
		IEnumerable<string> FindInstances();

		Task<IEnumerable<string>> FindInstancesAsync();
	}
}
