using System.Collections.Generic;
using UnityEngine;
public class Body : MonoBehaviour
{
    // Unit per Pixel
    const float UPP = 1f / 32f;
    [SerializeField] Limb[] limbs;
    [SerializeField] Vector2[] originPositions;
    [SerializeField] float[] originRotations;

    [ContextMenu("SetOrigin")]
    void SetOrigin()
    {
        originPositions = new Vector2[limbs.Length];
        originRotations = new float[limbs.Length];
        for (int i = 0; i < limbs.Length; i++)
        {
            originPositions[i] = (Vector2)limbs[i].transform.localPosition;
            originRotations[i] = limbs[i].transform.localRotation.eulerAngles.z;
        }
    }

    [ContextMenu("Reset")]
    void Reset()
    {
        for (int i = 0; i < limbs.Length; i++)
        {
            limbs[i].transform.localPosition = originPositions[i];
            Quaternion q = new Quaternion(0, 0, 0, 1f);
            q.eulerAngles = new Vector3(0, 0, originRotations[i]);
            limbs[i].transform.localRotation = q;
        }
    }
    #region Animation

    [ContextMenu("Print Frame")]
    void PrintFrame()
    {
        Debug.Log(SnapFrame().ToString());
    }
    BodyFrame SnapFrame()
    {
        //Quaternion q;
        BodyFrame frame = new BodyFrame(limbs.Length);
        for (int i = 0; i < limbs.Length; i++)
        {
            float angle = limbs[i].transform.localRotation.eulerAngles.z - originRotations[i];
            if (angle > 180)
                angle -= 360;
            frame.Set(i, ((Vector2)limbs[i].transform.localPosition - originPositions[i]) / UPP, angle);
        }
        return frame;
    }

    BodyClip bodyClip;
    BodyFrame previous = null, current;
    //float angle = 0;
    float timer, time;
    int frameI = 0;
    void NextFrame()
    {
        timer -= time;
        previous = bodyClip.GetFrameByNumber(frameI);
        frameI++;
        if (frameI >= bodyClip.size)
        {
            if (!Casual())
                ChangeState(old_state);
            frameI = 0;
        }
        current = bodyClip.GetFrameByNumber(frameI).GetCopy();
        time = bodyClip.time[frameI];
    }
    void ApplyClip()
    {
        while (timer >= time)
            NextFrame();
        BodyFrame frame = previous.MoveTo(current, timer / time, tremblePosition, GetSlug(), GetSway());
        for (int i = 0; i < limbs.Length; i++)
        {
            limbs[i].SetPos(originPositions[i] + frame.GetPosition(i) * UPP);
            Quaternion q = new Quaternion(0, 0, 0, 1f);
            q.eulerAngles = new Vector3(0, 0, originRotations[i] + frame.GetRotation(i));
            if (i == 0)
            {
                limbs[i].transform.localRotation = new Quaternion(0, 0, 0, 1f);
                limbs[i].transform.RotateAround(transform.position, Vector3.forward,
                                             originRotations[i] + frame.GetRotation(i));
            }
            else
                limbs[i].transform.localRotation = q;
        }
    }

    [SerializeField] public string state = "Stand";
    string old_state = "";
    float animationSpeed = 1f;
    public float GetProgress() { return timer / bodyClip.Length(); }

    public void ChangeState(string state)
    {
        if (bodyClip != null && this.state == state)
            return;
        previous = SnapFrame();
        this.state = state;
        old_state = "";
        bodyClip = BodyClip.clips[state];
        current = bodyClip.GetFrameByNumber(0);
        timer = 0;
        frameI = 0;
        time = 0.125f;
    }

    public void PlayOnce(string newClip)
    {
        previous = SnapFrame();
        old_state = state;
        state = newClip;
        timer = 0;
        bodyClip = BodyClip.clips[state];
        frameI = 0;
        time = 0.125f;
        current = bodyClip.GetFrameByNumber(0);
    }

    public bool Casual() { return old_state == ""; }

    #endregion Animation

    void Start()
    {
        ChangeState(state);
        SetSkin();
        tremblePosition = new Vector2[limbs.Length];
    }
    int order = 0;

    void FixedUpdate()
    {
        if (animationSpeed == 0) return;
        timer += Time.fixedDeltaTime * Core.TimeScale() * animationSpeed;
        UpdateDelta();
        ApplyClip();
        if (order != -(int)(transform.position.y * 1000) + skinID)
        {
            order = -(int)(transform.position.y * 1000) + skinID;
            foreach (Limb limb in limbs)
                limb.SetOrder(order);
        }
    }


    public void SetColor(Color color, bool save = false, bool onlyMain = false)
    {
        if (save)
            this.color = color;
        for (int i = 0; i < limbs.Length; i++)
        {
            if (onlyMain && i == 2)
                return;
            limbs[i].SetColor(color);
        }
    }

    public void Right() { transform.localScale = new Vector3(-1, 1, 1); }
    public void Left() { transform.localScale = new Vector3(1, 1, 1); }

    #region State
    [SerializeField] bool male;
    [SerializeField] int size;

    private int SkinID() { return size + (male ? 0 : 3); }

