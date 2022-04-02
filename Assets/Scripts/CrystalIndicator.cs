using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalIndicator : MonoBehaviour
{
    public Sprite emptySprite;
    public Sprite fullSprite;
    
    public void Fill()
    {
        GetComponent<SpriteRenderer>().sprite = fullSprite;
    }

    public void Empty()
    {
        GetComponent<SpriteRenderer>().sprite = emptySprite;
    }
}
