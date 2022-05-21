using FilterExpression;

var list = new List<Customer>
{
    new Customer{ Name = "Phuc", Age = 25, Id = 1, BirthDay = new DateTime(1997, 9, 15)  },
    new Customer{ Name = "An", Age = 35, Id = 2, BirthDay = new DateTime(1990, 1, 1) },
    new Customer{ Name = "Teo", Age = 15, Id = 3, BirthDay = new DateTime(2000, 5, 1)  },
};

var filterService = new FilterService();
var filter = filterService.Filter<Customer>("(BirthDay le `1997-9-15`)");

var test = list.Where(filter.Compile()).ToArray();

Console.WriteLine(test);

class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public DateTime BirthDay { get; set; }
}
