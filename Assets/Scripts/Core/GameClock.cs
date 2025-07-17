using UnityEngine;

public class GameClock : MonoBehaviour
{
    [SerializeField] uint ticks;

    void FixedUpdate()
    {        
        ticks++;
    }
}