using UnityEngine;
public class GridTile : MonoBehaviour
{
    [SerializeField] SpriteRenderer tile, frame;
    [SerializeField] SpriteRenderer floor, ceil, wall;
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
            wall.sprite = null;
            ceil.sprite = null;
            return;
        }
        if (block.GetFloor() == BlockType.Grass)
        {
            floor.sprite = BuilderTool.proxy.grass;
        }

        if (block.GetWall() == BlockType.Void || block.GetWall() == BlockType.Air)
        {
            wall.sprite = null;    
        }
        else
        {
            string wall_name = block.GetWall().ToString();
            wall.sprite = BuilderTool.proxy.walls.ClipFor(wall_name).frames[GetWallID()];
        }
        
        if (block.GetCeil() == BlockType.Void || block.GetCeil() == BlockType.Air)
        {
            ceil.sprite = null;    
        }
        else
        {
            string ceil_name = block.GetCeil().ToString();
            ceil.sprite = BuilderTool.proxy.ceils.ClipFor(ceil_name).frames[GetCeilID()];
        }
        
        floor.sortingOrder = -pos.y * 1000 + pos.x;
        wall.sortingOrder = -pos.y * 1000 + pos.x;
        ceil.sortingOrder = -(pos.y - 2) * 1000 + pos.x + 1;
    }



    void Update()
    {
        if (block == null)
            return;
        if (Input.GetMouseButtonDown(0) && IsOver())
        {

            if (block.GetWall() != BlockType.Air)
                block.SetWall(BlockType.Air);
            else
                block.SetWall(BlockType.WoodLog);
            World.SetBlock(pos, block);
            Grid.Upload();
        }
        if (Input.GetMouseButtonDown(1) && IsOver())
        {

            if (block.GetWall() != BlockType.Air)
                block.SetWall(BlockType.Air);
            else
                block.SetWall(BlockType.Fachwerk);
            World.SetBlock(pos, block);
            Grid.Upload();
        }
        if (Input.GetMouseButtonDown(2) && IsOver())
        {

            if (block.GetCeil() != BlockType.Air)
                block.SetCeil(BlockType.Air);
            else
                block.SetCeil(BlockType.WoodLog);
            World.SetBlock(pos, block);
            Grid.Upload();
        }
        
        
    }

    bool IsOver() { return IsOver(Camera.main.ScreenToWorldPoint(Input.mousePosition)); }
    bool IsOver(Vector2 pos)
    {
        return pos.x >= transform.position.x && pos.y >= transform.position.y
            && pos.x < transform.position.x + 1f && pos.y < transform.position.y + 1;
    }


    private int GetWallID()
    {
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

    private int GetCeilID()
    {
        if (HasCeil(World.GetNBlock(pos)))
        {
            if (HasCeil(World.GetSBlock(pos)))
            {
                if (HasCeil(World.GetWBlock(pos)))
                {
                    if (HasCeil(World.GetEBlock(pos)))
                    {
                        if (HasCeil(World.GetNEBlock(pos))
                         && HasCeil(World.GetSEBlock(pos))
                         && HasCeil(World.GetSWBlock(pos))
                         && HasCeil(World.GetNWBlock(pos)))
                            return 46; // NSEW+NE+SE+SW
                        if (HasCeil(World.GetNEBlock(pos))
                         && HasCeil(World.GetSEBlock(pos))
                         && HasCeil(World.GetSWBlock(pos)))
                            return 42; // NSEW+NE+SE+SW
                        if (HasCeil(World.GetSEBlock(pos))
                         && HasCeil(World.GetSWBlock(pos))
                         && HasCeil(World.GetNWBlock(pos)))
                            return 43; // NSEW+SE+SW+NW
                        if (HasCeil(World.GetSWBlock(pos))
                         && HasCeil(World.GetNWBlock(pos))
                         && HasCeil(World.GetNEBlock(pos)))
                            return 44; // NSEW+NE+SE+SW
                        if (HasCeil(World.GetNWBlock(pos))
                         && HasCeil(World.GetNEBlock(pos))
                         && HasCeil(World.GetSEBlock(pos)))
                            return 45; // NSEW+NE+SE+SW                        
                        if (HasCeil(World.GetNEBlock(pos)) && HasCeil(World.GetNWBlock(pos)))
                            return 36; // NSEW+NE+NW
                        if (HasCeil(World.GetNEBlock(pos)) && HasCeil(World.GetSEBlock(pos)))
                            return 37; // NSEW+NE+SE
                        if (HasCeil(World.GetNEBlock(pos)) && HasCeil(World.GetSWBlock(pos)))
                            return 38; // NSEW+NE+SW
                        if (HasCeil(World.GetSEBlock(pos)) && HasCeil(World.GetSWBlock(pos)))
                            return 39; // NSEW+SE+SW
                        if (HasCeil(World.GetSEBlock(pos)) && HasCeil(World.GetNWBlock(pos)))
                            return 40; // NSEW+SE+NW
                        if (HasCeil(World.GetSWBlock(pos)) && HasCeil(World.GetNWBlock(pos)))
                            return 41; // NSEW+SW+NW
                        if (HasCeil(World.GetNEBlock(pos)))
                            return 32; // NSEW+NE
                        if (HasCeil(World.GetNWBlock(pos)))
                            return 33; // NSEW+NW
                        if (HasCeil(World.GetSEBlock(pos)))
                            return 34; // NSEW+SE
                        if (HasCeil(World.GetSWBlock(pos)))
                            return 35; // NSEW+SW
                            
                        return 15; // NSEW
                    }
                    else
                    {
                        if (HasCeil(World.GetNWBlock(pos)) && HasCeil(World.GetSWBlock(pos)))
                            return 23; // NSW++
                        if (HasCeil(World.GetSWBlock(pos)))
                            return 24; // NSW+SW
                        if (HasCeil(World.GetNWBlock(pos)))
                            return 25; // NSW+NW
                        return 11; // NSW
                    }
                }
                else
                {
                    if (HasCeil(World.GetEBlock(pos)))
                    {
                        if (HasCeil(World.GetNEBlock(pos)) && HasCeil(World.GetSEBlock(pos)))
                            return 26; // NSE++
                        if (HasCeil(World.GetNEBlock(pos)))
                            return 27; // NSE+NE
                        if (HasCeil(World.GetSEBlock(pos)))
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
                if (HasCeil(World.GetWBlock(pos)))
                {
                    if (HasCeil(World.GetEBlock(pos)))
                    {
                        if (HasCeil(World.GetNWBlock(pos)) && HasCeil(World.GetNEBlock(pos)))
                            return 22; // NEW++
                        if (HasCeil(World.GetNEBlock(pos)))
                            return 20; // NEW+E 
                        if (HasCeil(World.GetNWBlock(pos)))
                            return 21; // NEW+W 
                        return 10; // NEW
                    }
                    else
                    {
                        if (HasCeil(World.GetNWBlock(pos)))
                            return 17; // NW+ 
                        else
                            return 7; // NW
                    }
                }
                else
                {
                    if (HasCeil(World.GetEBlock(pos)))
                    {
                        if (HasCeil(World.GetNEBlock(pos)))
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
            if (HasCeil(World.GetSBlock(pos)))
            {
                if (HasCeil(World.GetWBlock(pos)))
                {
                    if (HasCeil(World.GetEBlock(pos)))
                    {
                        if (HasCeil(World.GetSWBlock(pos)) && HasCeil(World.GetSEBlock(pos)))
                            return 29; // SEW++
                        if (HasCeil(World.GetSEBlock(pos)))
                            return 30; // SEW+E 
                        if (HasCeil(World.GetSWBlock(pos)))
                            return 31; // SEW+W 
                        return 13; // SEW
                    }
                    else
                    {
                        if (HasCeil(World.GetSWBlock(pos)))
                            return 18; // SW+ 
                        else
                            return 8; // SW
                    }
                }
                else
                {
                    if (HasCeil(World.GetEBlock(pos)))
                    {
                        if (HasCeil(World.GetSEBlock(pos)))
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
                if (HasCeil(World.GetWBlock(pos)))
                {
                    if (HasCeil(World.GetEBlock(pos)))
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
                    if (HasCeil(World.GetEBlock(pos)))
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
        return block.GetWall() != BlockType.Air && block.GetWall() != BlockType.Void;
    }

    static bool HasCeil(Block block)
    {
        if(block == null)
            return false;
        return block.GetCeil() != BlockType.Air && block.GetCeil() != BlockType.Void;
    }


}