using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CsharpPlayground.Collections;

public static class ListsPlayground
{
    public static void Run()
    {
        // IReadOnlyList<int> is similar to Kotlin List<Int>
        // List<int> is similar to Kotlin ArrayList<Int>
        // LinkedList<int> is similar to Kotlin LinkedList<Int> (doubly linked list)
        IReadOnlyList<int> readonlyNumbers = [1, 2, 3, 4, 5];

        // Iterate through the read-only list
        Console.WriteLine("Numbers: " + readonlyNumbers + ", count = " + readonlyNumbers.Count);
        foreach (var number in readonlyNumbers)
        {
            Console.WriteLine($"    Number: {number}");
        }

        // Access elements by index
        Console.WriteLine($"First number: {readonlyNumbers[0]}");
        Console.WriteLine($"Second number: {readonlyNumbers[1]}");
        var last = readonlyNumbers[^1];
        var secondToLast = readonlyNumbers[^2];
        Console.WriteLine($"Last number: {last}");
        Console.WriteLine($"Second to last number: {secondToLast}");

        // Access elements by range
        // List<int> is similar to Kotlin ArrayList<Int>
        List<int> numbers = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var sublist1 = numbers.GetRange(2..5); // Elements at index 2, 3, 4
        var sublist2 = numbers.GetRange(..4); // Elements at index 0, 1, 2, 3
        var sublist3 = numbers.GetRange(7..); // Elements from index 7 to the end
        Console.WriteLine("Sublist 1 (2..5): " + string.Join(", ", sublist1));
        Console.WriteLine("Sublist 2 (..4): " + string.Join(", ", sublist2));
        Console.WriteLine("Sublist 3 (7..): " + string.Join(", ", sublist3));

        // Modify the list
        numbers.Add(11);
        numbers.AddRange([12, 13, 14]);
        Console.WriteLine("Updated Numbers: " + string.Join(", ", numbers));
        numbers.Remove(14);
        numbers.RemoveAll(e => e == 13);
        numbers.RemoveAt(12);
        Console.WriteLine("After Removals: " + string.Join(", ", numbers));

        // Check equality of 2 lists
        List<int> listA = [1, 2, 3];
        List<int> listB = [1, 2, 3];
        Console.WriteLine($"List A == List B: {listA == listB}"); // compare references
        Console.WriteLine($"List A Equals List B: {listA.Equals(listB)}"); // compare references
        Console.WriteLine($"List A SequenceEqual List B: {listA.SequenceEqual(listB)}"); // compare contents

        // Doubly linked list.
        // LinkedList<int> is similar to Kotlin LinkedList<Int>
        LinkedList<int> linkedList = new();
        linkedList.AddLast(1);
        linkedList.AddLast(2);
        linkedList.AddLast(3);
        Console.WriteLine("LinkedList contents: " + string.Join(", ", linkedList));

        // ImmutableList<int>
        var immutableList = ImmutableList.Create(1, 2, 3);
        var newList = immutableList.Add(4); // instantiate a new list, original remains
        Console.WriteLine("ImmutableList: " + string.Join(", ", immutableList) + ", count = " + immutableList.Count);
        Console.WriteLine("New ImmutableList: " + string.Join(", ", newList) + ", count = " + newList.Count);
        // ==, Equals
        var immutableList1 = ImmutableList.Create(1, 2, 3);
        var immutableList2 = ImmutableList.Create(1, 2, 3);
        Console.WriteLine("immutableList2 == immutableList1: " +
                          (immutableList2 == immutableList1)); // reference comparison
        Console.WriteLine("immutableList2.Equals(immutableList1): " +
                          immutableList2.Equals(immutableList1)); // reference comparison
        Console.WriteLine("immutableList2.SequenceEqual(immutableList1): " +
                          immutableList2.SequenceEqual(immutableList1)); // content comparison
    }
}

public static class ListExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> GetRange<T>(this List<T> @this, Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(@this.Count);
        return @this.GetRange(offset, length);
    }
}