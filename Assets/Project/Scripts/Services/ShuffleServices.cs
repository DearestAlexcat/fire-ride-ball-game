using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableEx
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> Source)
    {
        T[] data = Source.ToArray();

        Random rs = new Random();

        for (int i = data.Length - 1; i > -1; --i)
        {
            int newIndex = rs.Next(i + 1);
            yield return data[newIndex];
            data[newIndex] = data[i];
        }
    }
}

public static class ShuffleServices
{
    public static void FillWithRandoms(Array Arr)
    {
        // The method is only for arrays of numbers
        if (Arr.GetType().GetElementType() != typeof(int))
            throw new TypeAccessException("This method available only for int arrays!");

        // Cache the lengths of all array dimensions
        int[] lengths = Enumerable.Range(0, Arr.Rank).Select(x => Arr.GetLength(x)).ToArray();

        // Let's create an array of indices by the number of dimensions (pivots)
        int[] indices = new int[Arr.Rank];

        // Generate a sequence of numbers along the length of the array, mix and get its enumerator
        IEnumerator<int> randoms = Enumerable.Range(0, Arr.Length).Shuffle().GetEnumerator();
        randoms.MoveNext();


        // Loop through the entire massive structure
        // Each iteration we move both the randoms enumerator and the values in indices
        for (int i = 0; i < Arr.Length; ++i, randoms.MoveNext(), MoveNext(Arr.Rank - 1))
            Arr.SetValue(randoms.Current, indices);

        // The recursive function that modifies the indices array, step by step iterating over
        // the indices of all array elements:
        // 0, 0, 0, ..., 0
        // 0, 0, 0, ..., 1
        // 0, 0, 0, ..., 2s
        // ...
        // 3, 2, 4, ..., 0
        // ...
        void MoveNext(int index)
        {
            if (index != -1 && ++indices[index] == lengths[index])
            {
                indices[index] = 0;
                MoveNext(index - 1);
            }
        }
    }
}
