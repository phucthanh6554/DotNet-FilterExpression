# DotNet-FilterExpression
This libary will help translate string into a C# Expression Tree
Example: ``` ((Name eq `Phuc Nguyen`) & (Age gt `25`)) ``` => ``` x => x.Name == "Phuc Nguyen" && x.Age > 25 ```

## Example Code:
```
using FilterExpression;
using FilterExpression.Extension;

var list = new List<Customer>
{
    new Customer{ Name = "Name 1", Age = 25, Id = 1, BirthDay = new DateTime(1997, 9, 15)  },
    new Customer{ Name = "Name 2", Age = 35, Id = 2, BirthDay = new DateTime(1990, 1, 1) },
    new Customer{ Name = "Name 3", Age = 15, Id = 3, BirthDay = new DateTime(2000, 5, 1)  },
};

var filterService = new FilterService();
var filter = filterService.Filter<Customer>("(BirthDay le `1997-9-15`)");

var filteredList = list.Where(filter.Compile()).ToArray();

// Another way
var filteredList = list.Filter("(BirthDay le `1997-9-15`)");

// Supported for IQueryable
var filteredList = list.AsQueryable.Filter("(BirthDay le `1997-9-15`)").ToList();
```

## Supported Operator

- Equal: ``` (Name eq `Phuc Nguyen`) ``` 
- Not Equal: ``` (Name ne `Phuc Nguyen`) ``` 
- Greater than: ``` (Age gt `25`) ```
- Greater than and equal ``` (Age ge `25`) ```
- Less than: ``` (Age lt `25`) ```
- Less than and equal ``` (Age le `25`) ```
- Contains: ``` (Name contains `P`) ```
- StartsWith: ``` (Name StartsWith `P`) ```
- In: ``` (Age in `[25, 26]`) ```
- And: ``` ( (Age eq `25`) & (Name eq `Phuc`)) ```
- Or: ``` ( (Age eq `25`) | (Name eq `Phuc`)) ```
- Not: ```!(Name eq `Phuc Nguyen`) ```

## Custom Filter
You can create a custom filter syntax by implement IFilterDirective interface
### Example
```
using System.Linq.Expressions;

namespace FilterExpression.Directive
{
    internal class EqualDirective : IFilterDirective
    {
        public string FilterSyntax
        {
            get
            {
                return "eq"; // Syntax will be used in FilterExpression string
            }
        }

        // Build expression tree here 
        public Expression GenerateExpression(ref MemberExpression property, ConstantExpression value)
        {
            return Expression.Equal(property, value);
        }
    }
}
```
