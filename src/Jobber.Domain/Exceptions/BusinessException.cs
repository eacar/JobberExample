using System;

namespace Jobber.Domain.Exceptions
{
    public class BusinessException : Exception
    {
        #region Constructors

        public BusinessException(string message, Exception ex = null) : base(message, ex)
        {

        }

        #endregion
    }
}