using System;
using System.Data.Common;
using CodeFirstProfiledEF.Models;

namespace CodeFirstProfiledEF.Framework
{
    public abstract class DbAuditor
        : IDbAuditor
    {
        private bool _isActive = false;
        private static IDbAuditor _auditor;

        public bool IsActive
        {
            get { return _isActive; }
        }

        public static IDbAuditor Current
        {
            get { return _auditor; }
        }

        internal static void SetCurrentAuditor(IDbAuditor auditor)
        {
            _auditor = auditor;
        }

        public abstract void ExecuteStart(DbCommand command, ExecuteType executeType);

        public abstract void OnError(DbCommand command, ExecuteType executeType, Exception exception);

        public abstract void ExecuteFinish(DbCommand command, ExecuteType executeType, DbDataReader reader);

        public abstract void ReaderFinish(DbDataReader reader);

        protected abstract string CurrentUserContext { get; }
    }
}