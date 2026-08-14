using UnityEngine;
public class Generator : MonoBehaviour {
    [SerializeField] private  int seed;         
    [SerializeField] private float noiseScale;    
    [SerializeField] private  int octaves;         
    [Range(0, 1)]               
    [SerializeField] private  float persistance;  
    [SerializeField] private  float lacunarity;   

    [SerializeField] private float upperHeight = 0f;

    public float[,] GetNoiseMap()
    {
        
        return Noise.GenerateNoiseMap(World.chunkSize.x * World.size.x, World.chunkSize.y * World.size.y, seed, noiseScale, octaves, persistance, lacunarity, new Vector2(0, 0), upperHeight);
    }
    public float[,] GetNoiseMap(Vector2 coords)
    {
        return Noise.GenerateNoiseMap(World.chunkSize.x, World.chunkSize.y, seed, noiseScale, octaves, persistance, lacunarity, new Vector2(coords.x * World.chunkSize.x, -coords.y * World.chunkSize.y), upperHeight);
    }
}