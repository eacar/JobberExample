using Jobber.Domain.EmailDomain.Entities;
using Rpa.WinService.Contracts;

namespace Jobber.Application.EmailDomain.Responses
{
    public class EmailResponse : IJobItem
    {
        #region Properties

        public EmailItem EmailItem { get; set; }

        #endregion
    }
}