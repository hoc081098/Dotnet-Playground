using System.Collections.Immutable;
using CsharpPlayground.Collections;

namespace CsharpPlayground.OOP;

public enum UserRole
{
    Admin = 101,
    User = 100
}

public record User(
    Guid Id,
    string Username,
    string Email,
    int Age,
    UserRole Role,
    ImmutableListWithValueSemantics<string> Nicknames
);

public record RecordWithInitProperties
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Email { get; set; }
}

public record struct RecordStructMutable(int Id, string Name);

public readonly record struct RecordStructReadonly(int Id, string Name);

public static class RecordsPlayground
{
    public static void Run()
    {
        // UserRole role = Enum.IsDefined(typeof(UserRole), 0)
        //     ? (UserRole)0
        //     : throw new ArgumentOutOfRangeException(nameof(role), "Invalid role value");

        var role = EnumExtensions.FindByValueOrThrow<UserRole>(100);
        var description = role switch
        {
            UserRole.Admin => "Administrator with full access",
            UserRole.User => "Regular user with limited access",
            _ => throw new ArgumentOutOfRangeException()
        };
        Console.WriteLine($"Role: {role}, Description: '{description}'\n");

        var newGuid = Guid.NewGuid();

        var user1 = new User(
            Id: newGuid,
            Username: "hoc081098",
            Email: "hoc081098@gmail.com",
            Age: 30,
            Role: role,
            Nicknames: ImmutableList.Create("Học", "Học 098")
        );
        var user2 = new User(
            Id: newGuid,
            Username: "hoc081098",
            Email: "hoc081098@gmail.com",
            Age: 30,
            Role: role,
            Nicknames: ImmutableList.Create("Học", "Học 098")
        );

        Console.WriteLine($"user1: {user1}");
        Console.WriteLine($"user2: {user2}");

        // Compare user1 and user2
        Console.WriteLine($"user1 == user2: {user1 == user2}"); // compare contents (static operator ==)
        Console.WriteLine($"user1.Equals(user2): {user1.Equals(user2)}"); // compare contents (instance method Equals)

        // Compare hashcode
        Console.WriteLine($"user1.GetHashCode(): {user1.GetHashCode()}");
        Console.WriteLine($"user2.GetHashCode(): {user2.GetHashCode()}\n");

        // Switch statement with property patterns
        switch (user2)
        {
            case { Age: 18 }:
                Console.WriteLine($"user2 has age 18");
                break;
            case { Age: > 18 }:
                Console.WriteLine($"user2 is adult");
                break;
        }

        // Switch expression with property patterns
        var ageGroup = user2 switch
        {
            { Age: < 13 } => "Child",
            { Age: < 20 } => "Teenager",
            { Age: < 65 } => "Adult",
            { Age: >= 65 } => "Senior",
            _ => "Unknown"
        };
        Console.WriteLine($"user2 is in age group: {ageGroup}\n");

        // Deconstructing records (positional deconstruction)
        var (id, username, email, age, roleValue, nicknames) = user2;
        Console.WriteLine(
            $"Deconstructed user2: Id={id}, Username={username}, Email={email}, Age={age}, Role={roleValue}, Nicknames=[{string.Join(", ", nicknames)}]\n");

        // Copy with `with` expression
        // user2.Age = 100; // Cannot change Age since records are immutable by default
        var user3 = user2 with { Age = 50, Username = "new-name" };
        Console.WriteLine($"user3 (copied from user2 with modifications): {user3}");
        Console.WriteLine($"user2 and user3 are same instance: {ReferenceEquals(user2, user3)}");

        Console.WriteLine(new string('-', 80));
        // ------------------------------- Init-only properties in records -------------------------------
        var recordWithInitProperties1 = new RecordWithInitProperties
        {
            Id = 1998,
            Name = "hoc081098",
            Email = "hoc081098@gmail.com"
        };
        var recordWithInitProperties2 = new RecordWithInitProperties
        {
            Id = 1998,
            Name = "hoc081098"
        };
        Console.WriteLine($"recordWithInitProperties1: {recordWithInitProperties1}");
        Console.WriteLine($"recordWithInitProperties2: {recordWithInitProperties2}");
        Console.WriteLine($"recordWithInitProperties1 == recordWithInitProperties2:" +
                          $" {recordWithInitProperties1 == recordWithInitProperties2}"); // False because Email is different (null vs "hoc081098@gmail.com")
        recordWithInitProperties2.Email = "hoc081098@gmail.com";
        Console.WriteLine($"After setting Email, recordWithInitProperties1 == recordWithInitProperties2:" +
                          $" {recordWithInitProperties1 == recordWithInitProperties2}"); // True because all properties are now the same

        Console.WriteLine(new string('-', 80));
        // ------------------------------- Struct records -------------------------------
        var recordStructMutable1 = new RecordStructMutable(Id: 1, Name: "Record Struct Mutable");
        var recordStructMutable2 = new RecordStructMutable(Id: 1, Name: "Record Struct Mutable");
        Console.WriteLine($"recordStructMutable1: {recordStructMutable1}");
        Console.WriteLine($"recordStructMutable2: {recordStructMutable2}");
        Console.WriteLine(
            $"recordStructMutable1 == recordStructMutable2: {recordStructMutable1 == recordStructMutable2}"); // True

        Console.WriteLine($"Before modifying recordStructMutable2: {recordStructMutable2}");
        DemoRecordStructMutable(recordStructMutable2);
        Console.WriteLine(
            $"After modifying recordStructMutable2: {recordStructMutable2}"); // do not change because struct is passed by value


        var recordStructReadonly = new RecordStructReadonly(10, "hoc");
        Console.WriteLine($"recordStructReadonly: {recordStructReadonly}");
        DemoRecordStructReadonly(recordStructReadonly);
        // recordStructReadonly.Id = 10; // Error: cannot modify because it's readonly
    }

    private static void DemoRecordStructMutable(RecordStructMutable r)
    {
        r.Name += "new name";
    }

    private static void DemoRecordStructReadonly(RecordStructReadonly r)
    {
        // r.Name += "new name"; // Error: cannot modify because it's readonly
    }
}