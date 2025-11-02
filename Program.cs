// See https://aka.ms/new-console-template for more information

using CsharpPlayground.OOP;

Console.WriteLine("Hello, World!");

Person hoc = new("Hoc Thai")
{
    LastName = "Nguyen",
    Address = "Da Nang"
};
Console.WriteLine($"hoc is {hoc}");

var lastNameLength = hoc.LastName.Length;
Console.WriteLine($"length of hoc's last name is {lastNameLength}");

hoc.Age = 30;
Console.WriteLine($"hoc's age is {hoc.Age}");

var addressLength = hoc.Address?.Length;
Console.WriteLine($"length of hoc's address is {addressLength}");

Person hocClone = new("Hoc Thai")
{
    LastName = "Nguyen",
    Address = "Da Nang",
    Age = 30
};
Console.WriteLine($"hocClone is {hocClone}");
Console.WriteLine($"hoc equals hocClone: {hoc.Equals(hocClone)}");
Console.WriteLine($"hoc equals \"string\": {hoc.Equals("string")}");
Console.WriteLine($"hoc == hocClone: {hoc == hocClone}");
Console.WriteLine($"hoc's hashcode is {hoc.GetHashCode()}");
Console.WriteLine($"hocClone's hashcode is {hocClone.GetHashCode()}");

Person.IncrementCount();
Person.IncrementCount();
Person.IncrementCount();

// Static readonly fields are initialized only once and are not inlined.
var n1 = Person.Names;
var n2 = Person.Names;
Console.WriteLine($"Are n1 and n2 the same instance? {ReferenceEquals(n1, n2)}");

// Const fields will be inlined at compile time
var c1 = Person.BoolConst;
var c2 = Person.BoolConst;
var c3 = c1 && c2;
Console.WriteLine($"c1: {c1}, c2: {c2}, c3: {c3}");
