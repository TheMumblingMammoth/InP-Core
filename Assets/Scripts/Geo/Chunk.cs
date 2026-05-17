using UnityEngine;

public class Chunk 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Block [][][] voxels {get; private set;}
    public Chunk()
    {
        voxels = new Block[World.chunkSize.z][][];
        for(int z = 0; z < World.chunkSize.z; z++)
        {
            voxels[z] = new Block[World.chunkSize.y][];
            for(int y = 0; y < World.chunkSize.x; y++)
            {
                voxels[z][y] = new Block[World.chunkSize.x];
                for(int x = 0; x < World.chunkSize.x; x++)
                {
                    voxels[z][y][x] = new Block();
                    
                    if( (Mathf.PerlinNoise(x / 5f, y / 7f) + Mathf.PerlinNoise(x / 17f, (y + 3) / 13f)) + 1 > z)
                    {
                        voxels[z][y][x].SetBlock(BlockType.Grass);
                    }                    
                    else
                    {
                        voxels[z][y][x].SetBlock(BlockType.Air);
                    }                        
                    /*
                    else if(z == 1)
                    {
                        if(Mathf.PerlinNoise(x / 5f, y / 7f) > 0.5f)
                            voxels[z][y][x].SetBlock(BlockType.Air);
                        else
                            voxels[z][y][x].SetBlock(BlockType.Grass);
                    }
                    */
                }
            }
        }
    }
}


/*
0 - air
1 - dirt
2 - grass
11 - grass
*/