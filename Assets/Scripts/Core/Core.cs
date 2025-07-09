using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class Core : MonoBehaviour{ // Ядро
    public const int PPU = 32; // Pixel per unit
    public static bool IsTester = false; // Хз) 0 использований пока
    public static bool CapsLock = false; // проверка нажатого капслока?

    #region Profile
    
    #endregion Profile    
    private static float timeScale = 1; // Множетель длительности времени(для слоумо или ускорения)PRIVAT
    public static float TimeScale(){   return timeScale;  } // Getter for param - timeScale



    #region Random
        public struct RandomToss{ // структура со string - "случай" и float - "значение"
            public string occasion;
            public float value;
            public RandomToss(string occasion, float value){ // Конструктор с параметрами
                this.occasion = occasion;
                this.value = value;                
            }
        }
        public static List<RandomToss> tosses = new List<RandomToss>(20); // Список из 20 структур RandomToss
        public static bool HasRandomToss(string occasion){ // Проверка есть ли у Core заданный "случай" в списке tosses
            foreach(RandomToss toss in tosses)
                if(toss.occasion.Equals(occasion))
                    return true;
            return false;
        }
        public static float GetRandomToss(string occasion){ // Возвращает "значение" выбранного "случая"
            foreach(RandomToss toss in tosses)
                if(toss.occasion.Equals(occasion))
                    return toss.value;
            return 1;
        }
    #endregion Random

    //public static Settings settings;
    //public static Game game;
    //public static UserInterface ui;
    
    public static bool gameOn; // Работает ли игра
    public static bool isServer; // Проверка - запущен ли сервер
    static bool initComplete = false; // Проверка - создан ли мир
    public static AudioMixer mixer {get; set;} // предположение: загрузка аудио дорожки?
    //public static MapPresetCollection mapPresetCollection {get; private set;}
    
    void InfoLoading(){ // загрузка информации
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
    
    void Initialisation(){ // создание мира
        //DontDestroyOnLoad(gameObject);
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
    
    static bool reset; // Переменная для запуска перезагрузки
    static float reset_timer; // Таймер для перезагрузки
    public static void ResetCore(){ // Перезапустить ядро
        reset = true;
        reset_timer = 0.5f;
    }

    void Awake(){
        if(initComplete) // если создание завершено - уничтожить игровой объект  
            DestroyImmediate(gameObject); // пока ничего не делает
        else // иначе - создать мир
            Initialisation(); 
        //Settings.SetResolution();
        //VE_Manager.SetGamma();
    }

    void Update(){
        if(reset){ // если нужно перезапустить
            reset_timer -= Time.deltaTime; // отсчёт задержки(таймера)
            if(reset_timer <= 0){
                Core.gameOn = false; // выключить игру?
                Core.loading = false;
                //LobbyPanel.ResetNames();
                Core.isServer = false;
                //Core.player = null;
                //Core.game = null;
                Core.reset = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.KeypadMultiply)){ // нажав * тячение времяни ускориться в 2 раза
            timeScale*=2;
            if(timeScale == 0)
                timeScale = 1;
        }
        if(Input.GetKeyDown(KeyCode.KeypadDivide)) // нажав / тячение времяни замедлится в 2 раза
            timeScale/=2;
            
    }


    public static float FloatTowards(float start, float end, float step){ // возвращает float значение - либо конец (если шаг слишком велик), либо старт+-шаг (в зависимости от конца)
        if(Mathf.Abs(end - start) <= step)
            return end;
        if(end > start)
            return start + step;
        else
            return start - step;
    }
    // get - публичное чтение значения. private set — установка значения разрешена только внутри класса.
    public static string load_file_name { get; private set; } // загрузка файла(сцены)
    public static bool loading {get; private set;} = false; // проверка - идёт ли загрузка
    public static void Load(string load_file){ // загрузка сцены
        load_file_name = load_file;
        loading = true;
        SceneManager.LoadScene("LobbyScene");
    }
    
}