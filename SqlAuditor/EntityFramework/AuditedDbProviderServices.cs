using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Meracord.Data.SqlAuditor.EntityFramework
{
    public class AuditedDbProviderServices
        : DbProviderServices
    {
        private readonly DbProviderServices _wrapped;
        private readonly ISqlAuditor _auditor;

        public AuditedDbProviderServices(DbProviderServices tail, ISqlAuditor auditor)
        {
            this._wrapped = tail;
            this._auditor = auditor;
        }

        protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
        {
            return _wrapped.GetProviderManifest(manifestToken);
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            var wrappedConnection = connection;

            var profiled = connection as AuditedDbConnection;
            if (profiled != null)
            {
                wrappedConnection = profiled.WrappedConnection;
            }

            return _wrapped.GetProviderManifestToken(wrappedConnection);
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            var cmdDef = _wrapped.CreateCommandDefinition(providerManifest, commandTree);
            var cmd = cmdDef.CreateCommand();
            return CreateCommandDefinition(new AuditedDbCommand(cmd, cmd.Connection, _auditor));
        }

        private static DbConnection GetRealConnection(DbConnection conn)
        {
            var audited = conn as AuditedDbConnection;
            if (audited != null)
            {
                conn = audited.WrappedConnection;
            }
            return conn;
        }

        protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
        {
            _wrapped.CreateDatabase(GetRealConnection(connection), commandTimeout, storeItemCollection);
        }

        protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
        {
            _wrapped.DeleteDatabase(GetRealConnection(connection), commandTimeout, storeItemCollection);
        }

        protected override string DbCreateDatabaseScript(string providerManifestToken, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
        {
            return _wrapped.CreateDatabaseScript(providerManifestToken, storeItemCollection);
        }

        protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
        {
            return _wrapped.DatabaseExists(GetRealConnection(connection), commandTimeout, storeItemCollection);
        }

        /// <summary>
        /// Get DB command definition
        /// </summary>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public override DbCommandDefinition CreateCommandDefinition(DbCommand prototype)
        {
            return _wrapped.CreateCommandDefinition(prototype);
        }
    }
}