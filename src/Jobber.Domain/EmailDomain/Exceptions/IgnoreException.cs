using Rpa.WinService.Exceptions;
using System;

namespace Jobber.Domain.EmailDomain.Exceptions
{
    [Serializable]
    public class IgnoreException : ActionException
    {
        #region Properties

        public string Subject { get; }
        public string Body { get; set; }

        #endregion

        #region Constructors

        public IgnoreException(string message, string subject, string body,
            Exception innerException = null,
            bool isRetryable = false,
            int maxRetryCount = 3,
            int waitSecondsWhenFailed = 5)
            : base(message, innerException, isRetryable, maxRetryCount, waitSecondsWhenFailed)
        {
            Subject = subject;
            Body = body;
        }

        #endregion
    }
}