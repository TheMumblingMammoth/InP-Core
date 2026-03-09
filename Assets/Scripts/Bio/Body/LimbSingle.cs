using UnityEngine;
public class LimbSingle : Limb
{
    [SerializeField] SpriteRenderer sprite;
    protected override void Awake()
    {
        pos = transform.localPosition;
    }
    public override void SetPos(Vector2 pos, float alpha = 0)
    {
        this.pos = pos;
        transform.localPosition = pos;
        this.alpha = alpha;
        transform.localRotation = Quaternion.Euler(0, 0, alpha);
    }
    public override Vector2 GetPos(){   return transform.localPosition; }
    public override void SetSkin(int skinID) 
    {   
        sprite.sprite = BodyClip.skins.ClipFor(Body.PartType(type).ToString() + (flipY? "Back" : "")).frames[skinID].pic; 
    }
    public override void SetOrder(int order) {   sprite.sortingOrder = order + (int)type;    }
    public override void SetColor(Color color){  sprite.color = color;   }
    void Update()
    {
        //transform.localPosition = pos;
        //transform.localPosition = Vector2.MoveTowards(transform.localPosition, pos, speed * Time.deltaTime);
    }
    public override void Flip()
    {
        sprite.flipX = !sprite.flipX;
    }
   
}