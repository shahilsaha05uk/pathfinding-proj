using UnityEngine;

public static class GridConfigHelper
{
    public static float DeviatedValue(float value, float deviation)
    {
        return Random.Range(value - deviation, value + deviation);
    }

    public static float DeviatedValue(Vector2 value, float deviation)
    {
        return Random.Range(
            minInclusive: DeviatedValue(value.x, deviation),
            maxInclusive: DeviatedValue(value.y, deviation));
    }

    public static float DeviatedValue((float x, float y) value, float deviation)
    {
        return Random.Range(
            minInclusive: DeviatedValue(value.x, deviation),
            maxInclusive: DeviatedValue(value.y, deviation));
    }
}