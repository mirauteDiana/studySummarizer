using FluentValidation;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Application.Validators;

public class SummarizeRequestValidator : AbstractValidator<SummarizeRequest>
{
    public SummarizeRequestValidator()
    {
        RuleFor(x => x.SummaryType)
            .IsInEnum().WithMessage($"SummaryType must be one of: {string.Join(", ", Enum.GetNames<SummaryType>())}.");
    }
}
