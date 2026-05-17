using System;
using UnityEngine;
public class GridTile : MonoBehaviour
{
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
        if (frame.enabled && !IsOver())
        {
            frame.enabled = false;
            return;
        }
        if (!frame.enabled && IsOver())
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
        
        if (block.GetBlock() == BlockType.Void || block.GetBlock() == BlockType.Air)
        {
            SetWallSprite(null);
        }
        else
        {
            string wall_name = block.GetBlock().ToString();
            if(block.GetBlock() == BlockType.Stuff)
            {
                SetWallSprite(BuilderTool.proxy.STUFF.frames[(pos.x * 5 + pos.y * 7) % 16]);
            }
            else
                SetWallSprite(BuilderTool.proxy.walls.ClipFor(wall_name).frames[GetWallID()]);
        }
        
        
        
        floor.sortingOrder = -pos.y * 1000 + pos.x;
        SetWallOrder(-pos.y * 1000 + pos.x + 1);
        ceil.sortingOrder = -(pos.y - 2) * 1000 + pos.x + 1;
        if(!Core.IsOrtho()){
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
    }



    void Update()
    {
        if (block == null)
            return;
        if (Input.GetMouseButtonDown(0) && IsOver())
        {

            if (block.GetBlock() != BlockType.Air)
                block.SetBlock(BlockType.Air);
            else
                block.SetBlock(BlockType.WoodLog);
            World.SetBlock(pos, block);
            Grid.Upload();
        }
        if (Input.GetMouseButtonDown(1) && IsOver())
        {

            if (block.GetBlock() != BlockType.Air)
                block.SetBlock(BlockType.Air);
            else
                block.SetBlock(BlockType.Fachwerk);
            World.SetBlock(pos, block);
            Grid.Upload();
        }
        if (Input.GetMouseButtonDown(2) && IsOver())
        {

            block.SetBlock(BlockType.Stuff);
            World.SetBlock(pos, block);
            Grid.Upload();
        }
        
        
    }

