using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerControlUnit : NetworkBehaviour
{
    public static PlayerControlUnit proxy { get; private set; }
    [SerializeField] float speed = 1f; // скорость юнита
    [SerializeField] float size = 3f;
    Vector3 direction; // вектор направления движения
    
    [SerializeField] private Vector3Int pos;
    [SerializeField] Body body;
    [SerializeField] public Vector3Int targetTile;// {get; private set;}
    void Awake()
    {
        if (proxy == null)
        {
            proxy = this;
            Camera.main.GetComponent<SimpleFollow>().SetTarget(body.transform);
            Camera.main.GetComponent<SimpleFollow>().SetSpeed(speed*2);
            Debug.Log("Owner spawned");
        }
    }
    void Start()
    {
        pos = new Vector3Int(5, 5, (int)World.GetBlockZ(new Vector2Int(5, 5)));
        float z = World.GetBlockZ((Vector2Int)pos);
        Debug.Log("Starting pos " + pos);
        transform.position = new Vector3(pos.x * IsoGridTile.TileSizeX, pos.y * IsoGridTile.TileSizeY, z);
    }
    void UpdateBodyAnimation()
    {
        body.transform.localPosition = new Vector2(0, body.GetHeight() + transform.position.z - 1);
        body.SetOrder(IsoGrid.CalculateOrderOnGrid(IsoGrid.IsoToLin(pos - new Vector3Int(0, 0, !World.GetTBlock(pos).full ? 1 : 0) ) ) + 100);
        if(body.Casual())
        {
            if (direction != Vector3.zero)
            {
                body.ChangeState(Input.GetKey(KeyCode.LeftShift) ? "Run" : "Walk");
            }
            else
            {
                body.ChangeState("Stand");
            }
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
        GetTarget();

        Vector2 aim = transform.position + direction;
        float step = speed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1) * Time.fixedDeltaTime;
        Vector2 new_pos = Vector3.MoveTowards(transform.position, aim, step); // поправка скорости с проверкой нажатого шифта и кол-ва фпс
        Vector2 new_front = Vector3.MoveTowards(new_pos, aim, size);
                
        if (CanGo(new_front) && CanGo(new_pos))
        {
            
            transform.position = new Vector3(new_pos.x, new_pos.y, transform.position.z);
        
            float z = World.GetBlockZ(new_pos);        
            pos = IsoGrid.LinToIso(transform.position, (int)z);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
            Rebound();
            pos = IsoGrid.LinToIso(transform.position, (int)z);
            UpdateBodyAnimation();        
        }

        //  Debug.Log("FX: " + IsoGrid.LinToIso(transform.position) + " : " +  pos);
    }

    bool CanGo(Vector2 aim)
    {
        Vector3Int aimedTile = IsoGrid.LinToIso(aim);
        if(World.GetBlock(aimedTile) == null)
            return false;
        
        return Mathf.Abs(transform.position.z - World.GetBlockZ((Vector2Int)aimedTile)) < 0.7;
    }

    void Rebound()
    {
        float fx = 0, fy = 0;
        //float size = this.size*1.5f;
        Vector2 aim = transform.position;// + direction

        if ( !CanGo(new Vector2(aim.x + size, aim.y + size/2))) // NE
            {
                fy -= size/2;
                fx -= size;
            }
        
        if ( !CanGo(new Vector2(aim.x - size, aim.y + size/2))) // NW
            {
                fy -= size/2;
                fx += size;
            }
        
        if ( !CanGo(new Vector2(aim.x + size, aim.y - size/2))) // SE
            {
                fy += size/2;
                fx -= size;
            }
        
        if ( !CanGo(new Vector2(aim.x - size, aim.y - size/2))) // SW
            {
                fy += size/2;
                fx += size;
            }
        
        if ( !CanGo(new Vector2(aim.x , aim.y + size))) // N
                fy -= size;
            
        if ( !CanGo(new Vector2(aim.x , aim.y - size))) // S
                fy += size;

        if ( !CanGo(new Vector2(aim.x + size, aim.y))) // E
                fx -= size;
            
        if ( !CanGo(new Vector2(aim.x - size, aim.y))) // W
                fx += size;

        Vector3 reb = new Vector3(fx, fy, transform.position.z);
        float speed = this.speed * (fx * fx + fy * fy) / Mathf.Pow(size, 2);
        
        Vector3 reb_pos = Vector3.MoveTowards(transform.position, transform.position + reb*10, speed*Time.fixedDeltaTime);
        Vector3 reb_front = Vector3.MoveTowards(reb_pos, reb_pos + reb*10, size);
        if(CanGo(reb_pos) && CanGo(reb_front))
            transform.position = reb_pos;
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
        Click();
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
    [SerializeField] float angle;
    void GetTarget()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 center = new Vector2(body.transform.position.x, body.transform.position.y - body.GetHeight());
        if(Vector2.Distance(center, mousePos) < 1.4f)
        {
            targetTile = new Vector3Int(pos.x, pos.y, (int)(World.GetBlockZ((Vector2Int)pos) - 0.3f));
            return;    
        }
        
        angle = Vector2.SignedAngle(Vector2.up, center - mousePos);
        Vector2Int target = new Vector2Int(pos.x, pos.y);
        if(Mathf.Abs(angle) < 15)
            target += new Vector2Int(0, -1);
        
        if(angle >= 15 && angle < 75)
            target += new Vector2Int(1, 0);
        if(angle <= -15 && angle > -75)
            target += new Vector2Int(-1, -1);
        
        if(angle >= 75 && angle < 105)
            target += new Vector2Int(2, 1);
        if(angle <= -75 && angle > -105)
            target += new Vector2Int(-2, -1);

        if(angle >= 105 && angle < 165)
            target += new Vector2Int(1, 1);
        if(angle <= -105 && angle > -165)
            target += new Vector2Int(-1, 0);
        
        if(Mathf.Abs(angle) > 165)
            target += new Vector2Int(0, 1);

        targetTile = new Vector3Int(target.x, target.y, (int)(World.GetBlockZ(target)-0.3f));
    }

    #region Click
    public void Click(){
        Block blockData = World.GetBlock(targetTile);

        if (Input.GetMouseButtonDown(0))
        {
            if (!blockData.full && blockData.GetBlock() == BlockType.DirtBrick)
            {
                if(body.Casual())   body.PlayOnce("Action");
                blockData.SetFull();
            }else if(blockData.full && pos.z < World.chunkSize.z - 1){
                if(body.Casual())   body.PlayOnce("Action");
                blockData = World.GetTBlock(targetTile);
                blockData.SetBlock(BlockType.DirtBrick);
                blockData.SetCarpet(CarpetType.None);
                blockData.SetHalf();
            }
            IsoGrid.Click();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if(body.Casual())   body.PlayOnce("Action");
            if (blockData.full)
            {
                blockData.SetHalf();
            }
            else
            {
                blockData.SetEmpty();
            }   
            IsoGrid.Click();
        }
        if (Input.GetMouseButtonDown(2))
        {
            if(body.Casual())   body.PlayOnce("Action");
            World.GetTBlock(targetTile).SetBlock(BlockType.Stuff);
            IsoGrid.Click();
        }    
    }
    #endregion


    #region StaticMethods
    // Передаём точку и радиус, определяем, синглтон персонаж в этом радиусе или нет
    public static bool InRange(Vector2 point, float radius) { return Vector2.Distance(proxy.transform.position, point) < radius; }
        public static float GetX() { return proxy.transform.position.x; }
    #endregion

}
