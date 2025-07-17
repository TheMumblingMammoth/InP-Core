using System.Collections.Generic;
using Unity.Collections;
using Unity.Multiplayer.Center.Common.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
public class Bird : MonoBehaviour
{
    AnimatedSprite animatedSprite; // раздел настройки в юнити?
    [SerializeField] ClipSet clipSet; // поле - анимировация(множество спрайтов)
    [SerializeField] Vector2 timeRange;
    bool onLand = true; // проверка - нахождения на земле
                        //     float timer;
    private static List<Bird> Birds = new List<Bird>(10); // storage of birds on loc
    Vector3 Point; // точка 
    [SerializeField]  Vector3 nextPoint;
    bool flee = false; // побег
    void Awake()
    {
        Birds.Add(this);
        animatedSprite = GetComponent<AnimatedSprite>(); // получаем анимированый спрайт
    }

    void Start()
    {
        Point = transform.position;
        nextPoint = Point;
        animatedSprite.ChangeClip(clipSet.ClipFor("Eat")); // спрайт при старте 
    }


    void FixedUpdate()
    {
        if(animatedSprite.IsCasual())
            CheckState();
    }

    #region State
    // Проверка состояния
    void CheckState() // проверка следующего действия   
    {
        if (onLand)
        {
            if (PlayerControlUnit.InRange(transform.position, 5)) // 5
            {
                if (PlayerControlUnit.InRange(transform.position, 2)) TakeOff(); //2
                else { TurnTo(PlayerControlUnit.GetX()); Flapping(); }
                return;
            }
            if (!flee)
            {
                foreach (Bird otherB in Birds)
                {
                    if (otherB == this || !otherB.onLand) continue;
                    if (Vector3.Distance(otherB.transform.position, transform.position) < 2)
                    {
                        BirdInteraction(otherB);
                        return;
                    }
                }
            }
            WalkRnd();
        }
        else if (!PlayerControlUnit.InRange(transform.position, 7)) Landing(); // 5-7
    }

    void WalkRnd() // идти к случайной точке
    {

        if (transform.position == nextPoint) // если не занят, то
        {
            flee = false;
            float x = Random.Range(-3f, 3f);
            float y = 0f;//Random.Range(-3f, 3f);
            nextPoint = Point + new Vector3(x, y, 0f);
            TurnTo(nextPoint.x);
            animatedSprite.ChangeClip(clipSet.ClipFor("Walk"));
        }
        /*Короч... не робит из-за того что снегерированный поинт остаётся не смотря на встречу птиц и независимо от этого птица пытается до него дойти... и птицы цепляются друг за друга)*/
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, // перевод направления в 3д(z всегда равна 0)
                                    Time.fixedDeltaTime);
        if (transform.position == nextPoint)
        {
            TryEat();
        }//else if(animatedSprite.IsCasual()) transform.position = Vector3.MoveTowards(transform.position, nextPoint, // перевод направления в 3д(z всегда равна 0)
        //                             Time.fixedDeltaTime);
    }

    void WalkTo(Vector3 point) // Идти к указанной точке
    {
        transform.position = Vector3.MoveTowards(transform.position, point, Time.fixedDeltaTime);
        if (transform.position == point)
        {
            TryEat();
        }
    }

    void TryEat()
    {
        int rnd = Random.Range(1, 100);
        if (rnd < 30) animatedSprite.PlayOnce(clipSet.ClipFor("Flapping")); // 30%
        else if (rnd < 31) animatedSprite.PlayOnce(clipSet.ClipFor("Walk")); // 50% - 80
        else animatedSprite.PlayOnce(clipSet.ClipFor("Eat")); // 20%
    }
    
    void BirdInteraction(Bird otherB)
    {
        if (otherB.animatedSprite.CurrentClip() == "Flapping") // если другая птица пугает
        {
            // отвернуться
            TurnFrom(otherB.transform.position.x);
            flee = true;
            animatedSprite.ChangeClip(clipSet.ClipFor("Walk"));
            nextPoint = Vector3.MoveTowards(transform.position, otherB.transform.position, -2f);
        }
        else
        {
            TurnTo(otherB.transform.position.x);
            animatedSprite.PlayOnce(clipSet.ClipFor("Flapping"));
            //nextPoint = transform.position; // остаться на месте 
            return;
        }
        WalkRnd();
        //WalkTo(nextPoint);
    }
    void Flapping() // отпугивание
    {
        animatedSprite.ChangeClip(clipSet.ClipFor("Flapping"));
        //animatedSprite.PlayOnce(clipSet.ClipFor("Flapping"));
    }
    void TakeOff() // взлёт 
    {
        TurnFrom(PlayerControlUnit.GetX());
        animatedSprite.PlayOnceThenChange(clipSet.ClipFor("TakeOff"), Clip.NullClip()); // пока не работает из-за проверки в FIXEDUPDATE так что пока буду взлетать и менять на флапинг
        animatedSprite.CustomSpeed(1.5f);
        onLand = false;
    }
    void TurnTo(float x) // повернуться лицом к цели
    {
        if (x > transform.position.x) animatedSprite.Right();
        else animatedSprite.Left();
    }
    void TurnFrom(float x) // отвернуться от цели
    {
        if (x > transform.position.x ) animatedSprite.Left();
        else animatedSprite.Right();
    }
    void Landing() // посадка
    {
        animatedSprite.PlayOnceThenChange(clipSet.ClipFor("Landing"), clipSet.ClipFor("Walk"));
        onLand = true;
    }
    #endregion State
}


/*
animatedSprite.CurrentClip - возвращяет имя текущей анимации
*/