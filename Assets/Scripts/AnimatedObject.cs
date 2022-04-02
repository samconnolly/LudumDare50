using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObject : MonoBehaviour
{
    public Sprite[] frames;
    private float spriteTimer = 0;
    private int frame = 0;
    public float frameRate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spriteTimer += Time.deltaTime;
        if (spriteTimer >= (1 / frameRate))
        {
            frame += 1;
            if (frame == frames.Length)
            {
                frame = 0;
            }
            GetComponent<SpriteRenderer>().sprite = frames[frame];
            spriteTimer = 0;
        }
    }
}
