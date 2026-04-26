using UnityEngine;

public class BuilderTool : MonoBehaviour
{
    public static BuilderTool proxy { get; private set;}
    public Sprite wall, floor, ceil,  grass, dirt;
    public TileClipSet walls, ceils;
    public TileClip STUFF;
    void Awake()
    {
        proxy = this;
    }
    public static Sprite GetSprite(BlockType block)
    {
        switch(block){
            case BlockType.Air:     return null;
            case BlockType.WoodLog: return proxy.floor;
            case BlockType.Grass:   return proxy.grass;
            case BlockType.Dirt:    return proxy.dirt;
            default:            return null;
        }
    }
}
 