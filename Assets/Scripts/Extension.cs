using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension{
    public static T[] Append<T>(this T[] array, T item) {
        return new List<T>(array) { item }.ToArray();
    }

}
