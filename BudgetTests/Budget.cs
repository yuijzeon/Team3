﻿namespace BudgetTests;

public class Budget
{
    private int _month;

    private int _year;

    public string YearMonth
    {
        get => _year + _month.ToString("00");
        set
        {
            _year = int.Parse(value.Substring(0, 4));
            _month = int.Parse(value.Substring(4, 2));
        }
    }

    public int Amount { get; set; }

    public int Days => DateTime.DaysInMonth(_year, _month);

    public DateTime GetDateTime(int day)
    {
        return new DateTime(_year, _month, day);
    }
}