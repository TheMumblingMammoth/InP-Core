using UnityEngine;
using System;

public class SpriteManager: MonoBehaviour{
    public static Sprite null_sprite = Resources.Load<Sprite>("Sprites/Icons/null");
    //public static Sprite item_light_sprite = Resources.Load<Sprite>("Sprites/Icons/ItemLight");
    //public static Sprite item_focused_sprite = Resources.Load<Sprite>("Sprites/Icons/ItemFocused");
    public static Color full = new Color(1,1,1),
                        dull = new Color(0.8f,0.8f,0.8f),
                        red = new Color(1f,0f,0f),
                        black = new Color(0f,0f,0f);
    
    void Awake(){
        /*capitals = precapitals;
        blazon = Loader.LoadNewSprite(@"\Sprites\Icon\Blazon.png", 64);
        
        System.Array allLands = Enum.GetValues(typeof(Land));
        lands = new NamedSprite[allLands.Length];
        for(int i = 0; i< lands.Length; i++)
			lands[i] = new NamedSprite(Loader.LoadNewSprite(@"\Sprites\Icon\"+ lands[i].name + ".png", 64),allLands.GetValue(i).ToString());

        frames = new Sprite[21];
            for(int i = 0; i<=20; i++)
              frames[i] = Loader.LoadNewSprite(@"\Sprites\Front\Frame\"+ i + ".png", 64);*/
    }

    public static void Init(){
        //null_sprite = Resources.Load<Sprite>("Sprites/Icons/null");
        //null_sprite = Resources.Load<Sprite>("Sprites/Icons/null");
    }
    

    public static void AddFrameSprite(string name, Transform parent, Vector3 position, out SpriteRenderer sprite, out SpriteRenderer frame){
        GameObject g = new GameObject(name);
		g.transform.parent = parent;
		g.transform.position = position;
		g.AddComponent<SpriteRenderer>();
        sprite = g.GetComponent<SpriteRenderer>();
        
        GameObject f = new GameObject("frame");
        f.transform.parent = g.transform;
        f.AddComponent<SpriteRenderer>();
        frame = f.GetComponent<SpriteRenderer>();
        f.transform.localPosition = new Vector3(-0.25f,-0.25f,0.01f);
    }
}