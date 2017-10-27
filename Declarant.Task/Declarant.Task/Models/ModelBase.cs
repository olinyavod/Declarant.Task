using EasyProg.Models.Interfaces;

namespace Declarant.Task.Models
{
	public class ModelBase : IIdentity<int>
	{
		public int Id { get; set; }
	}
}