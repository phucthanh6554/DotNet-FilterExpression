﻿using FilterExpression;
using FilterExpression.Extension;

var list = new List<Customer>
{
    new Customer{ Name = "Name 1", Age = 25, Id = 1, BirthDay = new DateTime(1997, 9, 15)  },
    new Customer{ Name = "Phuc", Age = 35, Id = 2, BirthDay = new DateTime(1990, 1, 1) },
    new Customer{ Name = "Hiep", Age = 15, Id = 3, BirthDay = new DateTime(2000, 5, 1)  },
};

var filteredList = list.Filter("!((Id eq `1`) | (Id eq `3`))");

Console.WriteLine(filteredList);

class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public DateTime BirthDay { get; set; }
}
