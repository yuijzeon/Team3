using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;

namespace BudgetTests;

public class BudgetServiceTests
{
    private IBudgetRepo _budgetRepo = null!;
    private BudgetService _budgetService = null!;

    [SetUp]
    public void SetUp()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);

        GivenBudgets(new List<Budget>
        {
            CreateBudget("202209", 31),
            CreateBudget("202210", 310000),
            CreateBudget("202211", 3000),
            CreateBudget("202212", 31),
        });
    }

    [Test]
    public void one_month()
    {
        var amount = _budgetService.Query(new DateTime(2022, 10, 1), new DateTime(2022, 10, 31));
        Assert.That(amount, Is.EqualTo(310000m));
    }

    [Test]
    public void one_day()
    {
        var amount = _budgetService.Query(new DateTime(2022, 10, 1), new DateTime(2022, 10, 1));
        Assert.That(amount, Is.EqualTo(10000m));
    }

    [Test]
    public void cross_days()
    {
        var amount = _budgetService.Query(new DateTime(2022, 10, 1), new DateTime(2022, 10, 6));
        Assert.That(amount, Is.EqualTo(60000m));
    }

    [Test]
    public void cross_months()
    {
        var amount = _budgetService.Query(new DateTime(2022, 10, 30), new DateTime(2022, 11, 5));
        Assert.That(amount, Is.EqualTo(20000m + 500m));
    }

    [Test]
    public void cross_three_months()
    {
        var amount = _budgetService.Query(new DateTime(2022, 10, 30), new DateTime(2022, 12, 1));
        Assert.That(amount, Is.EqualTo(20000m + 3000m + 1m));
    }

    [Test]
    public void cross_four_months()
    {
        var amount = _budgetService.Query(new DateTime(2022, 9, 1), new DateTime(2022, 12, 31));
        Assert.That(amount, Is.EqualTo(31 + 310000m + 3000m + 31m));
    }

    [Test]
    public void illegal_date()
    {
        var amount = _budgetService.Query(new DateTime(2022, 10, 30), new DateTime(2022, 1, 5));
        Assert.That(amount, Is.EqualTo(0));
    }

    [Test]
    public void db_no_data()
    {
        var amount = _budgetService.Query(new DateTime(2000, 10, 30), new DateTime(2000, 11, 5));
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

    private ConfiguredCall GivenBudgets(List<Budget> returnThis)
    {
        return _budgetRepo.GetAll().Returns(returnThis);
    }
}