using UnityEngine;

public class PlayerControlUnit : MonoBehaviour
{

    void Start()
    {

    }
    [SerializeField] float speed = 1f;

    Vector2 direction;
    void FixedUpdate()
    {
        transform.position = transform.position + (Vector3)direction * speed * (Input.GetKey(KeyCode.LeftShift) ? 2: 1)* Time.deltaTime;
    }

    void Update()
    {
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.W)) y = 1;
        if (Input.GetKey(KeyCode.S)) y = -1;
        if (Input.GetKey(KeyCode.A)) x = -1;
        if (Input.GetKey(KeyCode.D)) x = 1;
        direction = new Vector2(x, y);
    }

}
