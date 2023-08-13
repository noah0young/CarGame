using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static T GetRandomFromList<T>(List<T> list)
    {
        int chosen = Random.Range(0, list.Count);
        return list[chosen];
    }

    public static T GetRandomFromList<T>(List<T> list, List<float> spawnRate)
    {
        spawnRate = NormalizeSpawnRate(spawnRate);
        float chosen = Random.Range(0f, 1f);
        float runningTotal = 0;
        for (int i = 0; i < spawnRate.Count; i++)
        {
            runningTotal += spawnRate[i];
            if (chosen <= runningTotal)
            {
                return list[i];
            }
        }
        return list[list.Count - 1];
    }

    public static List<float> NormalizeSpawnRate(List<float> rate)
    {
        float total = 0;
        foreach (float r in rate)
        {
            total += r;
        }
        List<float> normalized = new List<float>();
        for (int i = 0; i < rate.Count; i++)
        {
            normalized.Add(rate[i] / total);
        }
        return normalized;
    }
}
