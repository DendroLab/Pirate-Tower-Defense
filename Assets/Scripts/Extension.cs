using System.Collections.Generic;

public static class Extension
{
    public static T[] Append<T>(this T[] array, T item)
    {
        return new List<T>(array) { item }.ToArray();
    }

}
