using UnityEngine;

public class LimbTest : MonoBehaviour
{
    Limb limb;
    bool left, leg;
    void Awake()
    {
        limb = GetComponent<Limb>();
    }
    void Update()
    {
        if(Input.GetMouseButton(left?1:0) && leg == Input.GetKey(KeyCode.LeftControl)){
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                limb.Flip();
                
            }
            limb.SetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition), limb.alpha + Input.mouseScrollDelta.y * 10);
        }
    }
}