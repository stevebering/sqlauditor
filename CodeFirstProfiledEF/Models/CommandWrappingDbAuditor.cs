using System;
using System.Data;
using System.Data.Common;

namespace CodeFirstProfiledEF.Models
{
    public abstract class CommandWrappingDbAuditor
        : DbAuditor
    {
        public override void ExecuteStart(DbCommand command, ExecuteType executeType)
        {
            if (executeType == ExecuteType.NonQuery)
            {
                var userName = CurrentUserContext;

                var connection = command.Connection;
                var auditCommand = connection.CreateCommand();
                auditCommand.CommandType = CommandType.StoredProcedure;
                auditCommand.CommandText = "spConnectionUserSet";
                var userNameParam = auditCommand.CreateParameter();
                userNameParam.ParameterName = "@userName";
                userNameParam.Value = userName;
                userNameParam.Direction = ParameterDirection.Input;
                userNameParam.DbType = DbType.AnsiString;
                userNameParam.Size = 50;

                command.ExecuteNonQuery();
            }
        }

        public override void OnError(DbCommand command, ExecuteType executeType, Exception exception)
        {
            // do nothing
        }

        public override void ExecuteFinish(DbCommand command, ExecuteType executeType, DbDataReader reader)
        {
            if (executeType == ExecuteType.NonQuery)
            {
                var conn = command.Connection;
                var unauditCommand = conn.CreateCommand();
                unauditCommand.CommandType = CommandType.StoredProcedure;
                unauditCommand.CommandText = "spConnectionUserReset";

                command.ExecuteNonQuery();
            }
        }

        public override void ReaderFinish(DbDataReader reader)
        {
            // do nothing
        }
    }
}