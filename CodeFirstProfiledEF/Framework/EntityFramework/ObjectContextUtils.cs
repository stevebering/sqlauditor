using System;
using System.Configuration;
using System.Data.Common;
using System.Data.EntityClient;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeFirstProfiledEF.Framework.EntityFramework
{
    public static class ObjectContextUtils
    {
        static class MetadataCache<U> where U : System.Data.Objects.ObjectContext
        {
            public static readonly System.Data.Metadata.Edm.MetadataWorkspace workspace;

            static MetadataCache()
            {
                workspace = new System.Data.Metadata.Edm.MetadataWorkspace(
                  new string[] { "res://*/" },
                  new Assembly[] { typeof(U).Assembly });
            }
        }

        /// <summary>
        /// Method 1 for getting a profiled EF context uses reflection
        /// </summary>
        public static T CreateObjectContext<T>(this DbConnection connection) where T : System.Data.Objects.ObjectContext
        {
            var workspace = MetadataCache<T>.workspace;
            var factory = DbProviderServices.GetProviderFactory(connection);

            var itemCollection = workspace.GetItemCollection(System.Data.Metadata.Edm.DataSpace.SSpace);
            itemCollection.GetType().GetField("_providerFactory", // <==== big fat ugly hack
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(itemCollection, factory);

            var ec = new EntityConnection(workspace, connection);
            return CtorCache<T, EntityConnection>.Ctor(ec);
        }


        /// <summary>
        /// Second method for getting an EF context, does no reflection tricks
        /// </summary>
        public static T GetAuditedContext<T>() where T : System.Data.Objects.ObjectContext
        {
            var conn = new EFAuditedDbConnection(GetStoreConnection<T>(), DbAuditor.Current);
            return CreateObjectContext<T>(conn);
        }

        public static DbConnection GetStoreConnection<T>() where T : System.Data.Objects.ObjectContext
        {
            return GetStoreConnection("name=" + typeof(T).Name);
        }

        public static DbConnection GetStoreConnection(string entityConnectionString)
        {
            // Build the initial connection string.
            var builder = new EntityConnectionStringBuilder(entityConnectionString);

            // If the initial connection string refers to an entry in the configuration, load that as the builder.
            object configName;
            if (builder.TryGetValue("name", out configName))
            {
                var configEntry = ConfigurationManager.ConnectionStrings[configName.ToString()];
                builder = new EntityConnectionStringBuilder(configEntry.ConnectionString);
            }

            // Find the proper factory for the underlying connection.
            var factory = DbProviderFactories.GetFactory(builder.Provider);

            // Build the new connection.
            DbConnection tempConnection = null;
            try
            {
                tempConnection = factory.CreateConnection();
                tempConnection.ConnectionString = builder.ProviderConnectionString;

                var connection = tempConnection;
                tempConnection = null;
                return connection;
            }
            finally
            {
                // If creating of the connection failed, dispose the connection.
                if (tempConnection != null)
                {
                    tempConnection.Dispose();
                }
            }
        }

        internal static class CtorCache<TType, TArg> where TType : class
        {
            public static readonly Func<TArg, TType> Ctor;
            static CtorCache()
            {
                Type[] argTypes = new Type[] { typeof(TArg) };
                var ctor = typeof(TType).GetConstructor(argTypes);
                if (ctor == null)
                {
                    Ctor = x => { throw new InvalidOperationException("No suitable constructor defined"); };
                }
                else
                {
                    var dm = new DynamicMethod("ctor", typeof(TType), argTypes);
                    var il = dm.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Newobj, ctor);
                    il.Emit(OpCodes.Ret);
                    Ctor = (Func<TArg, TType>)dm.CreateDelegate(typeof(Func<TArg, TType>));
                }
            }
        }
    }
}