using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public string valueName;
    public int startValue;
    private int _value;
    private Text text;

    private void Start() {
        text = GetComponent<Text>();
        Set(startValue);
    }
    public int Value {
        get {return _value;}
    }

    public void Set(int v) {
        _value = v;
        text.text = valueName + ": " + _value.ToString();
    }
    public void Add(int v) {
        Set(_value + v);
    }

}
