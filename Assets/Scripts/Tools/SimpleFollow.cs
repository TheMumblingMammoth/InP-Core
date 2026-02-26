using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 distance; 

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        transform.position = target.position + distance; // следование за целью камеры
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
