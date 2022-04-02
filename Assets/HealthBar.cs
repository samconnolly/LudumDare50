using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float maxValue;
    private float value;
    private Image image;
    private float width;
    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        value = maxValue;
        image = GetComponent<Image>();
        width = image.rectTransform.rect.width;
        pos = image.rectTransform.position;
    }

    public int Value{
        get {return (int) value;}
    }

    public void SetValue(float newValue) {
        value = newValue;
        float scale = (value / maxValue);
        image.rectTransform.localScale = new Vector3(scale, 1, 1);
        image.rectTransform.position = pos + new Vector3(width * ((-1 + scale)/ 2), 0, 0);        
    }
    public void Subtract(float sub) {
        float newValue = value - sub;
        if (newValue < 0) {
            newValue = 0;
        }
        SetValue(newValue);
    }

}
