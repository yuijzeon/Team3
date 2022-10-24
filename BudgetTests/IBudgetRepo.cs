using System.Collections.Generic;

namespace BudgetTests;

public interface IBudgetRepo
{
    List<Budget> GetAll();
}