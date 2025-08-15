using System.Linq;
using UnityEngine;

public class BodyTestUI : MonoBehaviour
{
    public static Body[] testBodies;
    [SerializeField] Transform container;
    void Awake()
    {
        testBodies = container.GetComponentsInChildren<Body>();
    }

    public static void SetColor(Color color)
    {
        foreach (Body body in testBodies)
            body.SetColor(color, true);
    }

    public static void SetState(string state)
    {
        foreach (Body body in testBodies)
            body.ChangeState(state);
    }

    public static void EffectClick(string effectName)
    {
        foreach (Body body in testBodies)
            if (body.HasEffect(effectName))
                body.RemoveEffect(effectName);
            else
                body.AddEffect(effectName);
    }

    Body.BodyPart[] bodyParts;
    public static void LimbChoice(string effectName)
    {
        foreach (Body body in testBodies)
            if (body.HasEffect(effectName))
                body.RemoveEffect(effectName);
            else
                body.AddEffect(effectName);
    }
    string [] effects;
    public static void EffectChoice(string effectName)
    {
        foreach (Body body in testBodies)
            if (body.HasEffect(effectName))
                body.RemoveEffect(effectName);
            else
                body.AddEffect(effectName);
    }

    
}