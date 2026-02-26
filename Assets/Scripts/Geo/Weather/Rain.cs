using UnityEngine;

public class Rain : MonoBehaviour
{
    [SerializeField] float intens = 1f;
    [SerializeField] Raindrop drop;
    float timer = 0;
    void Awake()
    {
    
    }

    void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime * intens;
        while(timer < 0)
        {
            Raindrop droplet = Instantiate(drop);
            droplet.spot = (Vector2)Camera.main.transform.position + new Vector2(Random.Range(-8, 8),Random.Range(-5, 5)); 
            droplet.transform.position = droplet.spot + new Vector2(3,4);
            
            timer += 1;
        }
    }

}