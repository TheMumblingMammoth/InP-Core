using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Clip)), CanEditMultipleObjects]
public class ClipEditor : Editor {
	static string lastPath;
	[SerializeField] float delay = 0.25f;
	[SerializeField] float length = 0.75f;
	public override void OnInspectorGUI(){
		Clip[] clips = new Clip[targets.Length];
		for(int i = 0; i < clips.Length; i++)
			clips[i] =  (Clip)targets[i];
		//Clip clip = (Clip)target;
		if(GUILayout.Button("LoadFromFolder")){
			string path;
			if(lastPath == null)
				path = EditorUtility.OpenFolderPanel("Choose Folder",Application.dataPath+"/Sprites","");	
			else
				path = EditorUtility.OpenFolderPanel("Choose Folder", lastPath ,""); 
			if(path != null)
				if(path != ""){
					clips[0].LoadFrames(FrameImporter.ImportSprites(path));
					clips[0].name = path.Split('/')[path.Split('/').Length - 1];
					lastPath = path.Remove(path.Length - clips[0].name.Length);
				}
					
		}
		
		DrawDefaultInspector();	
		delay  =  EditorGUILayout.FloatField("Delay", delay);
		if(GUILayout.Button("Set Delay"))
			foreach(Clip cl in clips)
				cl.SetDelay(delay);
		//length = clips.Length == 1 ? clips[0].Length() : 0.75f;
		float l = EditorGUILayout.FloatField("Current Length", clips.Length == 1 ? clips[0].Length() : 0.75f);
		length  =  EditorGUILayout.FloatField("Set Length to", length);
		if(GUILayout.Button("Set Length"))
			foreach(Clip cl in clips)
				cl.SetLength(length);

		if (GUI.changed){
			foreach(Clip cl in clips){
				EditorUtility.SetDirty(cl);
				EditorSceneManager.MarkSceneDirty(cl.gameObject.scene);
			}
        }
	}

	public static Clip Load(string path, Clip clip){
		if(path != null)
			if(path != ""){
				clip.LoadFrames(FrameImporter.ImportSprites(path));
				clip.name = path.Split('/')[path.Split('/').Length - 1];
				lastPath = path.Remove(path.Length - clip.name.Length);
			}
		return clip;
	}

}

