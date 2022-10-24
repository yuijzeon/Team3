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

        if (budgets.Count == 1)
        {
            return budgets[0].AmountPerDay * ((end - start).Days + 1);
        }

        var budgetOfStart = budgets.FirstOrDefault(x => x.YearMonthEqual(start), Budget.Empty(start));
        var budgetsOfMiddle = budgets.Where(x => start < x.GetDateTime(1) && x.GetDateTime(x.Days) < end);
        var budgetOfEnd = budgets.FirstOrDefault(x => x.YearMonthEqual(end), Budget.Empty(start));

        var totalAmount = 0m;
        totalAmount += budgetOfStart.AmountPerDay * (budgetOfStart.Days - start.Day + 1);
        totalAmount += budgetsOfMiddle.Sum(x => x.Amount);
        totalAmount += budgetOfEnd.AmountPerDay * end.Day;

        return totalAmount;
    }
}