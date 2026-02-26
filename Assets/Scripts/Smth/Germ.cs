using UnityEngine;

public class Germ : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] float speed = 1f;
    float dist = 3f;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        speed = global_speed;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.b, spriteRenderer.color.g, alpha);
        transform.localScale  = new Vector3(Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f), 1f);
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  
        if (Input.GetMouseButton(0) && Vector2.Distance(pos, transform.position) < dist){
            float mod = (dist - Vector2.Distance(pos, transform.position))/dist*2;
            transform.position = Vector2.MoveTowards(transform.position,
                            pos,
                            - mod * speed*Time.fixedDeltaTime);
        }
        else if (Input.GetMouseButton(1) && Vector2.Distance(pos, transform.position) < dist){
            float mod = (dist - Vector2.Distance(pos, transform.position))/dist*2;
            transform.position = Vector2.MoveTowards(transform.position,
                            pos,
                            mod * speed*Time.fixedDeltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position,
                             (Vector2)transform.position + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)),
                             speed*Time.fixedDeltaTime);
        }
    }
    static float global_speed, alpha;
    public static void SetSpeed(float speed)
    {
        Germ.global_speed = speed;
    }

    public static void SetAlpha(float alpha)
    {
        Germ.alpha = alpha;
    }
}