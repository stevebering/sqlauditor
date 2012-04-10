using System;
using System.Data.Common;

namespace Meracord.Data.SqlAuditor
{
    public abstract class SqlAuditor
        : ISqlAuditor
    {
        private static ISqlAuditor _auditor;

        public static ISqlAuditor Current
        {
            get { return _auditor; }
        }

        internal static void SetCurrentAuditor(ISqlAuditor auditor)
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