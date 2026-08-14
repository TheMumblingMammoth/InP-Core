using UnityEngine;

public class World : MonoBehaviour
{
    public static Vector3Int chunkSize = new Vector3Int(64, 64, 8);
    public static World main { get; private set; }
    public static Vector3Int size = new Vector3Int(5, 5, 1);
    [SerializeField] bool flat; public static bool IsFlat(){ return main.flat; }
    public Chunk[][][] chunks { get; private set; }

    [Header("Generators")]
    [SerializeField] private int seed = 0; public static int GetSeed(){ return main.seed; }
    public Generator TempGenerator, WetGenerator, HighGenerator;

    public void Gen()
    {
        chunks = new Chunk[size.z][][];
        for (int z = 0; z < size.z; z++)
        {
            chunks[z] = new Chunk[size.y][];
            for (int y = 0; y < size.y; y++)
            {
                chunks[z][y] = new Chunk[size.x];
                for (int x = 0; x < size.x; x++)
                {
                    chunks[z][y][x] = new Chunk(new Vector3Int(x, y, z));
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
        return new Vector3Int(pos.x / chunkSize.x, pos.y / chunkSize.y, pos.z / chunkSize.z);
    }

    Vector3Int PosInCoords(Vector3Int pos)
    {
        return new Vector3Int(pos.x % chunkSize.x, pos.y % chunkSize.y, pos.z % chunkSize.z);
    }

    public static float GetBlockZ(Vector2 pos)
    {
        return GetBlockZ((Vector2Int)IsoGrid.LinToIso(pos));
    }

    public static float GetBlockZ(Vector2Int pos)
    {
        if(pos.x < 0 || pos.x >= size.x * chunkSize.x
        || pos.y < 0 || pos.y >= size.y * chunkSize.y)
            return -1;
        //pos.x % chunkSize.x, pos.y % chunkSize.y, pos.z % chunkSize.z
        for(int floor = 0; floor < size.z; floor++){
            float q = floor * chunkSize.z + main.chunks[floor][pos.y / chunkSize.y][pos.x / chunkSize.x].GetBlockZ(pos.x % chunkSize.x, pos.y % chunkSize.y);
            if (q < (floor + 1) * chunkSize.z - 1)
                return floor * chunkSize.z + q;
        }
        return 0;
    }

    public static Block GetBlock(Vector2 pos, int z)
    {
        Vector3Int coords = IsoGrid.LinToIso(pos);
        return GetBlock(new Vector3Int(coords.x, coords.y, z));
    }
    public static Block GetBlock(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= size.x * chunkSize.x ||
            pos.y < 0 || pos.y >= size.y * chunkSize.y ||
            pos.z < 0 || pos.z >= size.z * chunkSize.z) return null;
        return main.chunks[pos.z / chunkSize.z][pos.y / chunkSize.y][pos.x / chunkSize.x].voxels[pos.z % chunkSize.z][pos.y % chunkSize.y][pos.x % chunkSize.x];
    }

    public static Vector2 GetClosestToBlock(Vector2 pos, Vector2 new_pos)
    {
        float dx = new_pos.x - pos.x, dy = new_pos.y - pos.y;
        float ix = (int)new_pos.x, iy = (int)new_pos.y;
        if (dx == 0)
        {
            return new Vector2(pos.x, iy + (dy < 0 ? 1f : -0.01f));
        }
        else if (dy == 0)
        {
            return new Vector2(ix + (dx < 0 ? 1f : -0.01f), pos.y);
        }
        float k = (new_pos.y - pos.y) / (new_pos.x - pos.x);
        float c = pos.y - k * pos.x;
        float x = ix + (dx < 0 ? 1f : -0.01f);
        float y = iy + (dy < 0 ? 1f : -0.01f);
        Vector2 VX = new Vector2(x, k * x + c);
        Vector2 VY = new Vector2((y - c) / k, y);
        return Vector2.Distance(pos, VX) < Vector2.Distance(pos, VY) ? VX : VY;
    }

    public int GetHighestBlock(Vector2Int pos)
    {
        return chunks[size.z-1][pos.y / chunkSize.y][pos.x / chunkSize.x].GetHighestBlock(pos.x % chunkSize.x, pos.y % chunkSize.y);
    }

    public static void SetBlock(Vector3Int pos, Block block)
    {
        main.chunks[pos.z / chunkSize.z][pos.y / chunkSize.y][pos.x / chunkSize.x].voxels[pos.z % chunkSize.z][pos.y % chunkSize.y][pos.x % chunkSize.x] = block;
    }

    #region DirGetBlock
        public static Block GetNBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x, pos.y + 1, pos.z)); }
        public static Block GetSBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x, pos.y - 1, pos.z)); }
        public static Block GetWBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x - 2, pos.y - 1, pos.z)); }
        public static Block GetEBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x + 2, pos.y + 1, pos.z)); }
        public static Block GetNEBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x + 1, pos.y + 1, pos.z)); }
        public static Block GetNWBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x - 1, pos.y, pos.z)); }
        public static Block GetSEBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x + 1, pos.y, pos.z)); }
        public static Block GetSWBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x - 1, pos.y - 1, pos.z)); }
        public static Block GetTBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x, pos.y, pos.z + 1)); }
        public static Block GetBBlock(Vector3Int pos) { return GetBlock(new Vector3Int(pos.x, pos.y, pos.z - 1)); }
    #endregion

    #region Generator

    public static float GetTemp(Vector3Int pos)
    {
        return main.chunks[pos.z / chunkSize.z][pos.y / chunkSize.y][pos.x / chunkSize.x].tempMap[pos.x % chunkSize.x, pos.y % chunkSize.y];
    }

    public static float GetWet(Vector3Int pos)
    {
        return main.chunks[pos.z / chunkSize.z][pos.y / chunkSize.y][pos.x / chunkSize.x].wetMap[pos.x % chunkSize.x, pos.y % chunkSize.y];
    }


    #endregion Generator

}
