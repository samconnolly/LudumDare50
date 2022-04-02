using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    private bool collected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (collected && ! GetComponent<AudioSource>().isPlaying)
        {
            Destroy(gameObject);
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collected && collision.gameObject.tag == "Player")
        {
            Collect();
        }
    }

    private void Collect()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<AudioSource>().Play();
        collected = true;
        GameHelper.gameManager.AddCrystal();
    }
}
