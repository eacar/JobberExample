using Newtonsoft.Json;
using Rpa.WinService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jobber.Domain.Settings
{
    public interface IJobSettings
    {
        #region Properties

        ConfigDetailSettings DefaultJobDetail { get; set; }
        List<JobConfigSettings> JobConfigs { get; set; }

        #endregion

        #region Methods

        JobConfigSettings GetJobConfig<T>() where T : class;

        #endregion
    }

    public sealed class JobSettings : IJobSettings
    {
        #region Properties

        public ConfigDetailSettings DefaultJobDetail { get; set; } = new ConfigDetailSettings();
        public List<JobConfigSettings> JobConfigs { get; set; } = new List<JobConfigSettings>();

        #endregion

        #region Methods - Public

        public JobConfigSettings GetJobConfig<T>() where T : class
        {
            return JobConfigs.First(c => c.Type.FullName == typeof(T).FullName);
        }

        #endregion
    }

    public sealed class JobConfigSettings
    {
        #region Properties

        public string Name { get; set; }
        public ConfigDetailSettings Detail { get; set; } = new ConfigDetailSettings();

        [JsonIgnore]
        public Type Type => Type.GetType($"Jobber.App.Jobs.{Name}, Jobber.App");

        #endregion
    }

    public sealed class ConfigDetailSettings
    {
        #region Properties - Shared

        public JobRunType JobRunType { get; set; }
        public int WaitWhenNoDataSeconds { get; set; }
        public int MaxRetryCount { get; set; }
        public int MaxDbRetryCount { get; set; }
        public int TopRecordCount { get; set; }
        public string QuartzScheduler { get; set; }
        public int MaxParallelTaskCount { get; set; }
        public bool? IsActive { get; set; }
        public bool IsTest { get; set; }

        #endregion

        #region Somewhat common props

        public string Folder { get; set; }
        public int Threshold { get; set; }

        #endregion
    }
}