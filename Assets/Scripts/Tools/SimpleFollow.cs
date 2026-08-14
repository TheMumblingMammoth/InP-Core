using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 distance; 
    [SerializeField] float MaxDistance = 2f;
    float speed;
    Vector3 aim;
    // Update is called once per frame
    void Update()
    {
        if(!Camera.main.orthographic){
            if (Input.mouseScrollDelta.y > 0)
                distance = distance + new Vector3(0, 0, 1);
            if (Input.mouseScrollDelta.y < 0)
                distance = distance - new Vector3(0, 0, 1);
        }
        if (target == null) return;
        aim = target.position + distance; // следование за целью камеры
        if(Vector3.Distance(aim, transform.position) < MaxDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, aim, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(aim, transform.position, MaxDistance);
        }
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void SetSpeed(float speed){ this.speed = speed; }
}
