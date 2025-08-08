using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="AudioClipSet", menuName = "AudioClipSet", order = 1)]
public class AudioClipSet : ScriptableObject {
    [SerializeField]
    public AudioClip [] clips;
    public bool Has(string clipName){
        foreach(AudioClip clip in clips)
            if(clip.name == clipName)
                return true;
        return false;
    }
    public AudioClip GetClip(string clipName){
        foreach(AudioClip clip in clips)
            if(clip.name == clipName)
                return clip;
        return null;
    }
}