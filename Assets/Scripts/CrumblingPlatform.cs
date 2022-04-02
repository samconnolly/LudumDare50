using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    public Sprite[] crumbleFrames;
    private float spriteTimer = 0;
    private int frame = 0;

    public float frameRate;
    public float crumbleTime = 0.3f;
    private float crumbleTimer = 0;
    private bool aboutToCrumble = false;
    private bool crumbling = false;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (aboutToCrumble)
        {
            crumbleTimer += Time.deltaTime;
            if (crumbleTimer >= crumbleTime)
            {
                crumbling = true;
                GetComponent<Collider2D>().enabled = false;
            }
        }
        if (crumbling) {

            // animation
            spriteTimer += Time.deltaTime;
            if (spriteTimer >= (1 / frameRate))
            {
                frame += 1;
                spriteTimer = 0;
                if (frame < crumbleFrames.Length)
                {
                    GetComponent<SpriteRenderer>().sprite = crumbleFrames[frame];
                }
                else
                {
                    crumbling = false;
                    GetComponent<SpriteRenderer>().enabled = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // only crumble if the player falls onto the platform, not hits it from below
            if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0)
            {
                aboutToCrumble = true;
            }            
        }
    }

    public void Reset()
    {
        crumbleTimer = 0;
        crumbling = false;
        aboutToCrumble = false;
        gameObject.SetActive(true);
        frame = 0;
        spriteTimer = 0;
        GetComponent<SpriteRenderer>().sprite = crumbleFrames[frame];
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }
}
