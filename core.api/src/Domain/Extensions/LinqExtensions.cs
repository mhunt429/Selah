namespace Domain.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<IGrouping<int, T>> GroupByCount<T>(this IEnumerable<T> items, int count)
    {
        return items.Zip(Enumerable.Range(0, items.Count()), (s, r) => new { Group = r / count, Item = s })
            .GroupBy(i => i.Group, g => g.Item).ToList();
    }
}