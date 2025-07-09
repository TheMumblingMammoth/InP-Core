using UnityEngine;

public class PlayerControlUnit : MonoBehaviour
{
    public static PlayerControlUnit proxy { get; private set; }
    [SerializeField] float speed = 1f; // скорость юнита
    Vector2 direction; // вектор направления движения

    void Awake()
    {
        proxy = this;
    }
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direction, // перевод направления в 3д(z всегда равна 0)
                                     speed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1) * Time.deltaTime); // поправка скорости с проверкой нажатого шифта и кол-ва фпс
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
