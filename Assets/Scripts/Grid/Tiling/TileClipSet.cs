using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileClipSet : MonoBehaviour{
    [SerializeField] List<TileClip> clips; public TileClip[] GetClips(){ return clips.ToArray(); }

    public void ClearClips(){ clips.Clear(); }
    public TileClip ClipFor(string clipName){
        foreach(TileClip clip in clips)
            if(clip.name.Equals(clipName))
                return clip;

        foreach(TileClip clip in clips)
            if(clip.name.Contains(clipName))
                return clip;

        
        Debug.Log("No "+ clipName +" clip in " + name +" ClipSet");
        return null;
    }

    public TileClip ClipFor(int i){
        if(clips.Count > i)
            return clips[i];        
        Debug.Log("No "+ i.ToString() +" clip in " + name +" ClipSet");
        return null;
    }

    public bool HasClipFor(string actionName){
        foreach(TileClip clip in clips)
            if(clip.name.Equals(actionName))
                return true;
        return false;
    }

    public void AddClip(TileClip clip){
        clips.Add(clip);
    }


    public static string Find(DirectoryInfo root, string name){
        FileInfo[] files = root.GetFiles();
		foreach(FileInfo file in files)
			if(file.Name.Equals(name))
				return file.FullName;
		
		string path;
		//Debug.Log("searched files " + files.ToString());
		DirectoryInfo[] folders = root.GetDirectories();
		foreach(DirectoryInfo folder in folders){
			path = Find(folder, name);
			if(path != null)
				return path;
		}
		//Debug.Log("searched directories " + folders.ToString());
		return null;
	}

    
}