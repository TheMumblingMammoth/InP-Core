using UnityEngine;

public class BuilderTool : MonoBehaviour
{
    public static BuilderTool proxy { get; private set;}
    public TileClipSet blocks, carpets, ceils;
    public TileClip STUFF;
    void Awake()
    {
        proxy = this;
    }
    
}
 