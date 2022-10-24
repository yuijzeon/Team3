using System.Globalization;

namespace BudgetTests;

public class Budget
{
    private DateTime _yearMonth;

    public string YearMonth
    {
        get => _yearMonth.ToString("yyyyMM");
        set => _yearMonth = DateTime.ParseExact(value, "yyyyMM", CultureInfo.InvariantCulture);
    }

    public int Amount { get; set; }

    public int Days => DateTime.DaysInMonth(_yearMonth.Year, _yearMonth.Month);

    public decimal AmountPerDay => (decimal)Amount / Days;

    public DateTime GetDateTime(int day)
    {
        return new DateTime(_yearMonth.Year, _yearMonth.Month, day);
    }

    public bool YearMonthEqual(DateTime value)
    {
        return _yearMonth.Year == value.Year && _yearMonth.Month == value.Month;
    }

    public static Budget Empty(DateTime dateTime)
    {
        return new Budget
        {
            YearMonth = dateTime.ToString("yyyyMM")
        };
    }
}