using Jobber.Application.EmailDomain.Queries;
using Jobber.Application.EmailDomain.Responses;
using Jobber.Domain.Entities;
using Jobber.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Jobber.Application.EmailDomain.Handlers
{
    public class EmailQueryHandler
        : IRequestHandler<FilterEmailsQuery, IEnumerable<EmailResponse>>
    {
        #region Fields
        
        private readonly EmailSettings _emailSettings;
        private readonly Random _rnd; //Just for testing and presentation purposes
        private readonly List<string> _possibleSubjects = new List<string>{ "Car", "Furniture", "Wrestling", "Jesse Pinkman" }; //Just for testing and presentation purposes

        #endregion

        #region Constructors

        public EmailQueryHandler(
            IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
            _rnd = new Random();
        }

        #endregion

        #region Methods - Public

        public async Task<IEnumerable<EmailResponse>> Handle(FilterEmailsQuery request, CancellationToken cancellationToken)
        {
            var result = new List<EmailResponse>();

            //Read emails from some source...
            //One of my NuGet package is includes IExchangeEmailHandler interface
            //And you can use these:
            /*
                _emailSettings.Default.Username
                _emailSettings.Default.Password
                _emailSettings.Default.Domain
                and so on...
             */

            for (int i = 0; i < 10; i++)
            {
                result.Add(new EmailResponse
                {
                    EmailItem = new EmailItem
                    {
                        Subject = _possibleSubjects[_rnd.Next(0, _possibleSubjects.Count - 1)],
                        Body = $"Super body {i}",
                        From = new MailAddress($"mysuperEmail_{i}@mail.com")
                    }
                });
            }

            return await Task.FromResult(result);
        }

        #endregion
    }
}