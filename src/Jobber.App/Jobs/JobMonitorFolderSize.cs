using Jobber.App.Base;
using Jobber.Domain.Settings;
using MediatR;
using Quartz;
using Rpa.WinService.Contracts;
using SerilogTimings;
using System.Threading.Tasks;

namespace Jobber.App.Jobs
{
    /// <summary>
    /// This demonstrates ONLY doing one job either always, one-time or most likely you'll use Scheduled.
    /// In this case, this job only checks some folder size and if so, either you can delete the older files
    /// or you can just inform the authorities
    /// </summary>
    public sealed class JobMonitorFolderSize : JobBase, IJob
    {
        #region Fields

        private readonly IJobHandler _jobHandler;
        private readonly JobConfigSettings _jobConfigSettings;

        #endregion

        #region Constructors

        public JobMonitorFolderSize(
            IMediator mediator, 
            IJobSettings jobSettings,
            IJobHandler jobHandler)
            : base(mediator)
        {
            _jobHandler = jobHandler;
            _jobConfigSettings = jobSettings.GetJobConfig<JobMonitorFolderSize>(); //Make sure this is the constructor name
        }

        #endregion

        #region Methods - Public - IJob

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(async () =>
            {
                await _jobHandler.RunAsync( //You can still use CUSTOM exception handlers here, I just wanted to keep it simple and didn't use
                    _jobConfigSettings.Detail.JobRunType,
                    Do
                );
            });
        }

        #endregion

        #region Methods - Private

        private async Task  Do()
        {
            base.LogInfo($"Checking folder '{_jobConfigSettings.Detail.Folder}'");

            using (Operation.Time($"Clearing folder '{_jobConfigSettings.Detail.Folder}' is completed!"))
            {
                //Get the folder size,
                int folderSize = 10_000_000;

                if (folderSize >= _jobConfigSettings.Detail.Threshold)
                {
                    //Send email or delete them or both
                    await Task.Delay(4000); //Trying to look busy...
                }
            }
        }

        #endregion
    }
}