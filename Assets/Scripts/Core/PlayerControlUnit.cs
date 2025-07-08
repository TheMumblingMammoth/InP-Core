using UnityEngine;

public class PlayerControlUnit : MonoBehaviour
{

    void Start()
    {

    }
    [SerializeField] float speed = 1f;

    void FixedUpdate()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + new Vector3(0f, 1f, 0f) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + new Vector3(0f, -1f, 0f) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + new Vector3(-1f, 0f, 0f) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + new Vector3(1f, 0f, 0f) * speed * Time.deltaTime;
        }
    }

}
