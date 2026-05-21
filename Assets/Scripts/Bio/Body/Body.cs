using System.Collections.Generic;
using UnityEngine;
public class Body : MonoBehaviour
{
    // Unit per Pixel
    const float UPP = 1f / 32f;
    [SerializeField] Limb[] limbs;
    [SerializeField] Equipment[] equipment;
    [SerializeField] Vector2[] originPositions;
    [SerializeField] float[] originRotations;

    [ContextMenu("SetOrigin")]
    void SetOrigin()
    {
        originPositions = new Vector2[limbs.Length];
        originRotations = new float[limbs.Length];
        for (int i = 0; i < limbs.Length; i++)
        {
            originPositions[i] = limbs[i].GetPos();
            originRotations[i] = limbs[i].transform.localRotation.eulerAngles.z;
        }
    }

    public bool flipY {get; private set;}
    public void FlipY()
    {    
        flipY = !flipY;
        foreach(Limb limb in limbs)
        {
            limb.FlipY();
            limb.SetSkin(skinID);
        }
    }

    [ContextMenu("Reset")]
    void Reset()
    {
        for (int i = 0; i < limbs.Length; i++)
        {
            //Quaternion q = new Quaternion(0, 0, 0, 1f);
            //q.eulerAngles = new Vector3(0, 0, originRotations[i]);
            limbs[i].SetPos(originPositions[i], originRotations[i]);
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
            frame.Set(i, (limbs[i].GetPos() - originPositions[i]) / UPP, angle);
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
            limbs[i].SetPos(originPositions[i] + frame.GetPosition(i) * UPP, originRotations[i] + frame.GetRotation(i));
            /*Quaternion q = new Quaternion(0, 0, 0, 1f);
            q.eulerAngles = new Vector3(0, 0, originRotations[i] + frame.GetRotation(i));
            if (i == 0)
            {
                limbs[i].transform.localRotation = new Quaternion(0, 0, 0, 1f);
                limbs[i].transform.RotateAround(transform.position, Vector3.forward,
                                             originRotations[i] + frame.GetRotation(i));
            }
            else
                limbs[i].transform.localRotation = q;
            */
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
        transform.localPosition = new Vector3(0, height, 0);
        ChangeState(state);
        SetSkin(skinID);
        tremblePosition = new Vector2[limbs.Length];
    }
    int order = 0;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.CapsLock))
            animationSpeed  = (animationSpeed + 1) % 2;
    }

    void FixedUpdate()
    {
        
        if (animationSpeed == 0) return;
        timer += Time.fixedDeltaTime * Core.TimeScale() * animationSpeed;
        //UpdateDelta();
        ApplyClip();
        if (order != IsoGrid.CalculateOrderOnGrid(transform.position + new Vector3(0, -2*height, 0)) + skinID)
        {
            order = IsoGrid.CalculateOrderOnGrid(transform.position + new Vector3(0, -2*height, 0)) + skinID;
            foreach (Limb limb in limbs)
                limb.SetOrder(order);
            foreach (Equipment eq in equipment)
                eq.SetOrder(order);
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
    [SerializeField] float height;

    private int SkinID() { return 1; }

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
    public void SetSkin(){ SetSkin(skinID); }
    
    public void SetSkin(int skinID)
    {
        this.skinID = skinID;
        height = HipSize(skinID) + KneeSize(skinID);
        originPositions[0] = HeadHeight(skinID);
        originPositions[2] = new Vector2(0, - (ShoulderSize(skinID) + ForearmSize(skinID)));
        originPositions[3] = new Vector2(0, - (ShoulderSize(skinID) + ForearmSize(skinID)));
        originPositions[4] = new Vector2(0, - height);
        originPositions[5] = new Vector2(0, - height);
        //  ShoulderWidth(skinID)
        limbs[2].transform.localPosition = new Vector2(ShoulderWidth(skinID), limbs[2].transform.localPosition.y);
        limbs[3].transform.localPosition = new Vector2(-ShoulderWidth(skinID), limbs[3].transform.localPosition.y);
        limbs[4].transform.localPosition = new Vector2(HipWidth(skinID), limbs[4].transform.localPosition.y);
        limbs[5].transform.localPosition = new Vector2(-HipWidth(skinID), limbs[5].transform.localPosition.y);
          
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
        LHip=14, LKnee=10, LFoot = 18,
        RHip=16, RKnee=12, RFoot = 20,
        LShoulder=22, LForearm=26, LHand = 30,
        RShoulder=24, RForearm=28, RHand = 32,
        Torso = 0, Head = 2, Sex = 3
    }
    public static string PartType(BodyPart part){
        switch (part)
        {
            case BodyPart.RFoot:
            case BodyPart.LFoot: return "Foot";
            case BodyPart.RKnee:
            case BodyPart.LKnee: return "Knee";
            case BodyPart.RHip:
            case BodyPart.LHip: return "Hip";
            case BodyPart.RForearm:
            case BodyPart.LForearm: return "Forearm";
            case BodyPart.RShoulder:
            case BodyPart.LShoulder: return "Shoulder";
            case BodyPart.RHand:
            case BodyPart.LHand: return "Hand";
            case BodyPart.Torso: return "Torso";
            default:
            case BodyPart.Head: return "Head";
        }
    }
    public enum EquipmentType
    {
        LInHand = 20, RInHand = 10, Belt = 18, Neck = 19,
        Greaves = 5, Plate = 6, LBracer = 23, RBracer = 9, Helmet = 15, FCloak = 17, BCloak = -1,
        LBoot = 11, RBoot = 13, Pants = 3, Shirt = 4, LGlove = 22, RGlove = 8, Headwear = 16
    }

    public void EquipmentUpdate()
    {

    }
    
    #endregion Equipment


    #region Sizes

    public static Vector2 HeadHeight(int ID)
    {
        switch (ID)
        {
            case 0: return new Vector2(0, 2*UPP);
            case 1:
            case 2: return new Vector2(0, 0);
        }
        return Vector2.zero;
    }
    public static float HipWidth(int ID)
    {
        switch (ID)
        {
            case 0: 
            case 2:
                return 3*UPP;
            case 1:
                return 4*UPP;
        }
        return 0;
    }

    public static float HipSize(int ID)
    {
        switch (ID)
        {
            case 0: 
                return 10*UPP;
            case 1:
            case 2: 
                return 8*UPP;
        }
        return 0;
    }

    public static float KneeSize(int ID)
    {
        switch (ID)
        {
            case 0: 
                return 6*UPP;
            case 1:
            case 2: 
                return 5*UPP;   
        }
        return 0;
    }

    public static float ShoulderSize(int ID)
    {
        switch (ID)
        {
            case 0: 
                return 12*UPP;
            case 1: 
            case 2: 
                return 10*UPP;
        }
        return 0;
    }
    public static float ShoulderWidth(int ID)
    {
        switch (ID)
        {
            case 0: 
            case 2: 
                return 10*UPP;
            case 1: 
                return 11*UPP;
        }
        return 0;
    }

    public static float ForearmSize(int ID)
    {
        switch (ID)
        {
            case 0: 
                return 8*UPP;
            case 1:
            case 2: 
                return 7*UPP;
        }
        return 0;
    }
    #endregion Sizes
}