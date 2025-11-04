using System.Collections.Immutable;
using CsharpPlayground.Collections;

namespace CsharpPlayground.OOP;

public enum UserRole
{
    Admin = 101,
    User = 100
}

public static class EnumExtensions
{
    public static T FindOrThrow<T>(Func<T, bool> predicate) where T : struct, Enum
        => Enum.GetValues<T>().First(predicate);
}

public record User(
    Guid Id,
    string Username,
    string Email,
    int Age,
    UserRole Role,
    ImmutableListWithValueSemantics<string> Nicknames
);

public static class RecordsPlayground
{
    public static void Run()
    {
        // UserRole role = Enum.IsDefined(typeof(UserRole), 0)
        //     ? (UserRole)0
        //     : throw new ArgumentOutOfRangeException(nameof(role), "Invalid role value");

        var role = EnumExtensions.FindOrThrow<UserRole>(e => (int)e == 100);
        var description = role switch
        {
            UserRole.Admin => "Administrator with full access",
            UserRole.User => "Regular user with limited access",
            _ => throw new ArgumentOutOfRangeException()
        };
        Console.WriteLine($"Role: {role}, Description: '{description}'");

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
        Console.WriteLine($"user1 == user2: {user1 == user2}"); // compare contents
        Console.WriteLine($"user1.Equals(user2): {user1.Equals(user2)}"); // compare contents

        // Compare hashcode
        Console.WriteLine($"user1.GetHashCode(): {user1.GetHashCode()}");
        Console.WriteLine($"user2.GetHashCode(): {user2.GetHashCode()}");

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
        Console.WriteLine($"user2 is in age group: {ageGroup}");

        // Deconstructing records (positional deconstruction)
        var (id, username, email, age, roleValue, nicknames) = user2;
        Console.WriteLine(
            $"Deconstructed user2: Id={id}, Username={username}, Email={email}, Age={age}, Role={roleValue}, Nicknames=[{string.Join(", ", nicknames)}]");

        // Copy with `with` expression
        var user3 = user2 with { Age = 50, Username = "new-name" };
        Console.WriteLine($"user3 (copied from user2 with modifications): {user3}");
        Console.WriteLine($"user2 and user3 are same instance: {ReferenceEquals(user2, user3)}");
    }
}