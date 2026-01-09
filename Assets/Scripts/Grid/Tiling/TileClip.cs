using UnityEngine;
public class TileClip : MonoBehaviour{
    
    [SerializeField] public Sprite [] frames;
    
    public void LoadFrames(Sprite [] sprites) // прогрузка кадров из массива спрайтов
    {
        frames = new Sprite[sprites.Length];
        for(int i = 0; i< sprites.Length; i++)
        {
            frames[i] = sprites[i];
        }  
    }   

}
