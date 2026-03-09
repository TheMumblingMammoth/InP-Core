using UnityEngine;

public class Vector1
{
    public static float MoveTowards(float current, float target, float maxDistanceDelta)
    {
        float num = target - current;
        if (num == 0f || (maxDistanceDelta >= 0f && Mathf.Abs(num) <= maxDistanceDelta))
        {
            return target;
        }
        return current + maxDistanceDelta * (num > 0 ? 1 : -1);
    }
}