using UnityEngine;

public class BuilderTool : MonoBehaviour
{
    public static BuilderTool proxy { get; private set;}
    public Sprite wall, floor, ceil,  grass, dirt;
    public TileClipSet wood;
    void Awake()
    {
        proxy = this;
    }
    public static Sprite GetSprite(Block block)
    {
        switch(block){
            case Block.Air:     return null;
            case Block.Wood:    return proxy.floor;
            case Block.Grass:   return proxy.grass;
            case Block.Dirt:    return proxy.dirt;
            default:            return null;
        }
    }
}
 