using UnityEngine;



/*
Полёт снаряда.

*/

public class Projectile : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision) 
    {
        // уничтожаем снаряд при любом столкновении 
        Destroy(gameObject);
    }
}