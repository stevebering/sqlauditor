using System;
using System.Data.Common;
using System.Reflection;

namespace Meracord.Data.SqlAuditor.EntityFramework
{
    public class EFAuditedDbConnection :
        AuditedDbConnection
    {
        private DbProviderFactory _factory;

        private static readonly Func<DbConnection, DbProviderFactory> RipInnerProvider =
            (Func<DbConnection, DbProviderFactory>)Delegate.CreateDelegate(typeof(Func<DbConnection, DbProviderFactory>),
                                                                           typeof(DbConnection).GetProperty("DbProviderFactory", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                                                               .GetGetMethod(true));
        
        public EFAuditedDbConnection(DbConnection connection, ISqlAuditor auditor)
            : base(connection, auditor)
        { }

        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                if (_factory != null)
                {
                    return _factory;
                }

                DbProviderFactory tail = RipInnerProvider(InnerConnection);
                _factory = (DbProviderFactory)EFProviderUtilities
                                                  .ResolveFactoryTypeOrOriginal(tail.GetType())
                                                  .GetField("Instance", BindingFlags.Public | BindingFlags.Static)
                                                  .GetValue(null);
                return _factory;
            }
        }
    }
}