public class DelayedClip{
    public float delay;
    public Clip clip;
    public bool stop;
    public DelayedClip(Clip newClip, float time, bool stop){
        delay = time;
        clip = newClip;
        this.stop = stop;
    }
}