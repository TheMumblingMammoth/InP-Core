using UnityEngine;

public class Bird : MonoBehaviour
{
    AnimatedSprite animatedSprite;
    [SerializeField] ClipSet clipSet;
    bool onLand = true;
    void Awake()
    {
        animatedSprite = GetComponent<AnimatedSprite>();
    }

    void Start()
    {
        animatedSprite.ChangeClip(clipSet.ClipFor("Eat"));
    }

    void FixedUpdate()
    {
        CheckState();
    }
    
     #region State
        // Проверка состояния
        void CheckState()
        {
            if (onLand && PlayerControlUnit.InRange(transform.position, 2)) TakeOff();        
            if (!onLand && PlayerControlUnit.InRange(transform.position, 5)) Landing();
        }

        void TakeOff()
        {
            animatedSprite.PlayOnceThenStop(clipSet.ClipFor("TakeOff"));
            onLand = false;
        }
        void Landing()
        {
            animatedSprite.PlayOnceThenChange(clipSet.ClipFor("Landing"), clipSet.ClipFor("Eat"));
            onLand = true;
        }
    #endregion State
}
