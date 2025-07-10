using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 distance; 

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + distance; // следование за целью камеры
    }
}
