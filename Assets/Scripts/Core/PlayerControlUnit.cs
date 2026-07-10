using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControlUnit : NetworkBehaviour
{
    public static PlayerControlUnit proxy { get; private set; }
    [SerializeField] float speed = 1f; // скорость юнита
    [SerializeField] float size = 3f;
    Vector3 direction; // вектор направления движения
    
    [SerializeField] private Vector3Int pos;
    [SerializeField] Body body;
    void Awake()
    {
        if (proxy == null)
        {
            proxy = this;
            Camera.main.GetComponent<SimpleFollow>().SetTarget(transform);
            Debug.Log("Owner spawned");
        }
    }
    void Start()
    {
        pos = new Vector3Int(5, 5, World.main.GetHighestBlock(new Vector2Int(5,5))+ 1);
        transform.position = pos;        
    }
    void UpdateBodyAnimation()
    {
        if (direction != Vector3.zero)
        {
            body.ChangeState(Input.GetKey(KeyCode.LeftShift) ? "Run" : "Walk");
        }
        else
        {
            body.ChangeState("Stand");
        }
        if(body.transform.localScale.x < 0 && direction.x < 0)
            body.transform.localScale = new Vector3(1, 1, 1);
        if(body.transform.localScale.x > 0 && direction.x > 0)
            body.transform.localScale = new Vector3(-1, 1, 1);
        if(body.flipY && direction.y < 0)
            body.FlipY();
        if(!body.flipY && direction.y > 0)
            body.FlipY();
    }

    void FixedUpdate()
    {
        
        Vector3 aim = transform.position + direction;
        float step = speed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1) * Time.fixedDeltaTime;
        Vector3 new_pos = Vector3.MoveTowards(transform.position, aim, step); // поправка скорости с проверкой нажатого шифта и кол-ва фпс
        Vector3 new_front = Vector3.MoveTowards(new_pos, aim, pos.z);
        
        UpdateBodyAnimation();        
        
        if (World.GetBlock(new_front, pos.z) != null && World.GetBlock(new_front, pos.z).HasBlock())
        {
            aim = World.GetClosestToBlock(transform.position, new_front);
            transform.position = Vector3.MoveTowards(aim, transform.position, pos.z);
        }
        else if (World.GetBlock(new_pos, pos.z) != null && World.GetBlock(new_pos, pos.z).HasBlock())
        {
            aim = World.GetClosestToBlock(transform.position, new_pos);
            transform.position = Vector3.MoveTowards(aim, transform.position, pos.z);
        }
        else
        {
            transform.position = new Vector3(new_pos.x, new_pos.y, pos.z);
            //transform.position = Vector2.MoveTowards(transform.position, new_pos, -size);
        }        


        pos = CalcPosition(); //new Vector3Int((int)transform.position.x, (int)transform.position.y, z);
        Rebound();
        pos = CalcPosition();//new Vector3Int((int)transform.position.x, (int)transform.position.y, z);
        //IsoGrid.SetFocus(pos - new Vector3Int(0, 0, 1));
        
    }

    Vector3Int CalcPosition()
    {
        Vector2 transPos = transform.position;
        int J = (int)(transPos.x / IsoGridTile.TileSizeX);
        int I = (int)(transPos.y / IsoGridTile.TileSizeY + (J % 2 == 0 ? 0 : 0.5f));


        float dx = transPos.x - J * IsoGridTile.TileSizeX;
        float dy = transPos.y - I * IsoGridTile.TileSizeY - (J % 2 == 0 ? 0 : IsoGridTile.TileSizeY / 2) - 0.5f;
         
        //if(dx == 0 || dy == 0)
            return new Vector3Int(J, I, pos.z);
        if (dx > 0 && dy > 0)
        {
            if(IsoGridTile.TileSizeX - dx > 2 * dy)
                return IsoGrid.GoNE(new Vector3Int(J, I, pos.z));
            else 
                new Vector3Int(J, I, pos.z);
        }
        if (dx < 0 && dy > 0)
        {
            if (IsoGridTile.TileSizeX + dx > 2 * dy)
                return IsoGrid.GoNW(new Vector3Int(J, I, pos.z));
            else 
                new Vector3Int(J, I, pos.z);
        }
        if (dx > 0 && dy < 0)
        {
            if (IsoGridTile.TileSizeX - dx > -2 * dy)
                return IsoGrid.GoSE(new Vector3Int(J, I, pos.z));
            else
                new Vector3Int(J, I, pos.z);
        }
        if (dx < 0 && dy < 0)
        {
            if (IsoGridTile.TileSizeX + dx > -2 * dy)
                return IsoGrid.GoSW(new Vector3Int(J, I, pos.z));
            else
                new Vector3Int(J, I, pos.z);
        }
        return new Vector3Int(J, I, pos.z);
    }

    void Rebound()
    {
        float x = transform.position.x - pos.x;
        float y = transform.position.y - pos.y;
        float fx = 0, fy = 0;


        
        if ( World.GetNBlock(pos) != null && World.GetNBlock(pos).HasBlock() && size > 1 - y) fy += 1 - size - y;
        if ( World.GetSBlock(pos) != null && World.GetSBlock(pos).HasBlock() && size > y) fy += size - y;
        if ( World.GetEBlock(pos) != null && World.GetEBlock(pos).HasBlock() && size > 1 - x)   fx += 1 - size - x;
        if ( World.GetWBlock(pos) != null && World.GetWBlock(pos).HasBlock() && size > x)       fx += size - x;

        if ( World.GetNEBlock(pos) != null && World.GetNEBlock(pos).HasBlock() && size > 1 - y && size > 1 - x)
        {
            fy += (1 - size - y)/2;
            fx += (1 - size - x)/2;
        }
        if ( World.GetNWBlock(pos) != null && World.GetNWBlock(pos).HasBlock() && size > 1 - y && size > x)
        {
            fy += (1 - size - y)/2;
            fx += (size - x)/2;
        }
        if ( World.GetSEBlock(pos) != null && World.GetSEBlock(pos).HasBlock() && size > y && size > 1 - x)
        {
            fy += (size - y)/2;
            fx += (1 - size - x)/2;
        }
        if ( World.GetSWBlock(pos) != null && World.GetSWBlock(pos).HasBlock() && size > y && size > x)
        {
            fy += (size - y)/2;
            fx += (size - x)/2;
        }

        Vector3 reb = new Vector3(fx, fy, transform.position.z);
        float speed = this.speed * (fx * fx + fy * fy) / Mathf.Pow(size, 2);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + reb*10, speed*Time.fixedDeltaTime);
    }
    int id = 1;
    void Update()
    {
        if (!IsOwner)
            return;
        direction = GetDirection();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            id = (id + 1) % 2;
            body.SetSkin(id);
        }
    }

    Vector2 GetDirection()
    {
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.W)) y = 100;
        if (Input.GetKey(KeyCode.S)) y = -100;
        if (Input.GetKey(KeyCode.A)) x = -100;
        if (Input.GetKey(KeyCode.D)) x = 100;
        return new Vector3(x, y, transform.position.z);
    }


    #region StaticMethods
    // Передаём точку и радиус, определяем, синглтон персонаж в этом радиусе или нет
    public static bool InRange(Vector2 point, float radius) { return Vector2.Distance(proxy.transform.position, point) < radius; }
        public static float GetX() { return proxy.transform.position.x; }
    #endregion

}
