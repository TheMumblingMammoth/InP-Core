using UnityEngine;

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
        float [,] noizeMap;
        switch (miniMapType)
        {
            default:
            case MiniMapType.High: noizeMap = world.HighGenerator.GetNoiseMap(); break;
            case MiniMapType.Temp: noizeMap = world.TempGenerator.GetNoiseMap(); break;
            case MiniMapType.Wet: noizeMap = world.WetGenerator.GetNoiseMap(); break;   
        } 

        for(int i = 0; i < size.y; i++){
            for(int j = 0; j < size.x; j++)
            {
                texture.SetPixel(j, i, GetColor(noizeMap[j, i]));
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
