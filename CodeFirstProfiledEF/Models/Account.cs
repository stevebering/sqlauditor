using System.Collections.Generic;

namespace CodeFirstProfiledEF.Models
{
    public class Account
    {
        public virtual int AccountID { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual int AccountTypeID { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual int AccountLockTypeID { get; set; }
        public virtual AccountLockType AccountLockType { get; set; }
        public virtual int AccountStatusTypeID { get; set; }
        public virtual AccountStatusType AccountStatusType { get; set; }
        public virtual int ContactID { get; set; }
        public virtual Contact Contact { get; set; }
    }

    public class AccountLockType
    {
        public virtual int AccountLockTypeID { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }

    public class AccountStatusType
    {
        public virtual int AccountStatusTypeID { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }

    public class AccountType
    {
        public virtual int AccountTypeID { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }

    public class Contact
    {
        public virtual int ContactID { get; set; }
        public virtual string Name { get; set; }
    }
}