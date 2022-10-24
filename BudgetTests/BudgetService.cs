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

        var budgetsOfStart = _budgetRepo.GetAll().FirstOrDefault(x => x.YearMonth.Equals(start.ToString("yyyyMM")));
        var budgetsOfMiddle = _budgetRepo.GetAll().Where(x => x.GetDateTime(x.Days) < end && x.GetDateTime(1) > start);
        var budgetOfEnd = _budgetRepo.GetAll().FirstOrDefault(x => x.YearMonth.Equals(end.ToString("yyyyMM")));

        if (budgetsOfStart == null)
        {
            return 0m;
        }

        if (start.ToString("yyyyMM") == end.ToString("yyyyMM"))
        {
            return budgetsOfStart.Amount / budgetsOfStart.Days * ((end - start).Days + 1);
        }

        var totalAmount = 0m;
        totalAmount += budgetsOfStart.Amount / budgetsOfStart.Days * (budgetsOfStart.Days - start.Day + 1);
        totalAmount += budgetOfEnd.Amount / budgetOfEnd.Days * (end.Day);

        totalAmount += budgetsOfMiddle.Sum(x => x.Amount);

        return totalAmount;
    }
}