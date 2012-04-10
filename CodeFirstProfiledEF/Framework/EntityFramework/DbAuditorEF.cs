using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;

namespace CodeFirstProfiledEF.Framework.EntityFramework
{
    public partial class DbAuditorEF
    {
        public static void Initialize(IDbAuditor auditor, bool supportExplicitConnectionStrings = true)
        {
            Initialize(auditor, false, supportExplicitConnectionStrings);
        }

        public static void Initialize(IDbAuditor auditor, bool applyEFHack, bool supportExplicitConnectionStrings)
        {
            DbAuditor.SetCurrentAuditor(auditor);

            if (supportExplicitConnectionStrings && (applyEFHack || IsEF41HackRequired())) {
                EFProviderUtilities.UseEF41Hack();
            }

            InitializeDbProviderFactories();
        }

        private static void InitializeDbProviderFactories()
        {
            try {
                // ensure that all the factories are loaded
                DbProviderFactories.GetFactoryClasses();
            }
            catch (ArgumentException) {
            }

            Type type = typeof(DbProviderFactories);
            object setOrTable = (type.GetField("_configTable", BindingFlags.NonPublic | BindingFlags.Static) ??
                                 type.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static))
                                 .GetValue(null);

            var table = (DataTable)setOrTable;

            foreach (DataRow row in table.Rows.Cast<DataRow>().ToList()) {
                DbProviderFactory factory;
                try {
                    factory = DbProviderFactories.GetFactory(row);
                }
                catch (Exception) {
                    continue;
                }

                var auditorType = EFProviderUtilities.ResolveFactoryType(factory.GetType());
                if (auditorType != null) {
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

        /// <summary>
        /// Returns true if the EF version is between 4.1.10331.0 and 4.2
        /// </summary>
        /// <returns></returns>
        private static bool IsEF41HackRequired()
        {
            try {
                var efAssembly = typeof(System.Data.Entity.DbContext).Assembly;
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(efAssembly.Location);
                if (fileVersion.FileMajorPart == 4
                    && fileVersion.FileMinorPart == 1
                    && fileVersion.FileBuildPart >= 10331) {
                    return true;
                }
            }
            catch (SecurityException) {
                // As this method requires full trust
                throw new ApplicationException("Could not read file version number of apply EF41 hack. Please try by calling Initialize_EF42() explicitly");
            }
            return false;
        }

    }
}