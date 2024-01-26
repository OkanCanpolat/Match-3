using System.Collections.Generic;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rnd = new System.Random();

        for (int i = list.Count; i > 0; i--)
            list.Swap(0, rnd.Next(0, i));
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        T temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}
