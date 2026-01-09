using Unity.Netcode;
using UnityEngine;

public class PlayerControlUnit : NetworkBehaviour
{
    public static PlayerControlUnit proxy { get; private set; }
    [SerializeField] float speed = 1f; // скорость юнита
    [SerializeField] float size = 3f;
    Vector2 direction; // вектор направления движения
    SpriteRenderer sprite;
    [SerializeField] private Vector3Int pos;
    [SerializeField] public int z = World.chunkSize.z / 2;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (proxy == null)
        {
            proxy = this;
            Camera.main.GetComponent<SimpleFollow>().SetTarget(transform);
            Debug.Log("Owner spawned");
        }
    }
    void FixedUpdate()
    {
        Vector2 aim = (Vector2)transform.position + direction;
        float step = speed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1) * Time.fixedDeltaTime;
        Vector2 new_pos = Vector2.MoveTowards(transform.position, aim, step); // поправка скорости с проверкой нажатого шифта и кол-ва фпс
        Vector2 new_front = Vector2.MoveTowards(new_pos, aim, size/2);
        if (World.GetBlock(new_front, z) != Block.Air)
        {
            aim = World.GetClosestToBlock((Vector2)transform.position, new_front);
            transform.position = Vector2.MoveTowards(aim, transform.position, size/2);
        }
        else if (World.GetBlock(new_pos, z) != Block.Air)
        {
            aim = World.GetClosestToBlock((Vector2)transform.position, new_pos);
            transform.position = Vector2.MoveTowards(aim, transform.position, size/2);
        }
        else
        {
            transform.position = new_pos;
            //transform.position = Vector2.MoveTowards(transform.position, new_pos, -size);
        }
        pos = new Vector3Int((int)transform.position.x, (int)transform.position.y, z);
        Rebound();
        pos = new Vector3Int((int)transform.position.x, (int)transform.position.y, z);
        sprite.sortingOrder = (int)(-transform.position.y * 32 + transform.position.x);
    }
    void Rebound()
    {
        float x = transform.position.x - pos.x;
        float y = transform.position.y - pos.y;
        float fx = 0, fy = 0;
        
        if ((World.GetNBlock(pos) != Block.Air) && size > 1 - y) fy += 1 - size - y;
        if ( (World.GetSBlock(pos) != Block.Air) && size > y) fy += size - y;
        if ( (World.GetEBlock(pos) != Block.Air) && size > 1 - x)   fx += 1 - size - x;
        if ( (World.GetWBlock(pos) != Block.Air) && size > x)       fx += size - x;

        if ((World.GetNEBlock(pos) != Block.Air) && size > 1 - y && size > 1 - x)
        {
            fy += (1 - size - y)/2;
            fx += (1 - size - x)/2;
        }
        if ((World.GetNWBlock(pos) != Block.Air) && size > 1 - y && size > x)
        {
            fy += (1 - size - y)/2;
            fx += (size - x)/2;
        }
        if ((World.GetSEBlock(pos) != Block.Air) && size > y && size > 1 - x)
        {
            fy += (size - y)/2;
            fx += (1 - size - x)/2;
        }
        if ((World.GetSWBlock(pos) != Block.Air) && size > y && size > x)
        {
            fy += (size - y)/2;
            fx += (size - x)/2;
        }

        Vector2 reb = new Vector2(fx, fy);
        float speed = this.speed * (fx * fx + fy * fy) / Mathf.Pow(size, 2);
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + reb*10, speed*Time.fixedDeltaTime);
    }

    void Update()
    {
        if (!IsOwner)
            return;
        direction = GetDirection();
    }

    Vector2 GetDirection()
    {
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.W)) y = 100;
        if (Input.GetKey(KeyCode.S)) y = -100;
        if (Input.GetKey(KeyCode.A)) x = -100;
        if (Input.GetKey(KeyCode.D)) x = 100;
        return new Vector2(x, y);
    }


    #region StaticMethods
    // Передаём точку и радиус, определяем, синглтон персонаж в этом радиусе или нет
    public static bool InRange(Vector2 point, float radius) { return Vector2.Distance(proxy.transform.position, point) < radius; }
        public static float GetX() { return proxy.transform.position.x; }
    #endregion

}
