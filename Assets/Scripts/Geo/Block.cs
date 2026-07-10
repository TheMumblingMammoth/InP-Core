using UnityEngine;
public enum BlockType
{
    Void = -1, Air = 0, Dirt = 1, Grass = 2,
    WoodLog = 11, Fachwerk = 12, Stuff = 13
}

public enum CarpetType
{
    None = 0, Grass = 1, DryGrass = 2,
}

public class Block 
{
    
    private BlockType block = BlockType.Void;
    public BlockType GetBlock(){ return block; }
    public void SetBlock(BlockType block){ this.block = block; }    
    public bool HasBlock(){  return block != BlockType.Void && block != BlockType.Air; }


    private CarpetType carpet = CarpetType.None;
    public CarpetType GetCarpet(){ return carpet; }
    public void SetCarpet(CarpetType carpet){ this.carpet = carpet; }
    public bool HasCarpet(){  return carpet != CarpetType.None; }

    public void SetEmpty(){ block = BlockType.Air; carpet = CarpetType.None; }

    public bool full { get; private set;} = true; 
    public void SetHalf(){ full = false;}
    public void SetFull(){ full = true;}
}