    List<string> effects = new List<string>(10);
    public void AddEffect(string effect)
    {
        effects.Add(effect);
        SetEffects(effects.ToArray());
    }
    public void RemoveEffect(string effect)
    {
        effects.Remove(effect);
        SetEffects(effects.ToArray());
    }
    public bool HasEffect(string effect) { return effects.Contains(effect); }
    public string EffectsToString()
    {
        string list = "";
        foreach (string effect in effects)
        {
            list += effect + "\n";
        }
        return list;
    }
    #endregion

    #region Skins
    public bool child;
    public int skinID;
    [ContextMenu("SetSkin")]
    public void SetSkin()
    {
        SetSkin(SkinID());
    }
    public void SetSkin(int skinID)
    {
        this.skinID = skinID;
        foreach (Limb limb in limbs)
            limb.SetSkin(skinID);
    }
    public Sprite GetHead()
    {
        return Resources.Load<Sprite>("Sprites/Heads/Heads" + (skinID + 1).ToString());
    }
    #endregion

    #region VisualEffects
    [SerializeField] Color color = Color.white;
    float delta_a, delta_b;
    float delta_time;

    void UpdateDelta()
    {
        delta_time = (delta_time + Time.fixedDeltaTime * Core.TimeScale() * animationSpeed) % 1f;
        UpdateVisualEffects(Time.fixedDeltaTime * Core.TimeScale() * animationSpeed);
        if (delta_a == delta_b)
            delta_b = delta_b == 1f ? 0f : 1f;

        if (delta_a < delta_b)
        {
            delta_a += Time.fixedDeltaTime * Core.TimeScale() * animationSpeed;
            delta_a = Mathf.Min(delta_b, delta_a);
        }
        else
        {
            delta_a -= Time.fixedDeltaTime * Core.TimeScale() * animationSpeed;
            delta_a = Mathf.Max(delta_b, delta_a);
        }
        UpdateLimbsColor();
    }

    void UpdateLimbsColor()
    {
        Vector4 color_sum = Vector4.zero;
        int color_count = 0;
        foreach (string effect in effects)
        {
            Vector4 color_plus = BodyEffects.GetEffectColor(effect);
            if (color_plus != Vector4.zero)
            {
                color_sum += color_plus;
                color_count++;
            }
        }
        if (color_count == 0)
        {
            SetColor(color);
            return;
        }
        color_sum /= color_count;
        color_sum = ((Vector4)color) * (1f - 0.1f * delta_a) + color_sum * 0.1f * delta_a;
        SetColor((Color)color_sum);
    }


    bool tremble, sway, slug;

    float GetSlug() { return slug ? 0.75f : 1f; }

    #region Sway
    float swayTimer = 0f, swaySign = 1f;
    float GetSway()
    {
        if (!sway)
            return 0;
        return swayTimer;
    }
    void DoSway(float delta_time)
    {
        if (!sway)
            return;

        swayTimer += swaySign * delta_time * 3;
        if (swayTimer < -3f || swayTimer > 3f)
        {
            swaySign = -swaySign;
            swayTimer += swaySign * delta_time;
        }

    }

    #endregion Sway

    #region Tremble
    float trembleTimer = 0;

    Vector2[] tremblePosition;
    public void SetEffects(string[] effects)
    {
        tremble = sway = slug = false;
        foreach (string effect in effects)
        {
            switch (effect)
            {
                case "Weak": slug = true; break;
                case "Hot": tremble = true; break;
                case "Sick": sway = true; break;
            }
        }
    }

    public void UpdateVisualEffects(float delta_time)
    {
        DoSway(delta_time);

        trembleTimer -= delta_time;
        if (trembleTimer < 0)
        {
            trembleTimer = Random.Range(0.01f, 0.05f);
            for (int i = 0; i < limbs.Length; i++)
                if (!tremble)
                    tremblePosition[i] = Vector2.zero;
                else if (i != 5 && i != 6) // не ноги
                    tremblePosition[i] = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.15f, 0.15f));
                else
                    tremblePosition[i] = new Vector2(Random.Range(-0.25f, 0.25f), 0);
        }
    }
    #endregion Tremble


    #endregion


    #region Equipment
    public enum BodyPart
    {
        LFoot = 1, RFoot = 2, Legs = 0, Chest = 3, LHand = 4, RHand = 5, Head = 6
    }
    public static string PartType(BodyPart part){
        switch (part)
        {
            case BodyPart.RFoot:
            case BodyPart.LFoot: return "Feet";
            case BodyPart.RHand:
            case BodyPart.LHand: return "Hands";
            case BodyPart.Chest: return "Chests";
            case BodyPart.Legs: return "Legs";
            default:
            case BodyPart.Head: return "Heads";
        }
    }
    public enum EquipmentType
    {
        LInHand, RInHand, Belt, Neck,
        Greaves, Plate, LBracer, RBracer, Helmet, FCloak, BCloak,
        LBoot, RBoot, Pants, Shirt, LGlove, RGlove, Headwear
    }

    public void EquipmentUpdate()
    {

    }
    
    #endregion Equipment
}