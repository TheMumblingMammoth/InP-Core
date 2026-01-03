using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Analytics;

public class World : MonoBehaviour
{
    public static Vector3Int chunkSize = new Vector3Int(64, 64, 8);
    public static World main {get; private set;}
    public static Vector3Int size = new Vector3Int(1, 1, 1);
    public Chunk [][][] chunks {get; private set;}
    public void Gen()
    {
        chunks = new Chunk[size.z][][];
        for(int z = 0; z < size.z; z++)
        {
            chunks[z] = new Chunk[size.y][];
            for(int y = 0; y < size.x; y++)
            {
                chunks[z][y] = new Chunk[size.x];
                for(int x = 0; x < size.x; x++)
                {
                    chunks[z][y][x] = new Chunk();
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        main = this;
        Gen();
    }

    // Update is called once per frame
    void Update()
    {

    }

    Vector3Int PosToCoords(Vector3Int pos)
    {
        return new Vector3Int(pos.x / size.x, pos.y / size.y, pos.z / size.z);
    }

    Vector3Int PosInCoords(Vector3Int pos)
    {
        return new Vector3Int(pos.x % size.x, pos.y % size.y, pos.z % size.z);
    }

    public static Block GetBlock(Vector3Int pos)
    { 
        return main.chunks[pos.z / size.z][pos.y / size.y][pos.x / size.x].voxels[pos.z % size.z][pos.y % size.y][pos.x % size.x];
    }
    public static void SetBlock(Vector3Int pos, Block block)
    { 
        main.chunks[pos.z / size.z][pos.y / size.y][pos.x / size.x].voxels[pos.z % size.z][pos.y % size.y][pos.x % size.x] = block;
    }

}
