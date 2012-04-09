using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace CodeFirstProfiledEF.Models.EntityFramework
{
    public partial class DbAuditorEF
    {
        public static void Initialize()
        {
            InitializeDbProviderFactories();
        }

        private static void InitializeDbProviderFactories()
        {
            try
            {
                // ensure that all the factories are loaded
                DbProviderFactories.GetFactoryClasses();
            }
            catch (ArgumentException)
            {
            }

            Type type = typeof(DbProviderFactories);
            object setOrTable = (type.GetField("_configTable", BindingFlags.NonPublic | BindingFlags.Static) ??
                                 type.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static))
                                 .GetValue(null);

            var table = (DataTable)setOrTable;

            foreach (DataRow row in table.Rows.Cast<DataRow>().ToList())
            {
                DbProviderFactory factory;
                try
                {
                    factory = DbProviderFactories.GetFactory(row);
                }
                catch (Exception)
                {
                    continue;
                }

                var auditorType = EFProviderUtilities.ResolveFactoryType(factory.GetType());
                if (auditorType != null)
                {
                    DataRow audited = table.NewRow();
                    audited["Name"] = row["Name"];
                    audited["Description"] = row["Description"];
                    audited["InvariantName"] = row["InvariantName"];
                    audited["AssemblyQualifiedName"] = auditorType.AssemblyQualifiedName;
                    table.Rows.Remove(row);
                    table.Rows.Add(audited);
                }
            }
        }

    }
}