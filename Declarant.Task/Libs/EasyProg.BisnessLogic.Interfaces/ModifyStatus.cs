using System.Collections.Generic;
using FluentValidation.Results;

namespace EasyProg.BusinessLogic.Interfaces
{
	public class ModifyStatus<TResult> : ExecuteStatus<TResult>
	{
		public ModifyStatus()
		{
			IsValid = true;
			FailPropeties = new List<ValidationFailure>();
		}

		public bool IsValid { get; set; }

		public IEnumerable<ValidationFailure> FailPropeties { get; set; }

		public bool WithErrors => HasError || !IsValid;
	}
}