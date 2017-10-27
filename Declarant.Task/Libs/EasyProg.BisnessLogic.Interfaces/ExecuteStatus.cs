namespace EasyProg.BusinessLogic.Interfaces
{
	public class ExecuteStatus
	{
		public bool HasError { get; set; }

		public string ErrorMessage { get; set; }
	}

	public class ExecuteStatus<TResult> : ExecuteStatus
	{
		public TResult Result { get; set; }
	}
}