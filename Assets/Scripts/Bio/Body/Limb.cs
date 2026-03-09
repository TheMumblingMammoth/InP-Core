using UnityEngine;
public abstract class Limb : MonoBehaviour
{
    public Body.BodyPart type;
    public Vector2 pos {get; protected set;}
    public float alpha {get; protected set;}
    protected abstract void Awake();
    public abstract void SetPos(Vector2 pos, float alpha = 0);
    public abstract Vector2 GetPos();
    public abstract void SetSkin(int skinID);
    public abstract void SetOrder(int order); 
    public abstract void SetColor(Color color);

    public abstract void Flip();
    [SerializeField] protected bool flipY;
    public void FlipY(){    flipY = !flipY; }
}