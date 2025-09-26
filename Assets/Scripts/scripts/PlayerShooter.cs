using UnityEngine;
/*

Настройки игрока:
1. Параметры, необходимые для полёта снаряда(начальное положение и тд)
2. Задержка между выстрелами, Вызов снаряда
3. Выстрел снаряда


*/
public class PlayerShooter : MonoBehaviour
{
    [Header("Основные настройки")]
    public GameObject projectilePrefab; //
    public Transform firePoint;
    public float shootCooldown = 0.5f;

    [Header("Настройки траектории")]
    public ProjectileType trajectoryType;
    public float projectileSpeed = 10f;
    public float parabolaHeight = 4f;

    private float lastShootTime;    //время окончания жизни снаряда

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // цель-точка фиксированная нажатием ЛКМ
        Vector2 shootDirection = (targetPosition - (Vector2)firePoint.position).normalized; //Направление до цели


// инициализация объекта снаряда: Префаб(образ) , точка начала и начальная ориентация(Quaternion.identity,без поворота)
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
// настройка траектории: копируем траекторию из созданного снаряда
        ProjectileTrajectory trajectory = projectile.GetComponent<ProjectileTrajectory>();
    
        if (trajectory == null)
        {
            trajectory = projectile.AddComponent<ProjectileTrajectory>();
        }
        trajectory.type = trajectoryType;
        trajectory.speed = projectileSpeed;
        trajectory.parabolaHeight = parabolaHeight;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = projectile.AddComponent<Rigidbody2D>();
            //rb.gravityScale = 0f;
        }
        
        trajectory.Initialize(rb, shootDirection);  // инициализируем траекторию полёта снаряда
                                                    //shootDirection - Направление до цели
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); 
    }
}