using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class Core : MonoBehaviour{
    public const int PPU = 32;
    public static bool IsTester = false;
    public static bool CapsLock = false;

    #region Profile
    
    #endregion Profile    
    private static float timeScale = 1;
    public static float TimeScale(){   return timeScale;  }



    #region Random
        public struct RandomToss{
            public string occasion;
            public float value;
            public RandomToss(string occasion, float value){
                this.occasion = occasion;
                this.value = value;                
            }
        }
        public static List<RandomToss> tosses = new List<RandomToss>(20);
        public static bool HasRandomToss(string occasion){
            foreach(RandomToss toss in tosses)
                if(toss.occasion.Equals(occasion))
                    return true;
            return false;
        }
        public static float GetRandomToss(string occasion){
            foreach(RandomToss toss in tosses)
                if(toss.occasion.Equals(occasion))
                    return toss.value;
            return 1;
        }
    #endregion Random

    //public static Settings settings;
    //public static Game game;
    //public static UserInterface ui;
    
    public static bool gameOn;
    public static bool isServer;
    static bool initComplete = false; 
    public static AudioMixer mixer {get; set;}
    //public static MapPresetCollection mapPresetCollection {get; private set;}
    
    void InfoLoading(){
        //CursorManager.Initialize();
        //TextManager.Initialize(TextManager.TextLanguage.Eng);
        /*
        Dialogs.Initialize();
        
        SkillWheel.Load();
        */
        //Executor.worldPath = Application.dataPath+ @"\Worlds\"; // unleash for export
        //game = FindObjectOfType<Game>();
        /*
        
        craftBook = CraftBook.Load();
        
        */
    }
    
    void Initialisation(){        
        DontDestroyOnLoad(gameObject);
        InfoLoading();
        //BodyClip.Init();
        //Settings.LoadSettings();
        //SpriteManager.Init();
        /*Instantiate(Resources.Load<VE_Manager>("Misc/VE"));
        VE_Manager.Initialize();
        
        /*
        AudioManager.Initialize();
        mixer = Resources.Load<AudioMixer>("Sound/AudioMixer");
        if(Mixer.ambientSound == null)   Instantiate(Resources.Load<AmbientSound>("Sound/Ambient Audio Source")).Bind();
        if(Mixer.globalSound == null)   Instantiate(Resources.Load<GlobalSound>("Sound/Global Audio Source")).Bind();
        if(Mixer.globalSound_ui == null)   Instantiate(Resources.Load<UISound>("Sound/UI Audio Source")).Bind();
        */
        
        //profile = Profile.LoadProfile();
        //settings = new Settings();
        //Settings.SaveSettings();
        
        
        //TextManager.Initialize(settings.language);
       
        //
        initComplete = true;
    }
    
    static bool reset;
    static float reset_timer;
    public static void ResetCore(){
        reset = true;
        reset_timer = 0.5f;
    }

    void Awake(){
        if(initComplete)
            DestroyImmediate(gameObject);
        else
            Initialisation();
        //Settings.SetResolution();
        //VE_Manager.SetGamma();
    }

    void Update(){
        if(reset){
            reset_timer -= Time.deltaTime;
            if(reset_timer <= 0){
                Core.gameOn = false;
                Core.loading = false;
                //LobbyPanel.ResetNames();
                Core.isServer = false;
                //Core.player = null;
                //Core.game = null;
                Core.reset = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.KeypadMultiply)){
            timeScale*=2;
            if(timeScale == 0)
                timeScale = 1;
        }
        if(Input.GetKeyDown(KeyCode.KeypadDivide))
            timeScale/=2;
            
    }


    public static float FloatTowards(float start, float end, float step){ 
        if(Mathf.Abs(end - start) <= step)
            return end;
        if(end > start)
            return start + step;
        else
            return start - step;
    }
    public static string load_file_name {get; private set;}
    public static bool loading {get; private set;} = false;
    public static void Load(string load_file){
        load_file_name = load_file;
        loading = true;
        SceneManager.LoadScene("LobbyScene");
    }
    
}