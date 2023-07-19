using System;
using System.Collections.Generic;

public static class RandomExtensions
{
    private static Random rand = new Random((int)(69420 + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds));

    public static bool BinaryRandom()
    {

        return rand.NextDouble() > 0.4999999f;
    }

    public static bool PercentRoll(float intervalPercent)
    {
        intervalPercent = Clamp01(intervalPercent);

        return rand.NextDouble() <= intervalPercent;
    }

    public static float Clamp01(float number)
    {
        return Math.Max(0,Math.Min(number, 1.0f));
    }

    public static int Range(int minInclusive, int maxExclusive)
    {
        return rand.Next(minInclusive, maxExclusive);
    }

    public static bool RollXOr(int rolls)
    {
        rolls = Math.Max(1, rolls);
        for (int i = 0; i < rolls; i++)
        {
            if (BinaryRandom())
                return true;
        }

        return false;
    }

    public static bool RollXAnd(int rolls)
    {
        rolls = Math.Max(1, rolls);
        for (int i = 0; i < rolls; i++)
        {
            if (!BinaryRandom())
                return false;
        }

        return true;
    }

    public static T RandomEntry<T>(this T[] array)
    {
        if(array == null)
        {
            throw new NullReferenceException("Array to select random from is null.");
        }

        if(array.Length == 0)
        {
            throw new NullReferenceException("Array is empty.");
        }

        return array[Range(0, array.Length)];

    }

    public static T RandomEntry<T>(this List<T> list)
    {
        if (list == null)
        {
            throw new NullReferenceException("List to select random from is null.");
        }

        if (list.Count == 0)
        {
            throw new NullReferenceException("List is empty.");
        }

        return list[Range(0, list.Count)];

    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}