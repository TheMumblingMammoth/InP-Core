using System;
using UnityEngine;
public class Limb2Joined : Limb
{
    [SerializeField] Transform L1, L2, End;
    [SerializeField] SpriteRenderer L1Sprite, L2Sprite, EndSprite;
    [SerializeField] float a = 12/32f, b = 8/32f;
    [SerializeField] bool onLeft, flip, leg;
    
    protected override void Awake()
    {
        pos = End.localPosition;
        L1Sprite.flipX = onLeft;
        if(!leg){
            L2Sprite.flipX = flip ^ onLeft;
            EndSprite.flipX = flip ^ onLeft;
        }
    }
    public override void SetPos(Vector2 aim, float alpha = 0){
        Vector2 s = Vector2.zero, m, e = aim;
        if(Vector2.Distance(s, e) > a + b) 
            e = Vector2.MoveTowards(s, aim, a + b);
    
        float r = Vector2.Distance(s, e);
        if(r > 0.01f)
        {
            float ra = (a*a - b*b + r*r) / 2 / r;
            float h = Mathf.Sqrt(Mathf.Abs(a*a - ra*ra)) * (flip ^ (onLeft != leg) ? -1 : 1);
            m = Vector2.MoveTowards(s, e, ra);  
            m = m + Vector2.Perpendicular(e-s) / r * h; 

            m = s + (m - s)*(a / Vector2.Distance(s, m));
            e = m + (e - m)*(b / Vector2.Distance(m, e));
        }
        else
        {
            m = e;
        }
        
        

        L1.localPosition = (s + m) / 2;
        L1.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, m - s));
        L2.localPosition = (m + e) / 2;
        L2.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, e - m));

        pos = e;
        End.localPosition = e;
        this.alpha = alpha;
        End.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, e - m));
    }
    public override Vector2 GetPos(){   return (Vector2)End.localPosition; }

    public override void SetSkin(int skinID) 
    {   
        a = leg ? Body.HipSize(skinID)  : Body.ShoulderSize(skinID);
        b = leg ? Body.KneeSize(skinID) : Body.ForearmSize(skinID);
        if(leg)
        {
            //transform.localPosition = new Vector2(transform.localPosition.x, -a-b);
            End.localPosition = new Vector2(transform.localPosition.x, -a-b);
        }
        else
            transform.localPosition = new Vector2(transform.localPosition.x, b);
        L1Sprite.sprite = BodyClip.skins.ClipFor((leg? "Hip" : "Shoulder") + (flipY? "Back" : "")).frames[skinID].pic; 
        L2Sprite.sprite = BodyClip.skins.ClipFor((leg? "Knee" : "Forearm") + (flipY? "Back" : "")).frames[skinID].pic; 
        EndSprite.sprite = BodyClip.skins.ClipFor((leg? "Foot" : "Hand") + (flipY? "Back" : "")).frames[skinID].pic; 
    }
    #region Order
        int order = 0;
        public override void SetOrder(int order)    
        {
            this.order = order;
            SetOrder();
        }
        private void SetOrder()
        {   
            int order;
            if(!leg){
                order = onLeft ? 12 : 9;
                order *= flip ? -1 : 1;
                order *= flipY ? -1 : 1;
            }
            else
            {
                order = onLeft ? 3 : 6;
                order *= flipY ? -1 : 1;
                //order *= flip ? -1 : 1;
            }
            //order *= flip ? -1 : 1;
            //order *= flipY ? -1 : 1;
            L1Sprite.sortingOrder = this.order + order;
            L2Sprite.sortingOrder = L1Sprite.sortingOrder + 1 * (flipY ? -1 : 1);
            EndSprite.sortingOrder = L1Sprite.sortingOrder + 2;
        }
    #endregion Order
    public override void SetColor(Color color)
    {  
        //sprite.color = color;   
    }
    

    public override void Flip()
    {
        flip = !flip; 
        SetOrder();
        
        L2Sprite.flipX = flip ^ onLeft;
        EndSprite.flipX = flip ^ onLeft;
    }
   
}