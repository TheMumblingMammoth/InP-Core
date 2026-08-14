using System;
using UnityEngine;

public class Chunk 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3Int coords;
    public Block [][][] voxels {get; private set;}
    public float[,] tempMap {get; private set;}
    public float[,] wetMap {get; private set;}
    public Chunk(Vector3Int coords)
    {
        this.coords = coords;
        float[,] heightMap = World.main.HighGenerator.GetNoiseMap(new Vector2(coords.x, coords.y));
        tempMap = World.main.TempGenerator.GetNoiseMap(new Vector2(coords.x, coords.y));
        wetMap = World.main.WetGenerator.GetNoiseMap(new Vector2(coords.x, coords.y));
        voxels = new Block[World.chunkSize.z][][];
        for(int z = 0; z < World.chunkSize.z; z++)
        {
            voxels[z] = new Block[World.chunkSize.y][];
            for(int y = 0; y < World.chunkSize.y; y++)
            {
                voxels[z][y] = new Block[World.chunkSize.x];
                for(int x = 0; x < World.chunkSize.x; x++)
                {
                    voxels[z][y][x] = new Block();  
                    if(World.IsFlat()){
                        if(z == 0){
                            voxels[z][y][x].SetBlock(BlockType.Dirt);
                            voxels[z][y][x].SetCarpet(CarpetType.Grass);
                        }
                        else
                        {
                            voxels[z][y][x].SetBlock(BlockType.Air);
                        }
                    }else{
                        if(z == 0){
                            voxels[z][y][x].SetBlock(BlockType.DirtRock);
                            continue;
                        }

                        float high = heightMap[x, y]*World.chunkSize.z/2 + 2f;
                        bool dry = tempMap[x, y] + wetMap[x, y] > 1.0f;
                        if(high / 2 > z)
                        {
                            voxels[z][y][x].SetBlock(BlockType.DirtRock);
                        }
                        else if(high > z)
                        {
                            voxels[z][y][x].SetBlock(BlockType.Dirt);
                            if(high < z + 0.5f)
                                voxels[z][y][x].SetCarpet(dry ? CarpetType.DryGrass : CarpetType.Grass);
                        }                     
                        else if(high + 0.5f > z)
                        {
                            voxels[z][y][x].SetBlock(BlockType.Dirt);
                            voxels[z][y][x].SetHalf();
                            voxels[z][y][x].SetCarpet(dry ? CarpetType.DryGrass : CarpetType.Grass);
                        }                     
                        else
                        {
                            voxels[z][y][x].SetBlock(BlockType.Air);
                        }
                    }
                    //*/
                }
            }
        }
    }

    

    public float GetBlockZ(int x, int y)
    {
        for(int z = 0; z < World.chunkSize.z-1; z++)
        {
            if(voxels[z][y][x].HasBlock() && !voxels[z][y][x].full)
                return z + 0.5f;
            if(!voxels[z+1][y][x].HasBlock())
                return z + 1;
        }
        if(voxels[World.chunkSize.z-1][y][x].HasBlock() && !voxels[World.chunkSize.z-1][y][x].full)
            return World.chunkSize.z - 1 + 0.5f;
            //if(!voxels[z+1][y][x].HasBlock())
        return World.chunkSize.z-1;
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