using System;
using UnityEngine;

[Serializable]
public class AnimationFrame{
    public Sprite pic;
    public float delay=0.25f;
    public AnimationFrame(Sprite sprite){
        pic = sprite;
        delay = 0.25f;
    }
}
