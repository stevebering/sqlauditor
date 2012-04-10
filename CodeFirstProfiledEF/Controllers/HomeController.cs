using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeFirstProfiledEF.Models;

namespace CodeFirstProfiledEF.Controllers
{
    public class HomeController : Controller
    {
        private ICodeFirstContext _context;
        //
        // GET: /Widget/

        public HomeController()
        {
            _context = new CodeFirstContext();
        }

        public ActionResult Index()
        {
            var items = GetSelectedItems();
            var accountTypes = _context.AccountTypes.ToList();
            var lockTypes = _context.AccountLockTypes.ToList();
            var statusTypes = _context.AccountStatusTypes.ToList();

            var model = items.Select(x => Convert(x, accountTypes, lockTypes, statusTypes));

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = _context.Accounts.FirstOrDefault(x => x.AccountID == id);
            var accountTypes = _context.AccountTypes.ToList();
            var lockTypes = _context.AccountLockTypes.ToList();
            var statusTypes = _context.AccountStatusTypes.ToList();

            var model = Convert(item, accountTypes, lockTypes, statusTypes);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AccountListItemViewModel item)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.AccountID == item.AccountID);
            var currentItem = item;
            account.AccountStatusTypeID = currentItem.AccountStatusTypeID;
            account.AccountLockTypeID = currentItem.AccountLockTypeID;
            account.AccountTypeID = currentItem.AccountTypeID;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private IList<Account> GetSelectedItems()
        {
            return _context.Accounts.Where(x => x.Contact.Name.Contains("lomax")).ToList();
        }

        private static AccountListItemViewModel Convert(Account account, IEnumerable<AccountType> accountTypes, IEnumerable<AccountLockType> lockTypes, IEnumerable<AccountStatusType> statusTypes)
        {
            return new AccountListItemViewModel {
                AccountID = account.AccountID,
                AccountNumber = account.AccountNumber,

                AccountLockTypeName = account.AccountLockType.Description,
                AccountLockTypeID = account.AccountLockTypeID,

                AccountStatusTypeName = account.AccountStatusType.Description,
                AccountStatusTypeID = account.AccountStatusTypeID,

                AccountTypeName = account.AccountType.Description,
                AccountTypeID = account.AccountTypeID,

                AccountTypes = accountTypes.Select(x => new SelectListItem {
                    Text = x.Description,
                    Value = x.AccountTypeID.ToString(CultureInfo.InvariantCulture),
                    Selected = x.AccountTypeID == account.AccountTypeID
                }),
                LockTypes = lockTypes.Select(x => new SelectListItem {
                    Text = x.Description,
                    Value = x.AccountLockTypeID.ToString(CultureInfo.InvariantCulture),
                    Selected = x.AccountLockTypeID == account.AccountLockTypeID
                }),
                StatusTypes = statusTypes.Select(x => new SelectListItem {
                    Text = x.Description,
                    Value = x.AccountStatusTypeID.ToString(CultureInfo.InvariantCulture),
                    Selected = x.AccountStatusTypeID == account.AccountStatusTypeID
                }),
            };
        }

        public class AccountListItemViewModel
        {
            public int AccountID { get; set; }
            public int AccountTypeID { get; set; }
            public string AccountTypeName { get; set; }
            public int AccountStatusTypeID { get; set; }
            public string AccountStatusTypeName { get; set; }
            public int AccountLockTypeID { get; set; }
            public string AccountLockTypeName { get; set; }
            public string AccountNumber { get; set; }

            public IEnumerable<SelectListItem> AccountTypes { get; set; }
            public IEnumerable<SelectListItem> LockTypes { get; set; }
            public IEnumerable<SelectListItem> StatusTypes { get; set; }
        }
    }
}
