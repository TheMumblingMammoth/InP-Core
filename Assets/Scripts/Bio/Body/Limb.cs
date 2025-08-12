using UnityEngine;
public class Limb : MonoBehaviour
{
    public Body.BodyPart type;
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
    public void SetSkin(int skinID) {   sprite.sprite = BodyClip.skins.ClipFor(Body.PartType(type)).frames[skinID].pic; }
    public void SetOrder(int order) {   sprite.sortingOrder = order + (int)type;    }
    public void SetColor(Color color){  sprite.color = color;   }
    void Update()
    {
        //transform.localPosition = pos;
        //transform.localPosition = Vector2.MoveTowards(transform.localPosition, pos, speed * Time.deltaTime);
    }

   
}