using UnityEngine;
public class Body : MonoBehaviour{
    // Unit per Pixel
    const float UPP = 1f/32f;
    [SerializeField] Limb[] limbs;
    [SerializeField] Vector2 [] originPositions;
    [SerializeField] float [] originRotations;
    
    [ContextMenu("SetOrigin")]
    void SetOrigin(){
        originPositions = new Vector2[limbs.Length];
        originRotations = new float[limbs.Length];
        for(int i = 0; i < limbs.Length; i++){
            originPositions[i] = (Vector2)limbs[i].transform.localPosition;
            originRotations[i] = limbs[i].transform.localRotation.eulerAngles.z;
        }
    }

    [ContextMenu("Reset")]
    void Reset(){
        for(int i = 0; i < limbs.Length; i++){
            limbs[i].transform.localPosition = originPositions[i];
            Quaternion q = new Quaternion(0, 0, 0, 1f);
            q.eulerAngles = new Vector3(0, 0, originRotations[i]);
            limbs[i].transform.localRotation = q;
        }
    }
#region Animation
    
    [ContextMenu("Print Frame")]
    void PrintFrame(){
        Debug.Log(SnapFrame().ToString());
    }
    BodyFrame SnapFrame(){
        //Quaternion q;
        BodyFrame frame = new BodyFrame(limbs.Length);
        for(int i = 0; i < limbs.Length; i++){
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
    void NextFrame(){
        timer -= time;
        previous = bodyClip.GetFrameByNumber(frameI);
        frameI++;            
        if(frameI >= bodyClip.size){
            if(!Casual())
                ChangeState(old_state);        
            frameI = 0;
        }
        current = bodyClip.GetFrameByNumber(frameI);
        time = bodyClip.time[frameI];
    }
    void ApplyClip(){
        while(timer >= time)
            NextFrame();    
        BodyFrame frame = previous.MoveTo(current, timer/time);

        for(int i = 0; i < limbs.Length; i++)
        {
            limbs[i].SetPos(originPositions[i] + frame.GetPosition(i) * UPP);
            // Vector2.MoveTowards(limbs[i].pos, originPositions[i] + frame.GetPosition(i) * UPP, Core.TimeScale() * Time.deltaTime));
            /*
            limbs[i].SetPos(Vector2.MoveTowards(originPositions[i] + previous.GetPosition(i) * UPP,
                                                    originPositions[i] + frame.GetPosition(i) * UPP,
                                                    timer));
            */
            Quaternion q = new Quaternion(0, 0, 0, 1f);
            
            /*
            float angle = limbs[i].transform.localRotation.eulerAngles.z;
            if (angle > 180)
                angle -= 360;
            bool fl = angle < originRotations[i] + frame.GetRotation(i);
            angle = Core.TimeScale() * 600f * Time.deltaTime * (fl ? 1 : - 1);
            angle = fl ? Mathf.Min(angle,  originRotations[i] + frame.GetRotation(i)) 
                        :Mathf.Max(angle,  ));
            */
            q.eulerAngles = new Vector3(0, 0, originRotations[i] + frame.GetRotation(i));
            limbs[i].transform.localRotation = q;
        }
    }      
    #endregion Animation
    
    [SerializeField] public string state = "Stand"; 
    string old_state = "";
    float speed = 1f;
    
    public float GetProgress(){ return timer / bodyClip.Length();   }
    void Start(){
        ChangeState(state);
        SetSkin(skinID);
    }
    int order = 0;

    void FixedUpdate(){
        if(speed == 0)  return; 
        timer += Time.fixedDeltaTime * Core.TimeScale() * speed;
        UpdateStatus();
        ApplyClip();
        if(order != -(int)(transform.position.y * 1000) + skinID)
        {
            order = -(int)(transform.position.y * 1000) + skinID;
            foreach(Limb limb in limbs)
                limb.SetOrder(order);
        }
    }
    public void ChangeState(string state){
        if(bodyClip != null && this.state == state)
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

    public void PlayOnce(string newClip){
        previous = SnapFrame();
        old_state = state;
        state = newClip;
        timer = 0;
        bodyClip = BodyClip.clips[state];
        frameI = 0;
        time = 0.125f;
        current = bodyClip.GetFrameByNumber(0);
    }

    public bool Casual() { return old_state == "";}

    public void SetColor(Color color, bool onlyMain = false){
        for(int i = 0; i < limbs.Length; i++){
            if(onlyMain && i == 2)
                return;
            limbs[i].SetColor(color);
        }
    }    

    public void Right(){    transform.localScale = new Vector3(-1, 1, 1);   }
    public void Left(){    transform.localScale = new Vector3(1, 1, 1);   }

    #region Skins
        public bool child;
        public int skinID;
        [ContextMenu("SetSkin")]
        public void SetSkin(){
            SetSkin(skinID);
        }
        public void SetSkin(int skinID){
            this.skinID = skinID;
            foreach(Limb limb in limbs)
                limb.SetSkin(skinID);
        }
        public Sprite GetHead(){
            return Resources.Load<Sprite>("Sprites/Heads/Heads" + (skinID + 1).ToString());
        }
    #endregion 
    [SerializeField] float k = 2f;
    #region Colors
    [SerializeField] string status = "Healthy";
    [SerializeField] Color color = new Color(182f/255f, 144f/255f, 106f/255f, 1f);
    float status_a, status_b; 
    public void ChangeStatus(string status){
        this.status = status;
        status_a = 0f;
    }

    void UpdateStatus()
    {
        foreach (Limb limb in limbs)
        {
            limb.SetColor(color);
        }
        return;
        if (status_a == status_b)
        status_b = status_b == 1f ? 0.5f : 1f;

        float speed = (status == "Migrane" ? 1f : 0.2f) * Time.fixedDeltaTime * Core.TimeScale();
        if(status_a < status_b){
            status_a += speed ;
            status_a = Mathf.Min(status_b, status_a);
        }else{
            status_a -= speed;
            status_a = Mathf.Max(status_b, status_a);
        }

        switch(status){
            case "Healthy": 
                limbs[0].SetColor(Color.white);
                break;
            case "Hot": 
                limbs[0].SetColor(new Color(1f, 1f - status_a/k, 1f - status_a/k));
                break;
            case "Weak": 
                limbs[0].SetColor(new Color(1f - status_a/k, 1f - status_a/k, 1f - status_a/(k*2)));
                break;
            case "Migrane": 
                limbs[0].SetColor(new Color(1f, 1f - status_a/k, 1f - status_a/k));
                break;
            case "Sick": 
                limbs[0].SetColor(new Color(1f - status_a/k, 1f, 1f - status_a/k));
                break;
        }
    }
    #endregion Colors
}