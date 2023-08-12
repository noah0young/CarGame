using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static T GetRandomFromList<T>(List<T> list)
    {
        int chosen = Random.Range(0, list.Count);
        return list[chosen];
    }
}
