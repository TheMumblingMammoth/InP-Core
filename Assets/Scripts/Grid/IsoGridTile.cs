using System;
using UnityEngine;
public class IsoGridTile : MonoBehaviour
{
    public const float TileSizeX = 62/32f, TileSizeY = 1;
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

            if (block.GetWall() != BlockType.Air)
                block.SetWall(BlockType.Air);
            else
                block.SetWall(BlockType.WoodLog);
            World.SetBlock(pos, block);
            IsoGrid.Upload();
        }
        if (Input.GetMouseButtonDown(1) && IsOver())
        {

            if (block.GetWall() != BlockType.Air)
                block.SetWall(BlockType.Air);
            else
                block.SetWall(BlockType.Fachwerk);
            World.SetBlock(pos, block);
            IsoGrid.Upload();
        }
        if (Input.GetMouseButtonDown(2) && IsOver())
        {

            
            block.SetWall(BlockType.Stuff);
            World.SetBlock(pos, block);
            IsoGrid.Upload();
        }
        
        
    }

    bool IsOver() { return IsOver(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)))); }
    bool IsOver(Vector2 pos)
    {
        return pos.x >= transform.position.x && pos.y >= transform.position.y
            && pos.x < transform.position.x + 1f && pos.y < transform.position.y + 1;
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


}
