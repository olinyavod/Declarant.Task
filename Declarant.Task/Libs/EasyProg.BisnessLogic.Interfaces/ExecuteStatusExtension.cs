namespace EasyProg.BusinessLogic.Interfaces
{
	public static class ExecuteStatusExtension
	{
		public static TStatus ToStatus<TStatus>(this ExecuteStatus result)
			where TStatus: ExecuteStatus, new()
		{
			return new TStatus()
			{
				HasError = result.HasError,
				ErrorMessage = result.ErrorMessage
			};
		}
	}
}
