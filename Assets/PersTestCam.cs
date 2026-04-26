using UnityEngine;

public class PersTestCam : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vx = 0, vy = 0;
        if(Input.GetKey(KeyCode.W))
            vy = 1;
        if(Input.GetKey(KeyCode.S))
            vy = -1;
        if(Input.GetKey(KeyCode.A))
            vx = -1;
        if(Input.GetKey(KeyCode.D))
            vx = 1;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(vx, vy, 0), 1*Time.deltaTime);
    }
}
