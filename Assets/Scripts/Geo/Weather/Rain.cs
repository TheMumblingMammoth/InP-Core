using UnityEngine;
using System.Collections.Generic;
public class Rain : MonoBehaviour
{
    [SerializeField] float intens = 1f;
    [SerializeField] Raindrop drop;
    public static List<Raindrop> dropPool  {get; private set;} = new List<Raindrop>(100);
    public static List<Splash> splashPool {get; private set;} = new List<Splash>(500);
    float timer = 0;
    void Awake()
    {
        
    }

    Raindrop PullInactiveDrop()
    {
        if(dropPool.Count == 0)
            return null;
        Raindrop temp = dropPool[0];
        dropPool.RemoveAt(0);
        return temp;
    }

    public static Splash PullInactiveSplash()
    {
        if(splashPool.Count == 0)
            return null;
        Splash temp = splashPool[0];
        splashPool.RemoveAt(0);
        return temp;
    }

    void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime * intens;
        while(timer < 0)
        {
            Raindrop droplet = PullInactiveDrop();
            if(droplet == null)
            {
                droplet = Instantiate(drop); 
                droplet.transform.SetParent(transform);
            }
            else
            {
                droplet.gameObject.SetActive(true);
            }  

            droplet.spot = (Vector2)Camera.main.transform.position + new Vector2(Random.Range(-8, 8),Random.Range(-5, 5)); 
            droplet.transform.position = droplet.spot + new Vector2(3,4);
            timer += 1;
        }
    }

}