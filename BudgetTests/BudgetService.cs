namespace BudgetTests;

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        if (end < start)
        {
            return 0m;
        }

        var budgets = _budgetRepo.GetAll().Where(x => start <= x.GetDateTime(x.Days) && end >= x.GetDateTime(1)).ToList();
        var budgetOfStart = budgets.FirstOrDefault(x => x.YearMonthEqual(start), Budget.Empty(start));
        var budgetOfEnd = budgets.FirstOrDefault(x => x.YearMonthEqual(end), Budget.Empty(end));

        return budgets.Sum(x => x.Amount)
               - budgetOfStart.AmountPerDay * (start.Day - 1)
               - budgetOfEnd.AmountPerDay * (budgetOfEnd.Days - end.Day);
    }
}