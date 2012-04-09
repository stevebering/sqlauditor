using System;
using System.Data.Common;

namespace CodeFirstProfiledEF.Models
{
    public abstract class DbAuditor
        : IDbAuditor
    {
        private bool _isActive = false;

        public bool IsActive
        {
            get { return _isActive; }
        }

        public static IDbAuditor Current
        {
            get
            {
                //Settings.EnsureAuditingProvider();
                //return Settings.EnsureAuditingProvider.GetCurrentAuditor;
                return null;
            }
        }

        public abstract void ExecuteStart(DbCommand command, ExecuteType executeType);

        public abstract void OnError(DbCommand command, ExecuteType executeType, Exception exception);

        public abstract void ExecuteFinish(DbCommand command, ExecuteType executeType, DbDataReader reader);

        public abstract void ReaderFinish(DbDataReader reader);

        protected abstract string CurrentUserContext { get; }
    }
}