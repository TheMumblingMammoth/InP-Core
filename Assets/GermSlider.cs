using UnityEngine;
using UnityEngine.UI;

public class GermSlider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        if(gameObject.name == "Slider")
            ChangeSpeed();
        else
            ChangeColor();
    }
    public void ChangeSpeed()
    {
        Germ.SetSpeed(GetComponent<Slider>().value);
    }

    public void ChangeColor()
    {
        Germ.SetAlpha(GetComponent<Slider>().value);
    }
}
