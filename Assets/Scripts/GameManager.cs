using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Guy player;
    public Scroll scroll;
    public float scrollGenInterval;
    public int maxScrolls;
    public StatBar researchCounter;
    public Zombie zombie;
    public float researchInterval;
    public int townResearchRate;
    public int collectableResearchRate;
    public int winScore;
    private float researchTimer;
    private float scrolltimer;
    private int nScrolls;
    public Tilemap groundTileMap;
    public Tilemap topTileMap;
    public Tilemap minimapTileMap;
    public TileBase thornTile;
    private ThornGrowth thornGrowth;

    private int towns = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameHelper.gameManager = this;
        thornGrowth = topTileMap.GetComponent<ThornGrowth>();
        researchCounter.SetMax(winScore);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameHelper.gameRunning) {
            researchTimer += Time.deltaTime;

            if (researchTimer >= researchInterval) {
                researchCounter.Add(towns * townResearchRate);
                researchTimer = 0;
            }

            if (researchCounter.Value >= winScore) {
                GameHelper.WinGame();
            }

            scrolltimer += Time.deltaTime;

            if (scrolltimer >= scrollGenInterval) {
                if (nScrolls < maxScrolls) {
                    Vector3 position = thornGrowth.RandomAccessiblePosition();
                    Instantiate(scroll, position, Quaternion.identity);
                    nScrolls += 1;
                }
                scrolltimer = 0;
            }
        }
    }
    public void LoseTown(){
        towns -= 1;      
        if (towns == 0) {
            GameHelper.LoseGame();
        }
    }
    public int Towns{
        get {return towns;} set {towns = value;}
    }

    public void CollectResearch(){
        researchCounter.Add(collectableResearchRate);  
        nScrolls -= 1;      
    }

}
