using UnityEngine;

public class Raindrop : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] float speed = 1f;
    public Vector2 spot;
    float dist = 3f;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -(int)(spot.y * 1000);
    }

    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, spot, speed * Time.fixedDeltaTime);
        if((Vector2)transform.position == spot)
        {
            Splash();
        }
    }

    [SerializeField] Splash sample_droplet;
    void Splash()
    {
        for(int i = 0; i < 5; i++){
            Splash droplet = Instantiate(sample_droplet);
            droplet.transform.position = transform.position;
            droplet.gameObject.SetActive(true);
            droplet.spot0 = droplet.transform.position;
            
            float angle = Random.Range(0, 360);
            droplet.spot1 = (Vector2)transform.position + new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        }
        Destroy(gameObject, 0.1f);
        enabled = false;
    }
}