using Jobber.Application.EmailDomain.Responses;
using MediatR;
using System.Collections.Generic;

namespace Jobber.Application.EmailDomain.Queries
{
    public class FilterEmailsQuery : IRequest<IEnumerable<EmailResponse>>
    {
        #region Properties

        public int TopCount { get; set; }
        public string EmailFolder { get; set; }
        public bool IsGetAttachments { get; set; }
        public bool IsReadAsHtml { get; set; }
        public bool IsMoveAfterRead { get; set; }

        #endregion
    }
}