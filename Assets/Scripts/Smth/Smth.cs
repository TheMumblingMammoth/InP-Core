using UnityEngine;

public class Smth : MonoBehaviour
{
    [SerializeField] Transform container;
    public Vector2 bounds;
    [SerializeField] GameObject germ;
    void Awake()
    {
        int n = 1000;
        for(int i= 0; i < n; i++)
        {
            GameObject copy = Instantiate(germ);
            copy.transform.SetParent(container);
            copy.transform.position = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
        }
    }
}