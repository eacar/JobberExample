using Jobber.Application.EmailDomain.Validators;
using Jobber.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rpa.Extensions;
using Rpa.WinService.Extensions;
using Rpa.WinService.Jobbers;
using System;
using System.IO.Abstractions;

namespace Jobber.App
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Settings Injection

            services.Configure<ServiceSettings>(options => _configuration.GetSection("Service").Bind(options));
            services.Configure<EmailSettings>(options => _configuration.GetSection("Email").Bind(options));
            services.Configure<JobSettings>(options => _configuration.GetSection("Job").Bind(options));

            #endregion

            #region Core Services

            services.AddRpaServices();
            services.AddRpaWinServiceServices();

            #endregion

            #region Mediatr

            services.AddMediatR(AppDomain.CurrentDomain.Load("Jobber.Application"));
            services.AddMediatR(AppDomain.CurrentDomain.Load("Jobber.Domain"));

            #endregion

            #region Misc Services

            services.AddSingleton<IFileSystem, FileSystem>();

            #endregion

            #region Validators

            services.AddScoped<IEmailSendCommandValidator, SendEmailCommandValidator>();

            #endregion

            #region Job

            var sb = services.BuildServiceProvider();

            var jobSettings = sb.GetService<IOptions<JobSettings>>();

            foreach (var job in jobSettings.Value.JobConfigs)
            {
                job.Detail = jobSettings.Value.DefaultJobDetail.OverridePropsWhenTargetHasDefaultValue(job.Detail);
                services.AddSingleton(job.Type);

                if (job.Detail.IsActive.HasValue && job.Detail.IsActive.Value)
                {
                    services.AddSingleton(new JobSchedule(
                        job.Type,
                        job.Detail.QuartzScheduler,
                        job.Detail.JobRunType));
                }
            }

            services.AddSingleton<IJobSettings, JobSettings>(_ => jobSettings.Value);

            #endregion
        }
    }
}