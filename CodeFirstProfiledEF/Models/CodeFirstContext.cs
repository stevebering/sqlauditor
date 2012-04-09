using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;

namespace CodeFirstProfiledEF.Models
{
    public class CodeFirstContext
        : DbContext
    {
        public override int SaveChanges()
        {
            var context = GetObjectContext();
            var changes = context.ObjectStateManager
                .GetObjectStateEntries(EntityState.Deleted | EntityState.Modified | EntityState.Added).ToList();
            if (changes.Any())
            {
                context.ExecuteFunction("dbo.fnStartAuditUser", new ObjectParameter("username", "jdoe"));
            }
            int changesSaved = base.SaveChanges();
            if (changes.Any())
            {
                context.ExecuteFunction("dbo.fnEndAuditUser", new ObjectParameter("username", "jdoe"));
            }
            return changesSaved;
        }

        private ObjectContext GetObjectContext()
        {
            return ((IObjectContextAdapter)this).ObjectContext;
        }
    }
}