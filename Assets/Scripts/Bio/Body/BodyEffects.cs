using UnityEngine;

public class BodyEffects
{
    public static Vector4 GetEffectColor(string effectName)
    {
        switch (effectName)
        {
            case "Hot":
                return new Vector4(1f, 0f, 0f, 1f);
            case "Migrane":
                return new Vector4(1f, 0f, 0f, 1f);
            case "Weak":
                return new Vector4(0.5f, 0.5f, 1f, 1f);
            case "Sick":
                return new Vector4(0f, 1f, 0f, 1f);
            default:
                return Vector4.zero;
        }
    }   
}