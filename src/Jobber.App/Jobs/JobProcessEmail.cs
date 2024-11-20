using Jobber.App.Base;
using Jobber.Application.EmailDomain.Commands;
using Jobber.Application.EmailDomain.Queries;
using Jobber.Application.EmailDomain.Responses;
using Jobber.Domain.Exceptions;
using Jobber.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using Rpa.WinService.Contracts;
using SerilogTimings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Jobber.App.Jobs
{
    /// <summary>
    /// This demonstrates reading a bunch of emails and then processing them in a parallel manner.
    /// Note that this example you can produce some data and then consume it. There is another example named
    /// "JobMonitorFolderSize.cs" which does ONLY have one method. Because, you don't always have a case
    /// where you would have batch data to consume.
    /// </summary>
    public sealed class JobProcessEmail : JobBase, IJob
    {
        #region Fields

        private readonly CultureInfo _ci;
        private readonly EmailSettings _emailSettings;
        private readonly IJobHandler _jobHandler;
        private readonly JobConfigSettings _jobConfigSettings;

        #endregion

        #region Constructors

        public JobProcessEmail(
            IMediator mediator, 
            IJobSettings jobSettings,
            IOptions<EmailSettings> emailOptions,
            IJobHandler jobHandler)
            : base(mediator)
        {
            _ci = CultureInfo.GetCultureInfo("en-US");
            _emailSettings = emailOptions.Value;
            _jobHandler = jobHandler;
            _jobConfigSettings = jobSettings.GetJobConfig<JobProcessEmail>(); //Make sure this is the constructor name
        }

        #endregion

        #region Methods - Public - IJob

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(async () =>
            {
                await _jobHandler.RunAsync<EmailResponse, BusinessException>(
                    _jobConfigSettings.Detail.JobRunType,
                    Produce,
                    Consume,
                    CatchBusinessException, //You don't have to put this or any other exception handling.
                    CatchException, //If something really goes bad with the EmailResponse (Or any other IJobItem) there already is a default catch exception logs and continues. This "CatchException" method is just a possible override.
                    sleepSecondsWhenNoData: _jobConfigSettings.Detail.WaitWhenNoDataSeconds,
                    maxParallelTaskCount: _jobConfigSettings.Detail.MaxParallelTaskCount,
                    timeoutGetItems: 25000 //ms
                );
            });
        }

        #endregion

        #region Methods - Private

        private async Task<IEnumerable<EmailResponse>> Produce()
        {
            var items = await base.Mediator.Send(new FilterEmailsQuery
            {
                TopCount = _jobConfigSettings.Detail.TopRecordCount,
                EmailFolder = _emailSettings.Default.Inbox,
                IsReadAsHtml = true,
                IsGetAttachments = true
            });

            return items;
        }

        private async Task Consume(EmailResponse item)
        {
            base.LogInfo($"Consuming subject '{item.EmailItem.Subject}'...");

            using (Operation.Time($"Subject '{item.EmailItem.Subject}' is completed!"))
            {
                switch (item.EmailItem.Subject.Trim().ToLower(_ci))
                {
                    case "car":
                        //Invoke some logic! Preferable another mediator.
                        base.LogInfo(item.EmailItem.Subject, "is processed as cars.");

                        await Task.Delay(1000); //Giving the impression of working hard...
                        break;

                    case "furniture":
                        //Perhaps, you do not want this in particular
                        
                        throw new BusinessException("This is something we have abandoned years ago! So, thanks, but no thanks!");

                    default:

                        base.LogInfo(item.EmailItem.Subject, "is something we do not care!");
                        //Don't bother if it's something we don't know...

                        break;
                }
            }
        }

        #endregion

        #region Catching Exceptions

        private async Task CatchBusinessException(BusinessException ex, EmailResponse item)
        {
            base.LogWarn(item.EmailItem.Subject, ex.Message);  //It can be a warning because its what we have already been expecting...

            await base.Mediator.Send(new SendEmailCommand
            {
                Subject = $"{item.EmailItem.Subject} - Not good!",
                Body = "Sorry, we know what's wrong. But we won't tell you!",
                To = item.EmailItem.From?.Address, //Send who ever send this
                Cc = _emailSettings.BusinessTeamEmail, //You might want to inform you Business team!
            });
        }

        private async Task CatchException(Exception ex, EmailResponse item)
        {
            base.LogError(item.EmailItem.Subject, ex);

            await base.Mediator.Send(new SendEmailCommand
            {
                Subject = $"{item.EmailItem.Subject} - Failed!",
                Body = "Sorry, something happened even we don't know...",
                To = item.EmailItem.From?.Address, //Send who ever send this
                Cc = _emailSettings.TechnicalTeamEmail, //You might want to inform you Technical team!
            });
        }

        #endregion
    }
}