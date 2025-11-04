using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CsharpPlayground.Collections;

public sealed class ImmutableListWithValueSemantics<T>(IImmutableList<T> list)
    : IImmutableList<T>, IEquatable<IImmutableList<T>>
{
    #region IImmutableList implementation

    public T this[int index] => list[index];

    public int Count => list.Count;

    public IImmutableList<T> Add(T value) => list.Add(value).WithValueSemantics();

    public IImmutableList<T> AddRange(IEnumerable<T> items) => list.AddRange(items).WithValueSemantics();

    public IImmutableList<T> Clear() => list.Clear().WithValueSemantics();

    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    public int IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) =>
        list.IndexOf(item, index, count, equalityComparer);

    public IImmutableList<T> Insert(int index, T element) => list.Insert(index, element).WithValueSemantics();

    public IImmutableList<T> InsertRange(int index, IEnumerable<T> items) =>
        list.InsertRange(index, items).WithValueSemantics();

    public int LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) =>
        list.LastIndexOf(item, index, count, equalityComparer);

    public IImmutableList<T> Remove(T value, IEqualityComparer<T>? equalityComparer) =>
        list.Remove(value, equalityComparer).WithValueSemantics();

    public IImmutableList<T> RemoveAll(Predicate<T> match) => list.RemoveAll(match).WithValueSemantics();

    public IImmutableList<T> RemoveAt(int index) => list.RemoveAt(index).WithValueSemantics();

    public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer) =>
        list.RemoveRange(items, equalityComparer).WithValueSemantics();

    public IImmutableList<T> RemoveRange(int index, int count) =>
        list.RemoveRange(index, count).WithValueSemantics();

    public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer) =>
        list.Replace(oldValue, newValue, equalityComparer).WithValueSemantics();

    public IImmutableList<T> SetItem(int index, T value) => list.SetItem(index, value).WithValueSemantics();

    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    #endregion

    public override bool Equals(object? obj) => Equals(obj as IImmutableList<T>);

    public override int GetHashCode()
    {
        unchecked
        {
            return list.Aggregate(19, (h, i) => h * 19 + (i?.GetHashCode() ?? 0));
        }
    }

    public bool Equals(IImmutableList<T>? other) =>
        other is not null && list.SequenceEqual(other);

    public static implicit operator ImmutableListWithValueSemantics<T>(ImmutableList<T> list) =>
        list.WithValueSemantics();

    public static bool operator ==(ImmutableListWithValueSemantics<T>? left,
        ImmutableListWithValueSemantics<T>? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(ImmutableListWithValueSemantics<T>? left,
        ImmutableListWithValueSemantics<T>? right) => !(left == right);
}

internal static class Ex
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImmutableListWithValueSemantics<T> WithValueSemantics<T>(this IImmutableList<T> list) =>
        list as ImmutableListWithValueSemantics<T> ?? new ImmutableListWithValueSemantics<T>(list);
}