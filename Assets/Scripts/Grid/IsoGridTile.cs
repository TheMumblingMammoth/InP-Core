using System;
using Unity.VisualScripting;
using UnityEngine;
public class IsoGridTile : MonoBehaviour
{
    public const float TileSizeX = 31/32f, TileSizeY = 1;
    [SerializeField] SpriteRenderer tile, frame;
    [SerializeField] SpriteRenderer floor, ceil, wall, wallL, wallR;
    [SerializeField] private Vector3Int pos;
    Block block;
    void Awake()
    {
        tile.enabled = false;
        frame.enabled = false;
    }
    void FixedUpdate()
    {
        if (frame.enabled && !IsMouseOver())
        {
            frame.enabled = false;
            return;
        }
        if (!frame.enabled && IsMouseOver())
        {
            frame.enabled = true;
            return;
        }
    }

    public void Upload(int x, int y, int z)
    {
        pos = new Vector3Int(x, y, z);
        Upload();

    }
    
    void Upload()
    {
        block = World.GetBlock(pos);
        if (block == null)
        {
            floor.sprite = null;
            SetWallSprite(null);
            ceil.sprite = null;
            return;
        }
        if (block.GetFloor() == BlockType.Grass)
        {
            floor.sprite = BuilderTool.proxy.grass;
        }

        
        if (block.GetWall() == BlockType.Void || block.GetWall() == BlockType.Air)
        {
            SetWallSprite(null);
        }
        else
        {
            string wall_name = block.GetWall().ToString();
            if(block.GetWall() == BlockType.Stuff)
            {
                Debug.Log("ss;");
                SetWallSprite(BuilderTool.proxy.STUFF.frames[(pos.x * 5 + pos.y * 7) % 16]);
            }
            else
            if(block.GetWall() == BlockType.Dirt)
                SetWallSprite(BuilderTool.proxy.walls.ClipFor(wall_name).frames[GetWallID()]);
            else
                SetWallSprite(BuilderTool.proxy.walls.ClipFor(wall_name).frames[0]);
        }
        
        if (block.GetCeil() == BlockType.Void || block.GetCeil() == BlockType.Air)
        {
            ceil.sprite = null;    
        }
        else
        {
            string ceil_name = block.GetCeil().ToString();
            ceil.sprite = BuilderTool.proxy.ceils.ClipFor(ceil_name).frames[0];
        }
        
        
        if(!Core.IsOrtho()){
            transform.position = new Vector3(transform.position.x, transform.position.y);
        }

        floor.sortingOrder = -(int)(transform.position.y * 1000) + pos.x;
        SetWallOrder(-(int)(transform.position.y * 1000 - 1) + pos.x + 1);
        ceil.sortingOrder = -(int)(transform.position.y - 2 * 1000) + pos.x + 1;
    }



    void Update()
    {
        if (block == null)
            return;
        if (Input.GetMouseButtonDown(0) && IsMouseOver())
        {

            if (block.GetWall() != BlockType.Air)
                block.SetWall(BlockType.Air);
            else
                block.SetWall(BlockType.Dirt);
            World.SetBlock(pos, block);
            IsoGrid.Upload();
        }
        if (Input.GetMouseButtonDown(1) && IsMouseOver())
        {

            if (block.GetWall() != BlockType.Air)
                block.SetWall(BlockType.Air);
            else
                block.SetWall(BlockType.Fachwerk);
            World.SetBlock(pos, block);
            IsoGrid.Upload();
        }
        if (Input.GetMouseButtonDown(2) && IsMouseOver())
        {

            
            block.SetWall(BlockType.Stuff);
            World.SetBlock(pos, block);
            IsoGrid.Upload();
        }
        
        
    }

