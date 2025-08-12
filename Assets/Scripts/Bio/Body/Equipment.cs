using UnityEngine;
public class Equipment : MonoBehaviour
{
    public Body.EquipmentType type;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteRenderer color_sprite;
    public Vector2 pos { get; private set; }
    void Awake()
    {
        pos = transform.localPosition;
    }
    public void SetPos(Vector2 pos)
    {
        this.pos = pos;
        transform.localPosition = pos;
    }
    
    public void SetOrder(int order) {   sprite.sortingOrder = order + (int)type;    }
    public void SetColor(Color color){  sprite.color = color;   }
    void Update()
    {
        //transform.localPosition = pos;
        //transform.localPosition = Vector2.MoveTowards(transform.localPosition, pos, speed * Time.deltaTime);
    }

   
}