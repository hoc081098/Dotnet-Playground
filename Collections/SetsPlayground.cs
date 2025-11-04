using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace CsharpPlayground.Collections;

public static class SetsPlayground
{
    public static void Run()
    {
        // IReadOnlySet<int> is similar to Kotlin Set<Int>
        // ISet<int> is similar to Kotlin MutableSet<Int>
        // HashSet<int> is similar to Kotlin HashSet<Int>
        var hashSet = new HashSet<int>([1, 2, 3, 3, 2, 1]);
        Console.WriteLine("HashSet: " + string.Join(", ", hashSet) + ", count = " + hashSet.Count);

        IReadOnlySet<int> readOnlySet = new ReadOnlySet<int>(hashSet); // Wrap HashSet in ReadOnlySet
        foreach (var number in readOnlySet)
        {
            Console.WriteLine($"    Number: {number}");
        }

        hashSet.Add(100);
        hashSet.Add(200);
        Console.WriteLine("HashSet after Add: " + string.Join(", ", hashSet));
        Console.WriteLine("After additions, ReadOnlySet: " + string.Join(", ", readOnlySet) + ", count = " +
                          readOnlySet.Count);

        Console.WriteLine("Contains 2: " + readOnlySet.Contains(2));
        Console.WriteLine("Contains 500: " + readOnlySet.Contains(500));

        var setA = new HashSet<int>([1, 2, 3, 4, 5]);
        var setB = new HashSet<int>([1, 2, 3, 4, 5]);
        Console.WriteLine($"Set A == Set B: {setA == setB}"); // compare references
        Console.WriteLine($"Set A Equals Set B: {setA.Equals(setB)}"); // compare references
        Console.WriteLine($"Set A SetEquals Set B: {setA.SetEquals(setB)}"); // compare contents

        // SortedSet<int>
        var sortedSet = new SortedSet<int>([1, 4, 6, 3, 4, 5, 7, 3, 2, 1, 100]);
        Console.WriteLine("SortedSet: " + string.Join(", ", sortedSet) + ", count = " + sortedSet.Count);

        // Descending sorted set
        var descendingComparer = Comparer<int>.Create((a, b) => b.CompareTo(a));
        var descendingSortedSet = new SortedSet<int>(descendingComparer) { 1, 4, 6, 3, 5, 7, 2, 100 };
        Console.WriteLine("descendingSortedSet: " + string.Join(", ", descendingSortedSet) + ", count = " +
                          descendingSortedSet.Count);

        Console.WriteLine($"HashSet comparer: {hashSet.Comparer.GetType().Name}");
        Console.WriteLine($"SortedSet comparer: {sortedSet.Comparer.GetType().Name}");

        // ImmutableHashSet<int>
        var immutableSet = ImmutableHashSet.Create(1, 2, 3);
        var newSet = immutableSet.Add(4); // instantiate a new set, original remains unchanged
        Console.WriteLine("ImmutableSet: " + string.Join(", ", immutableSet) + ", count = " + immutableSet.Count);
        Console.WriteLine("New ImmutableSet: " + string.Join(", ", newSet) + ", count = " + newSet.Count);
        var immutableSet1 = ImmutableHashSet.Create(1, 2, 3);
        Console.WriteLine("immutableSet == immutableSet1: " + (immutableSet == immutableSet1)); // reference comparison
        Console.WriteLine("immutableSet.Equals(immutableSet1): " +
                          immutableSet.Equals(immutableSet1)); // reference comparison
        Console.WriteLine("immutableSet.SetEquals(immutableSet1): " +
                          immutableSet.SetEquals(immutableSet1)); // content comparison
        var removedSet = newSet.Remove(2);
        Console.WriteLine("Removed 2 → " + string.Join(", ", removedSet));

        // Set operations
        var a = ImmutableHashSet.Create(1, 2, 3);
        var b = ImmutableHashSet.Create(3, 4, 5);
        var union = a.Union(b); // A ∪ B
        var intersection = a.Intersect(b); // A ∩ B
        var except = a.Except(b); // A - B
        var symmetricExcept = a.SymmetricExcept(b); // (A - B) ∪ (B - A) === (A ∪ B) - (A ∩ B)
        Console.WriteLine("Set A: " + string.Join(", ", a));
        Console.WriteLine("Set B: " + string.Join(", ", b));
        Console.WriteLine("A ∪ B: " + string.Join(", ", union));
        Console.WriteLine("A ∩ B: " + string.Join(", ", intersection));
        Console.WriteLine("A - B: " + string.Join(", ", except));
        Console.WriteLine("(A - B) ∪ (B - A): " + string.Join(", ", symmetricExcept));
        var c = ImmutableHashSet.Create(1,2, 3);
        var d = ImmutableHashSet.Create(2);
        Console.WriteLine("C ⊆ D: " + c.IsSubsetOf(d)); // C is subset of D === (D.....(C)....) => false
        Console.WriteLine("C ⊇ D: " + c.IsSupersetOf(d)); // C is superset of D === (C.....(D)....) => true
    }
}