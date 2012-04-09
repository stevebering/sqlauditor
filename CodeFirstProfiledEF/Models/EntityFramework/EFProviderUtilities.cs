using System;

namespace CodeFirstProfiledEF.Models.EntityFramework
{
    internal class EFProviderUtilities
    {
        public static Func<Type, Type> ResolveFactoryType { get; set; }

        static EFProviderUtilities()
        {
            ResolveFactoryType = GetAuditedProviderFactoryType;
        }

        public static Type ResolveFactoryTypeOrOriginal(Type factoryType)
        {
            return ResolveFactoryType(factoryType) ?? factoryType;
        }

        private static Type GetAuditedProviderFactoryType(Type factoryType)
        {
            return typeof(EFAuditedDbProviderFactory<>).MakeGenericType(factoryType);
        }
    }
}