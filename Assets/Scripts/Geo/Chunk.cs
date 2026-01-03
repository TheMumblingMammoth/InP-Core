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
                    if(z < 2)
                        voxels[z][y][x] = Block.Dirt;
                    else if(z > 2)
                        voxels[z][y][x] = Block.Air;
                    else
                        voxels[z][y][x] = Block.Grass;
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