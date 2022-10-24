using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;

namespace BudgetTests;

public class BudgetServiceTests
{
    private BudgetService _budgetService = null!;
    private IBudgetRepo _budgetRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);
    }

    [Test]
    public void one_month()
    {
        _budgetRepo.GetAll().Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202210",
                Amount = 310000
            }
        });

        var amount = _budgetService.Query(new DateTime(2022, 10, 1), new DateTime(2022, 10, 31));
        Assert.That(amount, Is.EqualTo(310000m));
    }

    [Test]
    public void one_day()
    {
        GivenBudget(new List<Budget>
        {
            CreateBudget("202210", 310000)
        });

        var amount = _budgetService.Query(new DateTime(2022, 10, 1), new DateTime(2022, 10, 1));
        Assert.That(amount, Is.EqualTo(10000m));
    }

    [Test]
    public void cross_days()
    {
        GivenBudget(new List<Budget>
        {
            CreateBudget("202210", 310000)
        });

        var amount = _budgetService.Query(new DateTime(2022, 10, 1), new DateTime(2022, 10, 6));
        Assert.That(amount, Is.EqualTo(60000m));
    }

    [Test]
    public void cross_months()
    {
        GivenBudget(new List<Budget>
        {
            CreateBudget("202210", 310000),
            CreateBudget("202211", 30000)
        });

        var amount = _budgetService.Query(new DateTime(2022, 10, 30), new DateTime(2022, 11, 5));
        Assert.That(amount, Is.EqualTo(20000m + 5000m));
    }

    [Test]
    public void cross_three_months()
    {
        GivenBudget(new List<Budget>
        {
            CreateBudget("202210", 310000),
            CreateBudget("202211", 3000),
            CreateBudget("202212", 31)
        });

        var amount = _budgetService.Query(new DateTime(2022, 10, 30), new DateTime(2022, 12, 1));
        Assert.That(amount, Is.EqualTo(20000m + 3000m + 1m));
    }

    [Test]
    public void cross_four_months()
    {
        GivenBudget(new List<Budget>
        {
            CreateBudget("202210", 31),
            CreateBudget("202211", 3000),
            CreateBudget("202212", 310000),
            CreateBudget("202301", 31),
        });

        var amount = _budgetService.Query(new DateTime(2022, 10, 1), new DateTime(2023, 1, 30));
        Assert.That(amount, Is.EqualTo(31 + 310000m + 3000m + 30m));
    }

    [Test]
    public void illegal_date()
    {
        GivenBudget(new List<Budget>
        {
            CreateBudget("202210", 310000),
            CreateBudget("202211", 3000),
            CreateBudget("202212", 31)
        });

        var amount = _budgetService.Query(new DateTime(2022, 10, 30), new DateTime(2022, 1, 5));
        Assert.That(amount, Is.EqualTo(0));
    }

    [Test]
    public void db_no_data()
    {
        GivenBudget(new List<Budget>());

        var amount = _budgetService.Query(new DateTime(2022, 10, 30), new DateTime(2022, 11, 5));
        Assert.That(amount, Is.EqualTo(0));
    }

    private static Budget CreateBudget(string yearMonth, int amount)
    {
        return new Budget
        {
            YearMonth = yearMonth,
            Amount = amount
        };
    }

    private ConfiguredCall GivenBudget(List<Budget> returnThis)
    {
        return _budgetRepo.GetAll().Returns(returnThis);
    }
}