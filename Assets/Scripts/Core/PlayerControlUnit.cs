using UnityEngine;

public class PlayerControlUnit : MonoBehaviour
{
    public static PlayerControlUnit proxy { get; private set; }
    [SerializeField] float speed = 1f;
    Vector2 direction;

    void Awake()
    {
        proxy = this;
    }
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direction,
                                     speed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1) * Time.deltaTime);
    }

    void Update()
    {
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.W)) y = 100;
        if (Input.GetKey(KeyCode.S)) y = -100;
        if (Input.GetKey(KeyCode.A)) x = -100;
        if (Input.GetKey(KeyCode.D)) x = 100;
        direction = new Vector2(x, y);
    }

    #region StaticMethods
        // Передаём точку и радиус, определяем, синглтон персонаж в этом радиусе или нет
        public static bool InRange(Vector2 point, float radius){   return Vector2.Distance(proxy.transform.position, point) < radius;    }

    #endregion

}
