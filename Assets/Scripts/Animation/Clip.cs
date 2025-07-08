using UnityEngine;
public class Clip : MonoBehaviour{
    public AniType type;
    [SerializeField] public AnimationFrame [] frames;
    [SerializeField] public AudioClip sound;
    
    public float Length(){
        float sum = 0;
        for(int i = 0; i < frames.Length; i++)
            sum += frames[i].delay;
        return sum;
    }
    public void LoadFrames(Sprite [] sprites){
        frames = new AnimationFrame[sprites.Length];
        for(int i = 0; i< sprites.Length; i++){
            frames[i] = new AnimationFrame(sprites[i]);
        }  
    }   

    public Sprite LastFrame(){
        return frames[frames.Length-1].pic;
    }

    public bool hasSound(){
        return sound != null;
    }

    public void SetDelay(float delay){
        foreach(AnimationFrame frame in frames)
            frame.delay = delay;
    }

    public void SetLength(float length){
        float delay = length / frames.Length;
        foreach(AnimationFrame frame in frames)
            frame.delay = delay;
    }

    private static Clip nullClip = null;
    public static Clip NullClip(){
        if(nullClip == null)
            nullClip = Resources.Load<Clip>("Clips/NullClip");
        return nullClip;
    }

    public Clip Reverse(){
        Clip reverse = Instantiate(this);
        for(int i = 0; i < frames.Length; i++)
            reverse.frames[i] = frames[frames.Length - 1 - i];
        return reverse;
    }
}
