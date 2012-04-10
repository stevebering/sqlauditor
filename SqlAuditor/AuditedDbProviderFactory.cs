using System.Data.Common;

namespace Meracord.Data.SqlAuditor
{
    public class AuditedDbProviderFactory : DbProviderFactory
    {
        /// <summary>
        /// Every provider factory must have an Instance public field
        /// </summary>
        public static AuditedDbProviderFactory Instance = new AuditedDbProviderFactory();

        private ISqlAuditor _auditor;
        private DbProviderFactory _tail;

        /// <summary>
        /// Used for db provider apis internally 
        /// </summary>
        private AuditedDbProviderFactory()
        {

        }

        /// <summary>
        /// Allow to re-init the auditor factory.
        /// </summary>
        /// <param name="auditor"> </param>
        /// <param name="tail"></param>
        public void InitProfiledDbProviderFactory(ISqlAuditor auditor, DbProviderFactory tail)
        {
            this._auditor = auditor;
            this._tail = tail;
        }

        /// <summary>
        /// proxy
        /// </summary>
        /// <param name="auditor"></param>
        /// <param name="tail"></param>
        public AuditedDbProviderFactory(ISqlAuditor auditor, DbProviderFactory tail)
        {
            this._auditor = auditor;
            this._tail = tail;
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override bool CanCreateDataSourceEnumerator
        {
            get
            {
                return _tail.CanCreateDataSourceEnumerator;
            }
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return _tail.CreateDataSourceEnumerator();
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbCommand CreateCommand()
        {
            return new AuditedDbCommand(_tail.CreateCommand(), null, _auditor);
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbConnection CreateConnection()
        {
            return new AuditedDbConnection(_tail.CreateConnection(), _auditor);
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbParameter CreateParameter()
        {
            return _tail.CreateParameter();
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return _tail.CreateConnectionStringBuilder();
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return _tail.CreateCommandBuilder();
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbDataAdapter CreateDataAdapter()
        {
            return _tail.CreateDataAdapter();
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override System.Security.CodeAccessPermission CreatePermission(System.Security.Permissions.PermissionState state)
        {
            return _tail.CreatePermission(state);
        }
    }
}