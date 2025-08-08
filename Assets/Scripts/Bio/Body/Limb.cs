using UnityEngine;
public class Limb : MonoBehaviour{
    public string type;
    [SerializeField] SpriteRenderer sprite;
    public Vector2 pos {get; private set;}
    void Awake()
    {
        pos = transform.localPosition;
    }
    public void SetPos(Vector2 pos)
    {
        this.pos = pos;
        transform.localPosition = pos;
    }
    public void SetSkin(int skinID)
    {
        if (type == "")
            return;
        if (BodyClip.skins.HasClipFor(type))
            sprite.sprite = BodyClip.skins.ClipFor(type).frames[skinID].pic;
    }
    public void SetOrder(int order){
        switch(type){
            case "Heads": order += 4; break;
            case "Hands": order += 5; break;
            case "Feet": order += 1; break;
            case "Legs": order += 3; break;
            case "Chests": order += 2; break;
        }
        sprite.sortingOrder = order;
    }
    public void SetColor(Color color){  sprite.color = color;   }
    void Update(){
        //transform.localPosition = pos;
        //transform.localPosition = Vector2.MoveTowards(transform.localPosition, pos, speed * Time.deltaTime);
    }

   
}