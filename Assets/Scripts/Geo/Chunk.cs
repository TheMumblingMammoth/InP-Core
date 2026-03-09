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
                    if(z < 4)
                    {
                        voxels[z][y][x].SetFloor(BlockType.Dirt);
                        voxels[z][y][x].SetWall(BlockType.Dirt);
                    }
                    else if(z > 4)
                    {
                        voxels[z][y][x].SetFloor(BlockType.Air);
                        voxels[z][y][x].SetWall(BlockType.Air);
                    }
                    else
                    {
                        voxels[z][y][x].SetFloor(BlockType.Grass);
                        voxels[z][y][x].SetWall(BlockType.Air);
                    }
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