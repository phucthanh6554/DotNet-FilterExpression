# DotNet-FilterExpression
This libary will help translate string into a C# Expression Tree
Example: (Name eq `Phuc Nguyen` & Age gt `25`) => x => x.Name == "Phuc Nguyen" && x.Age > 25

Support Operators: And (&), Or(|), Equals (eq), GreaterThan (gt), GreaterThanAndEqual(ge), LessThan(lt), LessThanAndEqual(le), StartsWith(StartsWith (string)), Contains(Contains (string))

Code:
using FilterExpression;

var list = new List<Customer>
{
    new Customer{ Name = "Name 1", Age = 25, Id = 1, BirthDay = new DateTime(1997, 9, 15)  },
    new Customer{ Name = "Name 2", Age = 35, Id = 2, BirthDay = new DateTime(1990, 1, 1) },
    new Customer{ Name = "Name 3", Age = 15, Id = 3, BirthDay = new DateTime(2000, 5, 1)  },
};

var filterService = new FilterService();
var filter = filterService.Filter<Customer>("(BirthDay le `1997-9-15`)");

var filteredList = list.Where(filter.Compile()).ToArray();