    bool IsOver() { return IsOver(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)))); }
    bool IsOver(Vector2 pos)
    {
        return pos.x >= transform.position.x && pos.y >= transform.position.y
            && pos.x < transform.position.x + 1f && pos.y < transform.position.y + 1;
    }


    private int GetWallID()
    {
                                return 0; // 
    
        if (HasWall(World.GetNBlock(pos)))
        {
            if (HasWall(World.GetSBlock(pos)))
            {
                if (HasWall(World.GetWBlock(pos)))
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        if (HasWall(World.GetNEBlock(pos))
                         && HasWall(World.GetSEBlock(pos))
                         && HasWall(World.GetSWBlock(pos))
                         && HasWall(World.GetNWBlock(pos)))
                            return 46; // NSEW+NE+SE+SW
                        if (HasWall(World.GetNEBlock(pos))
                         && HasWall(World.GetSEBlock(pos))
                         && HasWall(World.GetSWBlock(pos)))
                            return 42; // NSEW+NE+SE+SW
                        if (HasWall(World.GetSEBlock(pos))
                         && HasWall(World.GetSWBlock(pos))
                         && HasWall(World.GetNWBlock(pos)))
                            return 43; // NSEW+SE+SW+NW
                        if (HasWall(World.GetSWBlock(pos))
                         && HasWall(World.GetNWBlock(pos))
                         && HasWall(World.GetNEBlock(pos)))
                            return 44; // NSEW+NE+SE+SW
                        if (HasWall(World.GetNWBlock(pos))
                         && HasWall(World.GetNEBlock(pos))
                         && HasWall(World.GetSEBlock(pos)))
                            return 45; // NSEW+NE+SE+SW                        
                        if (HasWall(World.GetNEBlock(pos)) && HasWall(World.GetNWBlock(pos)))
                            return 36; // NSEW+NE+NW
                        if (HasWall(World.GetNEBlock(pos)) && HasWall(World.GetSEBlock(pos)))
                            return 37; // NSEW+NE+SE
                        if (HasWall(World.GetNEBlock(pos)) && HasWall(World.GetSWBlock(pos)))
                            return 38; // NSEW+NE+SW
                        if (HasWall(World.GetSEBlock(pos)) && HasWall(World.GetSWBlock(pos)))
                            return 39; // NSEW+SE+SW
                        if (HasWall(World.GetSEBlock(pos)) && HasWall(World.GetNWBlock(pos)))
                            return 40; // NSEW+SE+NW
                        if (HasWall(World.GetSWBlock(pos)) && HasWall(World.GetNWBlock(pos)))
                            return 41; // NSEW+SW+NW
                        if (HasWall(World.GetNEBlock(pos)))
                            return 32; // NSEW+NE
                        if (HasWall(World.GetNWBlock(pos)))
                            return 33; // NSEW+NW
                        if (HasWall(World.GetSEBlock(pos)))
                            return 34; // NSEW+SE
                        if (HasWall(World.GetSWBlock(pos)))
                            return 35; // NSEW+SW
                            
                        return 15; // NSEW
                    }
                    else
                    {
                        if (HasWall(World.GetNWBlock(pos)) && HasWall(World.GetSWBlock(pos)))
                            return 23; // NSW++
                        if (HasWall(World.GetSWBlock(pos)))
                            return 24; // NSW+SW
                        if (HasWall(World.GetNWBlock(pos)))
                            return 25; // NSW+NW
                        return 11; // NSW
                    }
                }
                else
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        if (HasWall(World.GetNEBlock(pos)) && HasWall(World.GetSEBlock(pos)))
                            return 26; // NSE++
                        if (HasWall(World.GetNEBlock(pos)))
                            return 27; // NSE+NE
                        if (HasWall(World.GetSEBlock(pos)))
                            return 28; // NSE+SE
                        return 12; // NSE
                    }
                    else
                    {
                        return 14; // NS
                    }
                }
            }
            else
            {
                if (HasWall(World.GetWBlock(pos)))
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        if (HasWall(World.GetNWBlock(pos)) && HasWall(World.GetNEBlock(pos)))
                            return 22; // NEW++
                        if (HasWall(World.GetNEBlock(pos)))
                            return 20; // NEW+E 
                        if (HasWall(World.GetNWBlock(pos)))
                            return 21; // NEW+W 
                        return 10; // NEW
                    }
                    else
                    {
                        if (HasWall(World.GetNWBlock(pos)))
                            return 17; // NW+ 
                        else
                            return 7; // NW
                    }
                }
                else
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        if (HasWall(World.GetNEBlock(pos)))
                            return 16; // NE+ 
                        else
                            return 6; // NE
                    }
                    else
                    {
                        return 4; // N
                    }
                }
            }
        }
        else
        {
            if (HasWall(World.GetSBlock(pos)))
            {
                if (HasWall(World.GetWBlock(pos)))
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        if (HasWall(World.GetSWBlock(pos)) && HasWall(World.GetSEBlock(pos)))
                            return 29; // SEW++
                        if (HasWall(World.GetSEBlock(pos)))
                            return 30; // SEW+E 
                        if (HasWall(World.GetSWBlock(pos)))
                            return 31; // SEW+W 
                        return 13; // SEW
                    }
                    else
                    {
                        if (HasWall(World.GetSWBlock(pos)))
                            return 18; // SW+ 
                        else
                            return 8; // SW
                    }
                }
                else
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        if (HasWall(World.GetSEBlock(pos)))
                            return 19; // SE+ 
                        else
                            return 9; // SE
                    }
                    else
                    {
                        return 5; // S
                    }
                }
            }
            else
            {
                if (HasWall(World.GetWBlock(pos)))
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        return 1; // WE
                    }
                    else
                    {
                        return 3; // W
                    }
                }
                else
                {
                    if (HasWall(World.GetEBlock(pos)))
                    {
                        return 2; // E
                    }
                    else
                    {
                        return 0; // 
                    }
                }
            }
        }
         
    }

   
    
    
    static bool HasWall(Block block)
    {
        if(block == null)
            return false;
        return block.GetBlock() != BlockType.Air && block.GetBlock() != BlockType.Void;
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


}
