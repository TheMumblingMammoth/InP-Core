using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 distance; 

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
            distance = distance + new Vector3(0, 0, 1);
        if (Input.mouseScrollDelta.y < 0)
            distance = distance - new Vector3(0, 0, 1);
        if (target == null) return;
        transform.position = target.position + distance; // следование за целью камеры
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
