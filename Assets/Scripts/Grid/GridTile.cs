using UnityEngine;
public class GridTile : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite, tile, frame, onTile;
    Vector3Int pos;
    Block block;
    void Awake()
    {
        tile.enabled = false;
        frame.enabled = false;
    }
    void FixedUpdate()
    {
        if (frame.enabled && !IsOver()){
            frame.enabled = false;
            return;
        }
        if (!frame.enabled && IsOver()){
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
        tile.sprite = BuilderTool.GetSprite(block);
        block = World.GetBlock(pos);
        tile.sprite = null;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && IsOver())
        {
            if(onTile.sprite != null) 
                onTile.sprite = null;
            else
                onTile.sprite = BuilderTool.proxy.wall;
        }
        if(Input.GetMouseButtonDown(1) && IsOver())
        {
            if(sprite.sprite != BuilderTool.proxy.grass) 
                sprite.sprite = BuilderTool.proxy.grass;
            else
                sprite.sprite = BuilderTool.proxy.floor;
        }
    }

    bool IsOver() { return IsOver(Camera.main.ScreenToWorldPoint(Input.mousePosition)); }
    bool IsOver(Vector2 pos)
    {
        return pos.x >= transform.position.x - 0.25f && pos.y >= transform.position.y 
            && pos.x < transform.position.x + 0.75f && pos.y < transform.position.y + 1;
    }

    
}