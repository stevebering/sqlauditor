using System.Data.Entity.Infrastructure;

namespace CodeFirstProfiledEF.Framework.EntityFramework
{
    public class AuditedDbConnectionFactory : IDbConnectionFactory
    {
        readonly IDbConnectionFactory _wrapped;

        /// <summary>
        /// Create a profiled connection factory
        /// </summary>
        /// <param name="wrapped">The underlying connection that needs to be profiled</param>
        public AuditedDbConnectionFactory(IDbConnectionFactory wrapped)
        {
            _wrapped = wrapped;
        }

        /// <summary>
        /// Create a wrapped connection for profiling purposes 
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        /// <returns></returns>
        public System.Data.Common.DbConnection CreateConnection(string nameOrConnectionString)
        {
            return new EFAuditedDbConnection(_wrapped.CreateConnection(nameOrConnectionString), DbAuditor.Current);
        }
    }
}