using System.Data.OleDb;

namespace Meracord.Data.SqlAuditor.EntityFramework
{
    /// <summary>
    /// Specific implementation of EFAuditedDbProviderFactory&lt;OleDbFactory&gt; to enable profiling
    /// </summary>
    public class EFAuditedOleDbProviderFactory
        : EFAuditedDbProviderFactory<OleDbFactory>
    {
        private EFAuditedOleDbProviderFactory()
        { }

        /// <summary>
        /// Every provider factory must have an Instance public field
        /// </summary>
        public static new EFAuditedOleDbProviderFactory Instance = new EFAuditedOleDbProviderFactory();
    }
}
