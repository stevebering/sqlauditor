using System.Web;

namespace CodeFirstProfiledEF
{
    public class WebDebtSettlementAuditor
        : DebtSettlementAuditor
    {
        protected override string GetCurrentUserName
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null || context.User == null || context.User.Identity.IsAuthenticated == false) {
                    return null;
                }

                return context.User.Identity.Name;
            }
        }
    }
}