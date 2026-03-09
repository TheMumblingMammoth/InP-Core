using UnityEngine;
public enum BlockType
{
    Void = -1, Air = 0, Dirt = 1, Grass = 2,
    WoodLog = 11, Fachwerk = 12,
}

public class Block 
{
    private BlockType floor = BlockType.Void, wall = BlockType.Void, ceil = BlockType.Void;
    public BlockType GetFloor(){ return floor; }
    public void SetFloor(BlockType floor){ this.floor = floor; }
    public BlockType GetWall(){ return wall; }
    public void SetWall(BlockType wall){ this.wall = wall; }
    public BlockType GetCeil(){ return ceil; }
    public void SetCeil(BlockType ceil){ this.ceil = ceil; }
    public bool HasWall(){  return wall != BlockType.Void && wall != BlockType.Air; }
}