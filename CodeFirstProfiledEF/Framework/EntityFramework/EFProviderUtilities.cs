using System;

namespace CodeFirstProfiledEF.Framework.EntityFramework
{
    internal class EFProviderUtilities
    {
        public static Func<Type, Type> ResolveFactoryType { get; set; }

        static EFProviderUtilities()
        {
            ResolveFactoryType = GetAuditedProviderFactoryType;
        }

        public static void UseEF41Hack()
        {
            ResolveFactoryType = GetEF41AuditedProviderFactoryType;
        }

        public static Type ResolveFactoryTypeOrOriginal(Type factoryType)
        {
            return ResolveFactoryType(factoryType) ?? factoryType;
        }

        private static Type GetAuditedProviderFactoryType(Type factoryType)
        {
            return typeof(EFAuditedDbProviderFactory<>).MakeGenericType(factoryType);
        }

        private static Type GetEF41AuditedProviderFactoryType(Type factoryType)
        {
            if (factoryType == typeof(System.Data.SqlClient.SqlClientFactory))
                return typeof(EFAuditedSqlClientDbProviderFactory);
            else if (factoryType == typeof(System.Data.OleDb.OleDbFactory))
                return typeof(EFAuditedOleDbProviderFactory);
            else if (factoryType == typeof(System.Data.Odbc.OdbcFactory))
                return typeof(EFAuditedOdbcProviderFactory);

            return null;
        }
    }
}