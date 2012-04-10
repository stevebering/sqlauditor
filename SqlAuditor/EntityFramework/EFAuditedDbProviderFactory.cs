using System;
using System.Data.Common;
using System.Reflection;

namespace Meracord.Data.SqlAuditor.EntityFramework
{
    public class EFAuditedDbProviderFactory<T>
        : DbProviderFactory, IServiceProvider where T : DbProviderFactory
    {
        /// <summary>
        /// Every provider factory must have an Instance public field
        /// </summary>
        public static EFAuditedDbProviderFactory<T> Instance = new EFAuditedDbProviderFactory<T>();

        private readonly T _tail;

        /// <summary>
        /// Used for db provider apis internally 
        /// </summary>
        protected EFAuditedDbProviderFactory()
        {
            FieldInfo field = typeof(T).GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            this._tail = (T)field.GetValue(null);
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
            return new AuditedDbCommand(_tail.CreateCommand(), null, SqlAuditor.Current);
        }
        /// <summary>
        /// proxy
        /// </summary>
        public override DbConnection CreateConnection()
        {
            return new EFAuditedDbConnection(_tail.CreateConnection(), SqlAuditor.Current);
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

        /// <summary>
        /// Extension mechanism for additional services;  
        /// </summary>
        /// <returns>requested service provider or null.</returns>
        object IServiceProvider.GetService(Type serviceType)
        {
            IServiceProvider tailProvider = _tail as IServiceProvider;
            if (tailProvider == null) return null;
            var svc = tailProvider.GetService(serviceType);
            if (svc == null) return null;

            if (serviceType == typeof(DbProviderServices))
            {
                svc = new AuditedDbProviderServices((DbProviderServices)svc, SqlAuditor.Current);
            }
            return svc;
        }
    }
}