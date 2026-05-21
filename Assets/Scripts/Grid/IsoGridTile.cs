using System;
using System.Security.Cryptography;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
public class IsoGridTile : MonoBehaviour
{
    public const float TileSizeX = 31/32f, TileSizeY = 1;
    [SerializeField] SpriteRenderer tile, frame;
    [SerializeField] SpriteRenderer blockSprite;
    [SerializeField] private Vector3Int pos;
    Block blockData;
    
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
        blockData = World.GetBlock(pos);
        if (blockData == null)
        {
            SetWallSprite(null);
            return;
        }
        

        
        if (blockData.GetBlock() == BlockType.Void || blockData.GetBlock() == BlockType.Air)
        {
            SetWallSprite(null);
        }
        else
        {
            string wall_name = blockData.GetBlock().ToString();
            if(blockData.GetBlock() == BlockType.Stuff)
            {
                Debug.Log("ss;");
                SetWallSprite(BuilderTool.proxy.STUFF.frames[(pos.x * 5 + pos.y * 7) % 16]);
            }
            else
            if(blockData.GetBlock() == BlockType.Dirt || blockData.GetBlock() == BlockType.Grass)
                SetWallSprite(BuilderTool.proxy.walls.ClipFor(wall_name).frames[GetWallID()]);
            else
                SetWallSprite(BuilderTool.proxy.walls.ClipFor(wall_name).frames[0]);
        }
        
        
        SetWallOrder(IsoGrid.CalculateOrderOnGrid(transform.position));
        
    }




    void Update()
    {
        if (blockData == null)
            return;
        if(focus)
            blockSprite.color = Color.red;
        else
        {
            if(!IsoGrid.HasClicked() && IsMouseOver())
                blockSprite.color = Color.green;
            else
            {
                blockSprite.color = Color.white;
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(pos.z < World.chunkSize.z - 1)
                World.GetTBlock(pos).SetBlock(BlockType.Grass);
            IsoGrid.Click();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if(pos.z > 0)
            World.GetBlock(pos).SetBlock(BlockType.Air);
            IsoGrid.Click();
        }
        if (Input.GetMouseButtonDown(2))
        {
            blockData.SetBlock(BlockType.Stuff);
            World.SetBlock(pos, blockData);
            IsoGrid.Click();
        }
        
        IsoGrid.Upload();
        
    }

    bool IsMouseOver()
    {
        if(HasNoWall(World.GetBlock(pos)))
            return false;
        
        if(HasNoWall(World.GetTBlock(pos)) && IsOverTop(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return true;
        
        bool se = HasNoWall(World.GetSEBlock(pos)), sw = HasNoWall(World.GetSWBlock(pos));
        if(se && IsOverSE(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return true;
        
        if(sw && IsOverSW(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return true;

        if(sw && se)
            return IsOverBottom(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        return false;
    }

    bool IsOverTop(Vector2 aim)
    {
        return IsOverBottom(aim - new Vector2(0, TileSizeY));
    }

    bool IsOverSE(Vector2 aim)
    {
        return aim.x >= transform.position.x && aim.y >= transform.position.y + TileSizeY / 2
            && aim.x <  transform.position.x + TileSizeX && aim.y <  transform.position.y + 3 / 2 * TileSizeY;
    }

    bool IsOverSW(Vector2 aim)
    {
        return aim.x >= transform.position.x - TileSizeX && aim.y >= transform.position.y + TileSizeY / 2
            && aim.x <  transform.position.x && aim.y <  transform.position.y + 3 / 2 * TileSizeY;
    }

    bool IsOverBottom(Vector2 aim) { 
        
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
    bool IsInRect(Vector2 aim)
    {
        return aim.x >= transform.position.x - TileSizeX && aim.y >= transform.position.y
            && aim.x <  transform.position.x + TileSizeX && aim.y <  transform.position.y + TileSizeY;
    }    
    static bool HasWall(Block block)
    {
        if(block == null)
            return false;
        return block.GetBlock() != BlockType.Air && block.GetBlock() != BlockType.Void;
    }
    static bool HasNoWall(Block block)
    {
        if(block == null)
            return true;
        return !HasWall(block);
    }


    #region Wall

    void SetWallSprite(Sprite sprite)
    {
        blockSprite.sprite = sprite;
    }

    void SetWallOrder(int sortingOrder)
    {
        blockSprite.sortingOrder = sortingOrder;    
    }
    
        
    #endregion

    #region Neigh
    private int GetWallID()
    {
        if(FullIsoNeighs(pos) && HasNoWall(World.GetSBlock(pos)))
            return 9;
        if(FullIsoNeighs(pos) && HasNoWall(World.GetEBlock(pos)))
            return 10;
        if(FullIsoNeighs(pos) && HasNoWall(World.GetWBlock(pos)))
            return 11;
        if(FullIsoNeighs(pos) && HasNoWall(World.GetNBlock(pos)))
            return 12;

        if(FullIsoNeighs(pos) || EmptyIsoNeighs(pos))
            return 0;
        
        if(HasWall(World.GetNEBlock(pos)) && HasWall(World.GetNWBlock(pos)) 
        && HasNoWall(World.GetSEBlock(pos)) && HasNoWall(World.GetSWBlock(pos)))
            return 5;

        if(HasWall(World.GetNEBlock(pos)) && HasWall(World.GetSEBlock(pos)) 
        && HasNoWall(World.GetNWBlock(pos)) && HasNoWall(World.GetSWBlock(pos)))
            return 6;

        if(HasWall(World.GetNWBlock(pos)) && HasWall(World.GetSWBlock(pos)) 
        && HasNoWall(World.GetNEBlock(pos)) && HasNoWall(World.GetSEBlock(pos)))
            return 7;

        if(HasWall(World.GetSEBlock(pos)) && HasWall(World.GetSWBlock(pos)) 
        && HasNoWall(World.GetNEBlock(pos)) && HasNoWall(World.GetNWBlock(pos)))
            return 8;

        if((HasWall(World.GetNEBlock(pos)) && HasNoWall(World.GetNWBlock(pos)) && HasNoWall(World.GetSEBlock(pos)) && HasNoWall(World.GetSWBlock(pos)))
        || (HasWall(World.GetNEBlock(pos)) && HasWall(World.GetNWBlock(pos)) && HasWall(World.GetSEBlock(pos)) && HasWall(World.GetSWBlock(pos))))
            return 1;
        if((HasWall(World.GetNWBlock(pos)) && HasNoWall(World.GetNEBlock(pos)) && HasNoWall(World.GetSWBlock(pos)) && HasNoWall(World.GetSEBlock(pos)))
        || (HasWall(World.GetNWBlock(pos)) && HasWall(World.GetNEBlock(pos)) && HasWall(World.GetSWBlock(pos)) && HasWall(World.GetSEBlock(pos))))
            return 2;
        if((HasWall(World.GetSEBlock(pos)) && HasNoWall(World.GetSWBlock(pos)) && HasNoWall(World.GetNEBlock(pos)) && HasNoWall(World.GetNWBlock(pos)))
        || (HasWall(World.GetSEBlock(pos)) && HasWall(World.GetSWBlock(pos)) && HasWall(World.GetNEBlock(pos)) && HasWall(World.GetNWBlock(pos))))
            return 3;
        if((HasWall(World.GetSWBlock(pos)) && HasNoWall(World.GetSEBlock(pos)) && HasNoWall(World.GetNWBlock(pos)) && HasNoWall(World.GetNEBlock(pos)))
        || (HasWall(World.GetSWBlock(pos)) && HasWall(World.GetSEBlock(pos)) && HasWall(World.GetNWBlock(pos)) && HasWall(World.GetNEBlock(pos))))
            return 4;

            /*if(FullIsoNeighs(pos) || EmptyIsoNeighs(pos) ||
           (HasWall(World.GetNEBlock(pos)) && HasWall(World.GetSWBlock(pos))
            && HasNoWall(World.GetSEBlock(pos)) && HasNoWall(World.GetNWBlock(pos))
           )||
           (HasWall(World.GetNWBlock(pos)) && HasWall(World.GetSEBlock(pos))
            && HasNoWall(World.GetSWBlock(pos)) && HasNoWall(World.GetNEBlock(pos))
           ))
            return 0;*/

        return 0;
    }

    static int CountIsoNeighs(Vector3Int pos)
    {
        int n = 0;
        if (HasWall(World.GetNEBlock(pos))) n++;
        if (HasWall(World.GetNWBlock(pos))) n++;
        if (HasWall(World.GetSEBlock(pos))) n++;
        if (HasWall(World.GetSWBlock(pos))) n++;
        return n;
    }
    static bool EmptyIsoNeighs(Vector3Int pos)
    {
        return (!HasWall(World.GetNEBlock(pos))) && (!HasWall(World.GetNWBlock(pos)))
             &&(!HasWall(World.GetSEBlock(pos))) && (!HasWall(World.GetSWBlock(pos)));
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



    public float GetLocalZ(float x, float y)
    {
        x = transform.position.x - x + TileSizeX;
        y = transform.position.y - y;
        switch (GetWallID())
        {
            default:
            case 0: return 0;
            case 1: return  x + y;
            case 2: return -x + y;
            case 3: return  x - y;
            case 4: return -x - y;
            ///
        }
    }

    #endregion Neigh
    bool focus;
    public void SetFocus(bool focus)
    {
        this.focus = focus;
    }
}
