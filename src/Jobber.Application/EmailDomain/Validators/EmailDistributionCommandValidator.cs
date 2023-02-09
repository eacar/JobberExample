using FluentValidation;
using Jobber.Application.EmailDomain.Commands;
using Rpa.Globals;
using System.Text.RegularExpressions;

namespace Jobber.Application.EmailDomain.Validators
{
    public interface IEmailSendCommandValidator : IValidator<SendEmailCommand>
    {
    }

    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>, IEmailSendCommandValidator
    {
        public SendEmailCommandValidator()
        {
            RuleFor(c => c.To).Must(c => Regex.IsMatch(c, Regexes.Email));
            RuleFor(c => c.Subject).NotEmpty().WithMessage("Come on! Write a subject!");
        }
    }
}