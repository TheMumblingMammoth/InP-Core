using UnityEngine;

public class Splash : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] float speed = 5f, size = 1f;
    public Vector2 spot0, spot1;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    [SerializeField] float a =  1f;
    void FixedUpdate()
    {
        
        /*
        a =  -((spot0.y - h) * spot1.x / spot0.x + h - spot1.y) / (spot1.x * (spot1.x - spot0.x));
        b = - a * (spot0.x * spot0.x  - spot0.y  + h) / spot0.x;
        float x = spot0.x + (spot1.x - spot0.x) * time;
        float y = a * x * x + b * x + h;
        */

        float time = 1f - size;
        float dx = spot1.x - spot0.x, dy = spot1.y - spot0.y;
        float b = dx == 0 ? 0 : (dy - a*dx*dx)/dx;
        
        float x = dx * time;
        float y = a*x*x + b*x ;
         

        transform.position = spot0 + new Vector2(x, y);//MoveTowards(transform.position, spot, speed * Time.fixedDeltaTime);
        size -= speed * Time.fixedDeltaTime;
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 1000);
        transform.localScale = new Vector3(1f, 1f, 1f) * size;
        if(size <= 0)
            Destroy(gameObject, 0.1f);
    }
}