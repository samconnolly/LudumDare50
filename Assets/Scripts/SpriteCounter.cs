using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCounter : MonoBehaviour
{
    public Sprite[] sprites; // ordered from max to zero
    private Image image;
    private int maxValue;
    private int value;

    // Start is called before the first frame update
    void Start()
    {
        maxValue = sprites.Length - 1;
        value = maxValue;
        image = GetComponent<Image>();
        image.sprite = sprites[0];
    }

    // Update is called once per frame
    public void SetValue(int v) {
        value = v;
        image.sprite = sprites[maxValue - value];
    }

    public void Change(int c) {
        SetValue(value + c);
    }
}
