using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;

namespace CodeFirstProfiledEF.Models
{
    public class CodeFirstContext
        : DbContext, ICodeFirstContext
    {
        public CodeFirstContext()
            : base("DebtSettlement")
        {
            Database.SetInitializer(new DoNothingDbInitializer());
        }

        private ObjectContext GetObjectContext()
        {
            return ((IObjectContextAdapter)this).ObjectContext;
        }

        public IDbSet<Account> Accounts { get; set; }
        public IDbSet<AccountStatusType> AccountStatusTypes { get; set; }
        public IDbSet<AccountLockType> AccountLockTypes { get; set; }
        public IDbSet<AccountType> AccountTypes { get; set; }
        public IDbSet<Contact> Contacts { get; set; }

        private class DoNothingDbInitializer
            : IDatabaseInitializer<CodeFirstContext>
        {
            public void InitializeDatabase(CodeFirstContext context)
            {

            }
        }
    }

    public interface ICodeFirstContext
    {
        IDbSet<Account> Accounts { get; set; }
        IDbSet<AccountStatusType> AccountStatusTypes { get; set; }
        IDbSet<AccountLockType> AccountLockTypes { get; set; }
        IDbSet<AccountType> AccountTypes { get; set; }
        IDbSet<Contact> Contacts { get; set; }

        int SaveChanges();
    }
}