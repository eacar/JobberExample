using Jobber.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rpa.Log.Loggers;
using Rpa.WinService.HostService;
using System;

namespace Jobber.App
{
    public class Service : ServiceBaseLifetime
    {
        #region Fields

        private readonly IMediator _mediator;
        private readonly JobSettings _jobSettings;

        #endregion

        #region Constructors

        public Service(
            IMediator mediator, 
            IHostApplicationLifetime applicationLifetime,
            IOptions<JobSettings> jobOptions)
            : base(applicationLifetime)
        {
            _mediator = mediator; //You can use any injection here
            _jobSettings = jobOptions.Value;
        }

        #endregion

        #region Methods - Protected

        protected override void OnStop()
        {
            try
            {
                //Is there you wanna do something very specific on stop? Do it here...
            }
            catch (Exception ex)
            {
                SharpLogger.LogError(ex);
                //Ignore throwing error because the service gotta stop safely.
            }

            base.ApplicationLifetime.StopApplication();
            base.OnStop();
        }

        #endregion
    }
}