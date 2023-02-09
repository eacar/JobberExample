namespace Jobber.Domain.Settings
{
    public sealed class EmailSettings
    {
        #region Properties

        public string TechnicalTeamEmail { get; set; }
        public string BusinessTeamEmail { get; set; }
        public DefaultSettings Default { get; set; }
        public TemplateSettings Template { get; set; }

        #endregion
    }

    public sealed class DefaultSettings
    {
        #region Properties

        public string Domain { get; set; }
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Inbox { get; set; }
        public string Processed { get; set; }

        #endregion
    }
    public sealed class TemplateSettings
    {
        #region Properties

        public string Default { get; set; }

        #endregion
    }
}