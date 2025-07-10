using UnityEngine;

public class Bird : MonoBehaviour
{
    AnimatedSprite animatedSprite; // раздел настройки в юнити?
    [SerializeField] ClipSet clipSet; // поле - анимировация(множество спрайтов)
    bool onLand = true; // проверка - нахождения на земле
    void Awake()
    {
        animatedSprite = GetComponent<AnimatedSprite>(); // получаем анимированый спрайт
    }

    void Start() 
    {
        animatedSprite.ChangeClip(clipSet.ClipFor("Eat")); // спрайт при старте 
    }

    void FixedUpdate()
    {
        CheckState();
    }

    #region State
    // Проверка состояния
    void CheckState() // проверка следующего действия   
    {
        if (onLand && PlayerControlUnit.InRange(transform.position, 5))
        {
            if (PlayerControlUnit.InRange(transform.position, 2)) TakeOff();
            else Flapping();
        }
        if (!onLand && !PlayerControlUnit.InRange(transform.position, 5)) Landing();
        //else Eat(); 
    }

    void Eat()
    {
        animatedSprite.PlayOnceThenStop(clipSet.ClipFor("Eat"));
    }
    void Flapping() // отпугивание
    {
        animatedSprite.ChangeClip(clipSet.ClipFor("Flapping"));
        //animatedSprite.PlayOnceThenStop(clipSet.ClipFor("Flapping"));
    }
    void TakeOff() // взлёт 
    {
        animatedSprite.PlayOnceThenStop(clipSet.ClipFor("TakeOff"));
        onLand = false;
    }
    void Landing() // посадка
    {
        animatedSprite.PlayOnceThenChange(clipSet.ClipFor("Landing"), clipSet.ClipFor("Eat"));
        onLand = true;
    }
    #endregion State
}
