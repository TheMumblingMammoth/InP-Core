using UnityEngine;
using System.Collections.Generic;

public class AnimatedSprite : MonoBehaviour{
    
    public SpriteRenderer sprite {get; private set;}
    [SerializeField] private Clip clip; // анимация
    private Clip futureClip = null; // хранилище "вспомогательной" анимации 
    public bool Casual() { return futureClip == null; } // очищает хранилище "вспомогательной" анимации
    [SerializeField] float step; // хз честно)
    [SerializeField] byte stage; // номер спрайта(кадра)
    public byte GetStage(){ return stage; } // получить номер кадра 
    public void SetStage(byte stage){  // установить кадр
        this.stage = stage;
        if(clip.frames.Length <= stage) this.stage = 0;
    }
	
    public float animspeed {get; private set;} = 1; // скорость проигрывания анимации
	bool forward = true; // направление проигрывания анимации TRUE - слева направо, FALSE - задом на перёд(справа налево)
    public bool ended = false; // проверка конца анимации
    bool stop = false; // проверка прекращения анимации
    public float rng = 0;
    List<DelayedClip> delayed_clips; // список отложенных анимаций
    bool playing_delayed = false;
    public bool pause {get; private set;} // проверка остановки анимации
    #region Options
        public void Stop(){ ended = true; } // конец анимации
        public void Pause(){ pause = true;} // остановка анимации
        public void Resume(){ pause = false;} // возобнавление анимации
        public void NormalSpeed(){ animspeed = 1; } // установка стандартной скорости - "1"
        public void DoubleSpeed(){ animspeed = 2; } // установка скорости - "2"
        public void CustomSpeed(float speed){ animspeed = speed; } // установка своего значения скорости анимации
    #endregion Options
    public void Awake(){
        sprite = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();
        delayed_clips = new List<DelayedClip>(1);
        if(sprite == null)
            Debug.Log(" You forgot SpriteRenderer!");
        
    }
  
    void Update(){
        if(ended || pause)
            return;

        if(clip==null){
            return;
        }

        UpdateSound();

        
        if(rng > 0)
            step += Time.deltaTime * Core.TimeScale() * animspeed * Random.Range(1f - rng, 1f + rng);    
        else   
            step += Time.deltaTime * Core.TimeScale() * animspeed;
        
        DelayedClip [] temp = delayed_clips.ToArray();
        for(int i = 0; i < temp.Length; i++){
            temp[i].delay -= animspeed;
            if(temp[i].delay <=0 ){
                PlayOnce(temp[i].clip);
                playing_delayed = true;
                delayed_clips.Remove(temp[i]);
            }
        }
        
        try{
            while(step >= clip.frames[stage].delay){
                step -= clip.frames[stage].delay;
                NextFrame();
            }
        }catch{
            Debug.Log(clip.name + " " + stage + " stage is not found");
        }
    }

    void NextFrame(){
        switch(clip.type){
            case AniType.Circle:
                stage++;
                if(stage == clip.frames.Length){ // last frame on circle
                    if(futureClip != null  || stop)  SwapFuture();
                    stage = 0;
                    step = 0;
                }                
            break;
            case AniType.Pong:
            
                if(forward){
                    stage++;
                    if(stage == clip.frames.Length-1){
                        forward = false;
                    }
                }else{
                    stage--;
                    if(stage == 0){ // last frame on pong
                        forward = true;
                        step = 0;
                        if(futureClip != null || stop)  SwapFuture();
                    }   
                }
            break;
        }        
        if(!ended)
            sprite.sprite = clip.frames[stage].pic;        
    }

    public bool FirstFrame(){ // проверка - первый ли кадр
        return stage == 0 && step == 0;
    }

    public bool LastFrame(){ // проверка - последний ли кадр
        return stage == clip.frames.Length - 1 && step >= clip.frames[clip.frames.Length - 1].delay - animspeed;
    }
    public string CurrentClip(){ // получение названия текущей анимации
        if(clip != null)
            return clip.name;
        else
            return "";
    }
    public void ChangeClip(Clip newClip){ // замена текущей анимации на другую
        if(newClip == clip)
            return;
        playing_delayed = false;
        clip = newClip;
        stop = false;
        stage = 0;
        forward = true;
        if(clip == null)
            Debug.Log("Clip == null");
        try{
            sprite.sprite = clip.frames[0].pic;
        }catch{
            Debug.Log(clip.name);
            sprite.sprite = clip.frames[0].pic;
        }
        ended = false;
    }
    public void PlayOnce(Clip newClip){ // проиграть анимацию один раз
        if(!playing_delayed)
            futureClip = clip;
        clip = newClip;
        step = 0;
        forward = true;
        stage = 0;
        sprite.sprite = clip.frames[stage].pic;
        ended = false;       
    }
    public void PlayOnceThenChange(Clip newClip, Clip nextClip){ // пороиграть анимацию один раз, затем заменить на другой 
        futureClip = nextClip;
        clip = newClip;
        stop = false;
        stage = 0;
        forward = true;
        sprite.sprite = clip.frames[0].pic;
        ended = false;
    }
    public void PlayOnceThenStop(Clip newClip){ // проиграть анимацию один раз и остановиться на первом кадре
        futureClip = clip;
        stop = true;
        clip = newClip;
        stage = 0;
        forward = true;
        sprite.sprite = clip.frames[0].pic;
        ended = false;
    }
    public void PlayClipLater(Clip newClip, int delay, bool stop){
        delayed_clips.Add(new DelayedClip(newClip, delay, stop));
    }
    private void SwapFuture(){ // сменить нынешнюю анимацию на "вспомогательную"(обычно конец анимации)
        if(stop){
            ended = true;
        }else{
            clip = futureClip;
            futureClip = null;
        }
        playing_delayed = false;
    }
    public void SetOrder(string layer, int order){
        sprite.sortingLayerName = layer;
        sprite.sortingOrder = order;
    }

    public void Left(){
        sprite.flipX = false;
    }
    public void Right(){
        sprite.flipX = true;
    }

    public void LastFrameOf(Clip newClip){ // установить последний кадр из анимации
        clip = newClip;
        futureClip = null;
        stage = (byte)(clip.frames.Length - 1);
        forward = true;
        sprite.sprite = clip.frames[stage].pic;
        ended = true;
    }
    

    #region Sound
        [SerializeField] bool constantSound;
        [SerializeField] AudioSource sound;
        public void LoadSound(){
            //sound = Instantiate<LocalSound>(Resources.Load<LocalSound>("Sound/Local Audio Source")).GetComponent<AudioSource>();
            sound.transform.parent = gameObject.transform;
            sound.transform.localPosition = new Vector3(0, 0, 0);
        }
        void UpdateSound(){
            if(sound == null) return;
            if(sound.clip != clip.sound){
                sound.clip = clip.sound;
                sound.Play();
            }
        }
    #endregion Sound

}
