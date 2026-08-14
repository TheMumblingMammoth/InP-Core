using UnityEngine;
using UnityEngine.AI;

public class MiniMap : MonoBehaviour
{
    enum MiniMapType
    {
        Temp, Wet, High, All
    }
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] World world;
    
    
    [SerializeField] MiniMapType miniMapType;
    [ContextMenu("Gen")]
    public void Gen()
    {   
        
        Vector2Int size = new Vector2Int(World.size.x * World.chunkSize.x, World.size.y * World.chunkSize.y);
        Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);
        for(int Y = 0; Y < World.size.y; Y++){
            for(int X = 0; X < World.size.x; X++){
                float [,] noizeMap;
                switch (miniMapType)
                {
                    default:
                    case MiniMapType.High: noizeMap = world.HighGenerator.GetNoiseMap(new Vector2(X, Y)); break;
                    case MiniMapType.Temp: noizeMap = world.TempGenerator.GetNoiseMap(new Vector2(X, Y)); break;
                    case MiniMapType.Wet: noizeMap = world.WetGenerator.GetNoiseMap(new Vector2(X, Y)); break;   
                } 

                for(int i = 0; i < World.chunkSize.y; i++){
                    for(int j = 0; j < World.chunkSize.x; j++)
                    {
                        texture.SetPixel(X * World.chunkSize.x + j, Y * World.chunkSize.y + i, GetColor(noizeMap[j, i]));
                    }
                }
            }
        }
        texture.Apply();
        sprite.sprite = Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), 32f);
    }


    void Update()
    {
        Gen();
    }


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;

}

    public TerrainType[] regions;

    public Color GetColor(float value)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (value <= regions[i].height)
            {
                return regions[i].color; 
            }
        }
        return regions[regions.Length - 1].color;
    }


}
