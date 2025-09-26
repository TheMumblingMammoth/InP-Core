using UnityEngine;

[System.Serializable]


/*

- Описывает полёт снаряда. 
- Сейчас есть 2 типа траекторий: прямо и по параболе - они заданы уравнениями.
- Движение равноускорено, но саму скорость можно изменить.
- Снаряды удаляются(исчезают, уничтожаются), когда выходят за пределы видимости игрока.


*/



public class ProjectileTrajectory : MonoBehaviour
{
    [Header("Основные параметры")]
    public ProjectileType type = ProjectileType.Linear;
    public float speed = 10f;
    public float parabolaHeight = 5f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 initialDirection;
    private float startTime;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;   // копия камеры для выявления пределов видимости
    }

    public void Initialize(Rigidbody2D rigidbody, Vector2 direction)
    {
        rb = rigidbody; //Rigidbody2D снаряда - для отдельных параметров 
        
        startPosition = rb.position;    
        initialDirection = direction.normalized;
        startTime = Time.time;  //начальное время
    }

    void FixedUpdate()
    {
        float t = Time.time - startTime;    // прошедшее время с начала движения - из текущего момента времени отнимаем время старта

        switch (type)   
        {
            case ProjectileType.Linear: // если равноускоренное прямолинейное движение:
                Vector2 linearPos = startPosition + initialDirection * speed * t;   
                rb.MovePosition(linearPos);
                break;

            case ProjectileType.Parabolic:  //если движение по параболе:
                float horizontalDistance = speed * t;
                float verticalDistance = parabolaHeight * t - 4.9f * t * t;
                Vector2 parabolicOffset = new Vector2(horizontalDistance, verticalDistance);
                Vector2 parabolicPos = startPosition + RotateVector(parabolicOffset, initialDirection);
                rb.MovePosition(parabolicPos);
                break;
                
        /*
            + добавить другие траектории полёта             
            
        */
                
        }

        CheckVisibility(); // проверка: остаётся ли снаряд в зоне видимости
    }

    private void CheckVisibility()
    {
        Vector2 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        /*
        WorldToViewportPoint преобразует координаты снаряда в общем ппространстве в нормализованные координаты для камеры
        , где (0,0) — левый нижний угол камеры, а (1,1) — правый верхний угол камеры. 
        
        Тогда нахождение объекта в пределах [0,1] => объект в зоне видимости
        */
        
        if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f)
        {
            Destroy(gameObject);    // уничтожается объект, выходящтй за пределы видимости
        }
    }

        // поворот снаряда относительно его центра. vector - исходный вектор(прим: по параболе), direction - направление(нормализир) 
    private Vector2 RotateVector(Vector2 vector, Vector2 direction)
    {   
        float angle = Mathf.Atan2(direction.y, direction.x); // угол поворота как арктангенс относительно направления
        //return vector.Rotate(angle); // Встроенный метод поворота вектора на угол
        
        
        return new Vector2(
            vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle),
            vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle)
        );
        
        //по матрице поворотов в 2Д: 
        //[ x' ] =  [ cos(angle)  -sin(angle) ] [ x ]
        //[ y' ]    [ sin(angle)   cos(angle) ] [ y ]
        
        
        
         
    }
}