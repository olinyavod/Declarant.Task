using System;
using FluentValidation.Results;

namespace EasyProg.BusinessLogic.Interfaces
{
	public static class LogicExtesions
	{
		public static ModifyStatus<TModel> ToValidStatus<TModel>(this ValidationResult validation)
		{
			return new ModifyStatus<TModel>
			{
				IsValid = validation.IsValid,
				FailPropeties = validation.Errors
			};
		}

		public static TStatus ToStatus<TStatus>(this Exception ex)
			where TStatus : ExecuteStatus, new ()
		{
			return new TStatus()
			{
				HasError = true, ErrorMessage = ex.ToString()
			};
		}
	}
}