using UnityEngine;

public class CameraSizer : MonoBehaviour 
{
    [SerializeField] float min, max;
    [SerializeField] float minFOV, maxFOV;
    [SerializeField] float speed = 1f;
    float size, FoV;
    void Awake()
    {
        size = (max + min) / 2;
        FoV = (maxFOV + minFOV) / 2;
        //mode = Camera.main.projection.
        if(Camera.main.orthographic)
            Camera.main.orthographicSize = size;
        else
            Camera.main.fieldOfView = FoV;
    }
    void Update()
    {
        if(Camera.main.orthographic)
        {
            if (Input.mouseScrollDelta.y > 0)
                size = Mathf.Min(max, size + 1);
            if (Input.mouseScrollDelta.y < 0)
                size = Mathf.Max(min, size - 1);
            Camera.main.orthographicSize =  Vector1.MoveTowards(Camera.main.orthographicSize, size, Time.deltaTime * speed);
        }
        else
        {
            if (Input.mouseScrollDelta.y > 0)
                FoV = Mathf.Min(maxFOV, FoV + 1);
            if (Input.mouseScrollDelta.y < 0)
                FoV = Mathf.Max(minFOV, FoV - 1);
            FoV =  Vector1.MoveTowards(Camera.main.fieldOfView, FoV, Time.deltaTime * speed);
            Camera.main.fieldOfView = FoV;
        }
        
    }
    
}
