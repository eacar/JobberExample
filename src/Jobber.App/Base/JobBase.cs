using MediatR;
using Rpa.Log.Loggers;
using System;
using System.Linq;
using System.Threading;

namespace Jobber.App.Base
{
    public abstract class JobBase : IDisposable
    {
        #region Properties

        protected IMediator Mediator { get; }
        protected Random Rnd { get; }

        #endregion

        #region Constructors

        protected JobBase(IMediator mediator)
        {
            Mediator = mediator;
            Rnd = new Random();
        }

        #endregion

        #region Methods - Protected

        protected void Sleep(int min = 0, int max = 2000)
        {
            Thread.Sleep(Rnd.Next(min, max));
        }
        protected string GetInfo(object id, params string[] logs)
        {
            return $"{GetType().FullName} | {id} | {CombineLogs(logs)}";
        }
        protected string GetInfo(params string[] logs)
        {
            return $"{GetType().FullName} | {CombineLogs(logs)}";
        }
        protected void LogInfo(params string[] logs)
        {
            SharpLogger.LogInfo(GetInfo(logs));
        }
        protected void LogInfo(object id, params string[] logs)
        {
            SharpLogger.LogInfo(GetInfo(id, logs));
        }
        protected void LogWarn(params string[] logs)
        {
            SharpLogger.LogWarn(GetInfo(logs));
        }
        protected void LogWarn(object id, params string[] logs)
        {
            SharpLogger.LogWarn(GetInfo(id, logs));
        }
        protected void LogError(Exception ex, params string[] logs)
        {
            SharpLogger.LogError($"{GetInfo(logs)} | Ex: {ex}");
        }
        protected void LogError(object id, Exception ex, params string[] logs)
        {
            SharpLogger.LogError($"{GetType().FullName} | {id} | {CombineLogs(logs)} | Ex: {ex}");
        }

        #endregion

        #region Methods - Public - IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Nothing to do currently
            }
        }
        ~JobBase()
        {
            Dispose(false);
        }

        #endregion

        #region Methods - Private

        private string CombineLogs(params string[] logs)
        {
            return logs.Any() ? string.Join(" | ", logs) : " - ";
        }

        #endregion
    }
}