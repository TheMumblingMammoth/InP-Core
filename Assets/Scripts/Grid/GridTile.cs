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
        if (block == Block.Void)
        {
            floor.sprite = null;
            wall.sprite = null;
        }
        if (block == Block.Air)
        {
            floor.sprite = BuilderTool.proxy.grass;
            wall.sprite = null;
        }
        else if (block == Block.Wood)
        {
            wall.sprite = BuilderTool.proxy.wood.ClipFor("Wall").frames[GetWallID()];
        }
        floor.sortingOrder = -pos.y * 32 + pos.x;
        wall.sortingOrder = -pos.y * 32 + pos.x;
        ceil.sortingOrder = -pos.y * 32 + pos.x;
    }



    void Update()
    {
        if (block == Block.Void)
            return;
        if (Input.GetMouseButtonDown(0) && IsOver())
            {

                if (block != Block.Air)
                    block = Block.Air;
                else
                    block = Block.Wood;
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
        if (IsWall(World.GetNBlock(pos)))
        {
            if (IsWall(World.GetSBlock(pos)))
            {
                if (IsWall(World.GetWBlock(pos)))
                {
                    if (IsWall(World.GetEBlock(pos)))
                    {
                        if (IsWall(World.GetNEBlock(pos))
                         && IsWall(World.GetSEBlock(pos))
                         && IsWall(World.GetSWBlock(pos))
                         && IsWall(World.GetNWBlock(pos)))
                            return 46; // NSEW+NE+SE+SW
                        if (IsWall(World.GetNEBlock(pos))
                         && IsWall(World.GetSEBlock(pos))
                         && IsWall(World.GetSWBlock(pos)))
                            return 42; // NSEW+NE+SE+SW
                        if (IsWall(World.GetSEBlock(pos))
                         && IsWall(World.GetSWBlock(pos))
                         && IsWall(World.GetNWBlock(pos)))
                            return 43; // NSEW+SE+SW+NW
                        if (IsWall(World.GetSWBlock(pos))
                         && IsWall(World.GetNWBlock(pos))
                         && IsWall(World.GetNEBlock(pos)))
                            return 44; // NSEW+NE+SE+SW
                        if (IsWall(World.GetNWBlock(pos))
                         && IsWall(World.GetNEBlock(pos))
                         && IsWall(World.GetSEBlock(pos)))
                            return 45; // NSEW+NE+SE+SW                        
                        if (IsWall(World.GetNEBlock(pos)) && IsWall(World.GetNWBlock(pos)))
                            return 36; // NSEW+NE+NW
                        if (IsWall(World.GetNEBlock(pos)) && IsWall(World.GetSEBlock(pos)))
                            return 37; // NSEW+NE+SE
                        if (IsWall(World.GetNEBlock(pos)) && IsWall(World.GetSWBlock(pos)))
                            return 38; // NSEW+NE+SW
                        if (IsWall(World.GetSEBlock(pos)) && IsWall(World.GetSWBlock(pos)))
                            return 39; // NSEW+SE+SW
                        if (IsWall(World.GetSEBlock(pos)) && IsWall(World.GetNWBlock(pos)))
                            return 40; // NSEW+SE+NW
                        if (IsWall(World.GetSWBlock(pos)) && IsWall(World.GetNWBlock(pos)))
                            return 41; // NSEW+SW+NW
                        if (IsWall(World.GetNEBlock(pos)))
                            return 32; // NSEW+NE
                        if (IsWall(World.GetNWBlock(pos)))
                            return 33; // NSEW+NW
                        if (IsWall(World.GetSEBlock(pos)))
                            return 34; // NSEW+SE
                        if (IsWall(World.GetSWBlock(pos)))
                            return 35; // NSEW+SW
                            
                        return 15; // NSEW
                    }
                    else
                    {
                        if (IsWall(World.GetNWBlock(pos)) && IsWall(World.GetSWBlock(pos)))
                            return 23; // NSW++
                        if (IsWall(World.GetSWBlock(pos)))
                            return 24; // NSW+SW
                        if (IsWall(World.GetNWBlock(pos)))
                            return 25; // NSW+NW
                        return 11; // NSW
                    }
                }
                else
                {
                    if (IsWall(World.GetEBlock(pos)))
                    {
                        if (IsWall(World.GetNEBlock(pos)) && IsWall(World.GetSEBlock(pos)))
                            return 26; // NSE++
                        if (IsWall(World.GetNEBlock(pos)))
                            return 27; // NSE+NE
                        if (IsWall(World.GetSEBlock(pos)))
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
                if (IsWall(World.GetWBlock(pos)))
                {
                    if (IsWall(World.GetEBlock(pos)))
                    {
                        if (IsWall(World.GetNWBlock(pos)) && IsWall(World.GetNEBlock(pos)))
                            return 22; // NEW++
                        if (IsWall(World.GetNEBlock(pos)))
                            return 20; // NEW+E 
                        if (IsWall(World.GetNWBlock(pos)))
                            return 21; // NEW+W 
                        return 10; // NEW
                    }
                    else
                    {
                        if (IsWall(World.GetNWBlock(pos)))
                            return 17; // NW+ 
                        else
                            return 7; // NW
                    }
                }
                else
                {
                    if (IsWall(World.GetEBlock(pos)))
                    {
                        if (IsWall(World.GetNEBlock(pos)))
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
            if (IsWall(World.GetSBlock(pos)))
            {
                if (IsWall(World.GetWBlock(pos)))
                {
                    if (IsWall(World.GetEBlock(pos)))
                    {
                        if (IsWall(World.GetSWBlock(pos)) && IsWall(World.GetSEBlock(pos)))
                            return 29; // SEW++
                        if (IsWall(World.GetSEBlock(pos)))
                            return 30; // SEW+E 
                        if (IsWall(World.GetSWBlock(pos)))
                            return 31; // SEW+W 
                        return 13; // SEW
                    }
                    else
                    {
                        if (IsWall(World.GetSWBlock(pos)))
                            return 18; // SW+ 
                        else
                            return 8; // SW
                    }
                }
                else
                {
                    if (IsWall(World.GetEBlock(pos)))
                    {
                        if (IsWall(World.GetSEBlock(pos)))
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
                if (IsWall(World.GetWBlock(pos)))
                {
                    if (IsWall(World.GetEBlock(pos)))
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
                    if (IsWall(World.GetEBlock(pos)))
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
    
    static bool IsWall(Block block)
    {
        return block != Block.Air;
    }

}