using UnityEngine;
public class Limb : MonoBehaviour{
    public string type;
    [SerializeField] SpriteRenderer sprite;
    public Vector2 pos {get; private set;}
    float speed = 1f;
    void Awake(){
        pos = transform.localPosition;
    }
    public void SetPos(Vector2 pos){
        this.pos = pos;
        transform.localPosition = pos;
    }
    public void SetSkin(int skinID, bool child)
    {
        if (type == "")
            return;
        if (!child)
        {
            if (BodyClip.skins.HasClipFor(type + "s"))
                sprite.sprite = BodyClip.skins.ClipFor(type + "s").frames[skinID].pic;
        }
        else
        {
            if (BodyClip.childSkins.HasClipFor(type + "s"))
                sprite.sprite = BodyClip.childSkins.ClipFor(type + "s").frames[skinID].pic;
        }
    }
    public void SetOrder(int order){
        switch(type){
            case "Head": order += 3; break;
            case "Hand": order += 4; break;
            case "Leg": order += 1; break;
            case "Torso": order += 2; break;
        }
        sprite.sortingOrder = order;
    }
    public void SetColor(Color color){  sprite.color = color;   }
    void Update(){
        //transform.localPosition = pos;
        //transform.localPosition = Vector2.MoveTowards(transform.localPosition, pos, speed * Time.deltaTime);
    }

   
}