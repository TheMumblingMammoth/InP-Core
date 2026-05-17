using UnityEngine;
public enum BlockType
{
    Void = -1, Air = 0, Dirt = 1, Grass = 2,
    WoodLog = 11, Fachwerk = 12, Stuff = 13
}

public class Block 
{
    private BlockType block = BlockType.Void;
    public BlockType GetBlock(){ return block; }
    public void SetBlock(BlockType block){ this.block = block; }
    public bool HasBlock(){  return block != BlockType.Void && block != BlockType.Air; }
}