using System;
using System.Data.Common;
using Meracord.Data.SqlAuditor;

namespace CodeFirstProfiledEF
{
    public abstract class DebtSettlementAuditor
        : ISqlAuditor
    {
        public bool IsActive
        {
            get { return true; }
        }

        protected abstract string GetCurrentUserName { get; }

        public void ExecuteStart(DbCommand command, ExecuteType executeType)
        {
            if (command.IsModification() && IsUserNameSet()) {
                // now we want to track it
                var commandText = command.CommandText;
                var setUserName = string.Format("exec dbo.spSetCurrentUserName @username = '{0}'", GetCurrentUserName);
                var clearUserName = string.Format("exec dbo.spClearCurrentUserName");

                command.CommandText = string.Format("{0} \r\n {1} \r\n {2}", setUserName, commandText, clearUserName);
            }
        }

        public void OnError(DbCommand command, ExecuteType executeType, Exception exception)
        {
            // do we need to do anything here?
        }

        public void ExecuteFinish(DbCommand command, ExecuteType executeType, DbDataReader reader)
        {
            // do we need to do anything here?
        }

        public void ReaderFinish(DbDataReader reader)
        {
            // do we need to do anything here?
        }

        private bool IsUserNameSet()
        {
            return (!string.IsNullOrEmpty(GetCurrentUserName));
        }
    }

    public static class DbCommandExtensions
    {
        public static bool IsModification(this DbCommand command)
        {
            return (command.IsUpdate() || command.IsInsert() || command.IsDelete());
        }

        public static bool IsUpdate(this DbCommand command)
        {
            return Is(command, "UPDATE ");
        }

        public static bool IsInsert(this DbCommand command)
        {
            return Is(command, "INSERT ");
        }

        public static bool IsDelete(this DbCommand command)
        {
            return Is(command, "DELETE ");
        }

        public static bool Is(DbCommand command, string commandType)
        {
            return command.CommandText.Trim().StartsWith(commandType, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}