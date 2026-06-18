using FluentValidation;
using StudySummarizer.Application.DTOs;

namespace StudySummarizer.Application.Validators;

public class UploadDocumentFormValidator : AbstractValidator<UploadDocumentRequest>
{
    public UploadDocumentFormValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessages.TitleRequired);

        RuleFor(x => x.File)
            .NotNull().WithMessage(ValidationMessages.FileRequired);

        RuleFor(x => x.File)
            .Must(f => f!.Length > 0).WithMessage(ValidationMessages.FileEmpty)
            .Must(f => ValidationMessages.AllowedFileExtensions.Contains(Path.GetExtension(f!.FileName)))
            .WithMessage(ValidationMessages.FileTypeNotAllowed)
            .When(x => x.File is not null);
    }
}
