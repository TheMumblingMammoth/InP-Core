using UnityEngine;

public class Chunk 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3Int coords;
    public Block [][][] voxels {get; private set;}

    public Chunk(Vector3Int coords)
    {
        this.coords = coords;
        float[,] noiseMap = World.main.HighGenerator.GetNoiseMap(coords);

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
                    if(z == 0){
                        voxels[z][y][x].SetBlock(BlockType.Dirt);
                        voxels[z][y][x].SetCarpet(CarpetType.Grass);
                    }
                    else
                    {
                        voxels[z][y][x].SetBlock(BlockType.Air);
                    }
                    /*
                    
                    float high = noiseMap[x, y]*World.chunkSize.z + 1;
                    if(high > z)
                    {
                        voxels[z][y][x].SetBlock(BlockType.Dirt);
                        if(high < z + 0.5)
                            voxels[z][y][x].SetCarpet(CarpetType.Grass);
                    }                     
                    else if(high + 0.5 > z)
                    {
                        voxels[z][y][x].SetBlock(BlockType.Dirt);
                        voxels[z][y][x].SetHalf();
                        voxels[z][y][x].SetCarpet(CarpetType.Grass);
                    }                     
                    else
                    {
                        voxels[z][y][x].SetBlock(BlockType.Air);
                    }
                    */
                }
            }
        }
    }

    public int GetHighestBlock(int x, int y)
    {
        for(int z = 0; z < World.chunkSize.z-1; z++)
            if(!voxels[z+1][y][x].HasBlock())
                return z;
        return World.chunkSize.z-1;
    }
}


/*
0 - air
1 - dirt
2 - grass
11 - grass
*/