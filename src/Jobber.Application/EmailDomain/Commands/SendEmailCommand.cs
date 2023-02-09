using MediatR;
using System.Collections.Generic;

namespace Jobber.Application.EmailDomain.Commands
{
    public class SendEmailCommand : IRequest
    {
        #region Properties

        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Cc { get; set; }
        public List<string> Attachments { get; set; }
        public bool IsHtmlBody { get; set; } = true;

        public bool IsSaveCopy { get; set; } = true;
        public int TimeOut { get; set; } = 30000;

        #endregion
    }
}