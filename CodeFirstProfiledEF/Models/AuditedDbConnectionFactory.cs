using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace CodeFirstProfiledEF.Models
{
    /// <summary>
    /// Connection factory used for EF Code First DbContext API
    /// </summary>
    public class AuditedDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDbConnectionFactory _wrapped;

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
            return new AuditedDbConnection(_wrapped.CreateConnection(nameOrConnectionString), DbAuditor.Current);
        }
    }
}