using System;
using Declarant.Task.Models;
using FluentValidation;

namespace Declarant.Task.Validators
{
	class EventItemValidator : AbstractValidator<EventItem>
	{
		public EventItemValidator()
		{
			RuleFor(m => m.Name)
				.MaximumLength(128)
				.WithMessage(string.Format(Properties.Resources.validNameMaxLen, 128));

			RuleFor(m => m.Name)
				.NotEmpty()
				.WithMessage(Properties.Resources.validNameIsRequred);

			RuleFor(m => m.Description)
				.MaximumLength(1024)
				.WithMessage(string.Format(Properties.Resources.validDescriptionName, 1024));

			RuleFor(m => m.StartTime)
				.GreaterThan(new DateTime(1900, 1, 1))
				.WithMessage(string.Format(Properties.Resources.validMinStartTime, new DateTime(1900, 1, 1)));

			RuleFor(m => m.EndTime)
				.GreaterThan(m => m.StartTime)
				.WithMessage(Properties.Resources.validEndTimeAfterStartTime);
		}
	}
}
