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
        var budgetsOfMiddle = _budgetRepo.GetAll().Where(x => GetEndOfMonth(x) < end && GetStartOfMonth(x) > start);
        var budgetOfEnd = _budgetRepo.GetAll().FirstOrDefault(x => x.YearMonth.Equals(end.ToString("yyyyMM")));
        
        if (budgetsOfStart == null)
        {
            return 0m;
        }
        if (start.ToString("yyyyMM") == end.ToString("yyyyMM"))
        {
            return budgetsOfStart.Amount / GetDaysInMonth(budgetsOfStart) * ((end -start).Days +1);
        }

        var totalAmount = 0m;
        totalAmount += budgetsOfStart.Amount / GetDaysInMonth(budgetsOfStart) * (GetDaysInMonth(budgetsOfStart) - start.Day +1);
        totalAmount += budgetOfEnd.Amount / GetDaysInMonth(budgetOfEnd) * (end.Day);

        totalAmount += budgetsOfMiddle.Sum(x => x.Amount);
        
        return totalAmount;
    }

    private static DateTime GetStartOfMonth(Budget x)
    {
        return new DateTime(int.Parse(x.YearMonth.Substring(0, 4)), int.Parse(x.YearMonth.Substring(4,2)), 1);
    }
    private static DateTime GetEndOfMonth(Budget x)
    {
        return new DateTime(int.Parse(x.YearMonth.Substring(0, 4)), int.Parse(x.YearMonth.Substring(4,2)), GetDaysInMonth(x));
    }

    private static int GetDaysInMonth(Budget budgets)
    {
        return DateTime.DaysInMonth(int.Parse(budgets.YearMonth.Substring(0,4)),int.Parse(budgets.YearMonth.Substring(4,2)));
    }
}