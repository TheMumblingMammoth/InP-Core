using UnityEngine;
public class Clip : MonoBehaviour{
    public AniType type;
    [SerializeField] public AnimationFrame [] frames;
    [SerializeField] public AudioClip sound;
    
    public float Length() // возврат суммарной длительности клипа
    {
        float sum = 0;
        for(int i = 0; i < frames.Length; i++)
            sum += frames[i].delay;
        return sum;
    }
    public void LoadFrames(Sprite [] sprites) // прогрузка кадров из массива спрайтов
    {
        frames = new AnimationFrame[sprites.Length];
        for(int i = 0; i< sprites.Length; i++){
            frames[i] = new AnimationFrame(sprites[i]);
        }  
    }   

    public Sprite LastFrame()
    {
        return frames[frames.Length-1].pic;
    }

    public bool HasSound() // Проверка есть ли звук у клипа
    {
        return sound != null;
    }

    public void SetDelay(float delay) // задаёт всем кадрам фиксированную задержку
    {
        foreach(AnimationFrame frame in frames)
            frame.delay = delay;
    }

    public void SetLength(float length) // задаёт всем кадрам такую задержку, чтобы общая длительность была равна заданной
    {
        float delay = length / frames.Length;
        SetDelay(delay);
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
