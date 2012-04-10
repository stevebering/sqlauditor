using System.Data.Odbc;

namespace CodeFirstProfiledEF.Framework.EntityFramework
{
    /// <summary>
    /// Specific implementation of EFProfiledDbProviderFactory&lt;OdbcFactory&gt; to enable profiling
    /// </summary>
    public class EFAuditedOdbcProviderFactory
        : EFAuditedDbProviderFactory<OdbcFactory>
    {
        private EFAuditedOdbcProviderFactory()
        { }

        /// <summary>
        /// Every provider factory must have an Instance public field
        /// </summary>
        public static new EFAuditedOdbcProviderFactory Instance = new EFAuditedOdbcProviderFactory();
    }
}
