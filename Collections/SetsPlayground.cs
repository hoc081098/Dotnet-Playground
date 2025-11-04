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
        Console.WriteLine("ReadOnlySet: " + string.Join(", ", hashSet) + ", count = " + hashSet.Count);

        IReadOnlySet<int> readOnlySet = new ReadOnlySet<int>(hashSet); // Wrap HashSet in ReadOnlySet
        foreach (var number in readOnlySet)
        {
            Console.WriteLine($"    Number: {number}");
        }

        hashSet.Add(100);
        hashSet.Add(200);
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
    }
}