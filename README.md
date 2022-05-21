# DotNet-FilterExpression
This libary will help translate string into a C# Expression Tree
Example: (Name eq `Phuc Nguyen` & Age gt `25`) => x => x.Name == "Phuc Nguyen" && x.Age > 25

Support Operators: And (&), Or(|), Equals (eq), GreaterThan (gt), GreaterThanAndEqual(ge), LessThan(lt), LessThanAndEqual(le), StartsWith(StartsWith (string)), Contains(Contains (string))