    bool IsMouseOver()
    {
        return IsOver(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    bool IsOver(Vector2 aim) { 
        
        if(!IsInRect(aim))
            return false;
        float dx = aim.x - transform.position.x;
        float dy = aim.y - transform.position.y - 0.5f;
        if(dx == 0 || dy == 0)
            return true;
        if (dx > 0 && dy > 0)
            return TileSizeX - dx > 2 * dy;
        if (dx < 0 && dy > 0)
            return TileSizeX + dx > 2 * dy;
        if (dx > 0 && dy < 0)
            return TileSizeX - dx > -2 * dy;
        if (dx < 0 && dy < 0)
            return TileSizeX + dx > -2 * dy;
        return false;
    }
    bool IsInRect(Vector2 pos)
    {
        return pos.x >= transform.position.x - TileSizeX && pos.y >= transform.position.y
            && pos.x <  transform.position.x + TileSizeX && pos.y <  transform.position.y + TileSizeY;
    }    
    static bool HasWall(Block block)
    {
        if(block == null)
            return false;
        return block.GetWall() != BlockType.Air && block.GetWall() != BlockType.Void;
    }

    static bool HasCeil(Block block)
    {
        if(block == null)
            return false;
        return block.GetCeil() != BlockType.Air && block.GetCeil() != BlockType.Void;
    }

    #region Wall

    void SetWallSprite(Sprite sprite)
    {
        wall.sprite = sprite;
        wallL.sprite = sprite;
        wallR.sprite = sprite;
    }

    void SetWallOrder(int sortingOrder)
    {
        wall.sortingOrder = sortingOrder;    
        wallL.sortingOrder = sortingOrder-1;
        wallR.sortingOrder = sortingOrder-2;
    }
    
        
    #endregion

    public void SetColor(Color color)
    {
        floor.color = color;
        Debug.Log("WTF");
    }

    #region Neigh
    private int GetWallID()
    {
        if (FullIsoNeighs(pos)&&!HasWall(World.GetSBlock(pos)))
            return 9;

        if (FullIsoNeighs(pos)&&!HasWall(World.GetEBlock(pos)))
            return 10;
        
        if (FullIsoNeighs(pos)&&!HasWall(World.GetWBlock(pos)))
            return 11;
        
        if (FullIsoNeighs(pos)&&!HasWall(World.GetNBlock(pos)))
            return 12;

        if ( HasWall(World.GetNEBlock(pos))
          && HasWall(World.GetSEBlock(pos))
          &&!HasWall(World.GetNWBlock(pos))
          &&!HasWall(World.GetSWBlock(pos)))
            return 5;

        if ( HasWall(World.GetNEBlock(pos))
          && HasWall(World.GetNWBlock(pos))
          &&!HasWall(World.GetSEBlock(pos))
          &&!HasWall(World.GetSWBlock(pos)))
            return 6;

        if ( HasWall(World.GetNWBlock(pos))
          && HasWall(World.GetSWBlock(pos))
          &&!HasWall(World.GetNEBlock(pos))
          &&!HasWall(World.GetSEBlock(pos)))
            return 7;

        if ( HasWall(World.GetSEBlock(pos))
          && HasWall(World.GetSWBlock(pos))
          &&!HasWall(World.GetNEBlock(pos))
          &&!HasWall(World.GetNWBlock(pos)))
            return 8;
        
        if (HasWall(World.GetNEBlock(pos)) &&!HasWall(World.GetSWBlock(pos)))
            return 1;

        if (HasWall(World.GetNWBlock(pos)) &&!HasWall(World.GetSEBlock(pos)))
            return 2;

        if (HasWall(World.GetSEBlock(pos)) &&!HasWall(World.GetNWBlock(pos)))
            return 3;

        if (HasWall(World.GetSWBlock(pos)) &&!HasWall(World.GetNEBlock(pos)))
            return 4;

        return 0;
    }

    static bool EmptyIsoNeighs(Vector3Int pos)
    {
        return !HasWall(World.GetNEBlock(pos)) && !HasWall(World.GetNWBlock(pos))
             &&!HasWall(World.GetSEBlock(pos)) && !HasWall(World.GetSWBlock(pos));
    }
    static bool FullIsoNeighs(Vector3Int pos)
    {
        return HasWall(World.GetNEBlock(pos)) && HasWall(World.GetNWBlock(pos))
             &&HasWall(World.GetSEBlock(pos)) && HasWall(World.GetSWBlock(pos));
    }
    static bool EmptyNeighs(Vector3Int pos)
    {
        return !HasWall(World.GetNBlock(pos)) && !HasWall(World.GetWBlock(pos))
             &&!HasWall(World.GetSBlock(pos)) && !HasWall(World.GetEBlock(pos));
    }

    static bool FullNeighs(Vector3Int pos)
    {
        return HasWall(World.GetNBlock(pos)) && HasWall(World.GetWBlock(pos))
             &&HasWall(World.GetSBlock(pos)) && HasWall(World.GetEBlock(pos));
    }

    #endregion Neigh

}
