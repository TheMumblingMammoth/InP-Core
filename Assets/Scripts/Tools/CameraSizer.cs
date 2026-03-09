using UnityEngine;

public class CameraSizer : MonoBehaviour 
{
    [SerializeField] float min, max;
    [SerializeField] float speed = 1f;
    float size;
    void Awake()
    {
        size = (max + min) / 2;
        Camera.main.orthographicSize = size;
    }
    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
            size = Mathf.Min(max, size + 1);
        if (Input.mouseScrollDelta.y < 0)
            size = Mathf.Max(min, size - 1);
        Camera.main.orthographicSize =  Vector1.MoveTowards(Camera.main.orthographicSize, size, Time.deltaTime * speed);
    }
    
}
