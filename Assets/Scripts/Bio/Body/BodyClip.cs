using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class BodyClip{
    const int LimbsCount = 7;
    public float[] time {get; private set;}
    BodyFrame [] frames;
    public int size {get; private set;}
    BodyClip(int size){
        this.size = size;
        time = new float[size];
        frames = new BodyFrame[size];
    }
    BodyClip(int size, string [] lines){
        this.size = size;
        time = new float[size];
        frames = new BodyFrame[size];
        for(int i = 0; i < size; i++){
            time[i] = float.Parse(lines[i].Split(":")[0]);
            frames[i] = BodyFrame.Parse(LimbsCount, lines[i].Split(":")[1]);
        }
    }

    public BodyFrame GetFrameByNumber(int i){   return frames[i];   }


    public int GetFrameNumber(float timer){
        int i = 0;
        while(timer > time[i]){
            timer -= time[i];
            i++;
            i = i % time.Length;
        }
        return i > 0 ? i - 1 : size - 1;
    }
    public static Dictionary<string, BodyClip> clips;
    public static ClipSet skins { get; private set; }  = Resources.Load<ClipSet>("Clips/Bodies/Human/HumanClipSet");
    public static ClipSet childSkins { get; private set; }
    public static void Init() {
        Debug.Log("Body Init");
        skins = Resources.Load<ClipSet>("Clips/Bodies/Human/HumanClipSet");
        //childSkins = Resources.Load<ClipSet>("Clips/Bodies/Humachild/HumachildClipSet");
        clips = new Dictionary<string, BodyClip>(100);
        BodyClipXML clipData = BodyClipXML.LoadClips();
        foreach (BodyClipXML.XMLData data in clipData.clips) {
            clips.Add(data.name, new BodyClip(data.size, data.frames));
        }
    }

    public float Length(){
        float sum = 0;
        for(int i = 0; i < time.Length; i++)
            sum += time[i];
        return sum;
    }
   
}