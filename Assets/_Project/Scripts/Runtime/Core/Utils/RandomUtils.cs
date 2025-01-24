using UnityEngine;

public static class RandomUtils
{
    public static float Range(float min, float max)
    {
        return Random.Range(min, max);
    }

    public static int Range(int min, int max)
    {
        return Random.Range(min, max);
    }

    public static bool Chance(float percentage)
    {
        return Random.Range(0f, 100f) <= percentage;
    }
}
