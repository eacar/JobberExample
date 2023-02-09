using FluentValidation;
using Jobber.Application.EmailDomain.Commands;
using Jobber.Application.EmailDomain.Validators;
using Jobber.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Jobber.Application.EmailDomain.Handlers
{
    public class EmailCommandHandler
        : IRequestHandler<SendEmailCommand>
    {
        #region Fields

        private readonly EmailSettings _emailSettings;
        private readonly IEmailSendCommandValidator _emailSendCommandValidator;

        #endregion

        #region Constructors

        public EmailCommandHandler(
            IOptions<EmailSettings> emailOptions,
            IEmailSendCommandValidator emailSendCommandValidator)
        {
            _emailSettings = emailOptions.Value;
            _emailSendCommandValidator = emailSendCommandValidator;
            _emailSettings = emailOptions.Value;
        }

        #endregion

        #region Methods - Public

        public async Task<Unit> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            var validation = await _emailSendCommandValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            //One of my NuGet package is includes IExchangeEmailHandler interface

            //You can send it like below:

            //await _exchangeEmailHandler.SendAsync(new SendRequest
            //{
            //    Body = request.Body
            //    EmailTemplate = request.EmailTemplate,
            //    Cc = request.Cc,
            //    To = request.To,
            //    From = _emailSettings.FromEmail,
            //    Host = _emailSettings.Default.Server,
            //    Domain = _emailSettings.Default.Domain,
            //    ExchangeVersion = _emailSettings.Default.ExchangeVersion,
            //});

            return await Unit.Task;
        }

        #endregion
    }
}