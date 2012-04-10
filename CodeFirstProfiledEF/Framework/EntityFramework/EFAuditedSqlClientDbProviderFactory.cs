using System.Data.SqlClient;

namespace CodeFirstProfiledEF.Framework.EntityFramework
{
    /// <summary>
    /// Specific implementation of EFAuditedDbProviderFactory&lt;SqlClientFactory&gt; to enable profiling
    /// </summary>
    public class EFAuditedSqlClientDbProviderFactory
        : EFAuditedDbProviderFactory<SqlClientFactory>
    {
        private EFAuditedSqlClientDbProviderFactory()
        { }

        /// <summary>
        /// Every provider factory must have an Instance public field
        /// </summary>
        public static new EFAuditedSqlClientDbProviderFactory Instance = new EFAuditedSqlClientDbProviderFactory();
    }
}
