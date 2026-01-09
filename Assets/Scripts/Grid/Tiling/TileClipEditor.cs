using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TileClip)), CanEditMultipleObjects]
public class TileClipEditor : Editor
{
	static string lastPath;
	public override void OnInspectorGUI()
	{
		TileClip[] clips = new TileClip[targets.Length];
		for (int i = 0; i < clips.Length; i++)
			clips[i] = (TileClip)targets[i];
		//Clip clip = (Clip)target;
		if (GUILayout.Button("LoadFromFolder"))
		{
			string path;
			if (lastPath == null)
				path = EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath + "/Sprites", "");
			else
				path = EditorUtility.OpenFolderPanel("Choose Folder", lastPath, "");
			if (path != null)
				if (path != "")
				{
					clips[0].LoadFrames(ImportSprites(path));
					clips[0].name = path.Split('/')[path.Split('/').Length - 1];
					lastPath = path.Remove(path.Length - clips[0].name.Length);
				}

		}

		DrawDefaultInspector();

		if (GUI.changed)
		{
			foreach (TileClip cl in clips)
			{
				EditorUtility.SetDirty(cl);
				EditorSceneManager.MarkSceneDirty(cl.gameObject.scene);
			}
		}
	}

	public static TileClip Load(string path, TileClip clip)
	{
		if (path != null)
			if (path != "")
			{
				clip.LoadFrames(ImportSprites(path));
				clip.name = path.Split('/')[path.Split('/').Length - 1];
				lastPath = path.Remove(path.Length - clip.name.Length);
			}
		return clip;
	}
	
	static Sprite[] ImportSprites(string path)
	{
        string [] frameNames = Directory.GetFiles(path,"*.png");
        Sprite [] sprites = new Sprite[frameNames.Length];
        path = path.Remove(0, (Application.dataPath).Length);
        path = path +'/'+ path.Split('/')[path.Split('/').Length-1];
        for(int i =0; i< frameNames.Length;i++){
            //Debug.Log("Assets"+path+i+".png");
            sprites[i] =  (Sprite) AssetDatabase.LoadAssetAtPath("Assets"+path+(i+1)+".png",typeof(Sprite));
            //Debug.Log("Assets"+path+(i+1)+".png");
        }
        return sprites;
    }

}

