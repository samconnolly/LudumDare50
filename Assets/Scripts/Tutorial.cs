using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Tutorial : MonoBehaviour
{
    public float readTime;
    public Image background;
    public GameObject tileHalo;
    private Text uiText;
    private bool visible;
    private float readTimer;
    private GameObject currentHalo;

    private int tooltipIndex;
    private string[] tooltipText = new string[]{
        "Use WASD to move. Use the arrow keys to face a different direction to movement.",
        "This is your health, don't let it get to zero!",
        "This is your research score, fill the bar to discover the source of the thorns.",
        "This is a town, it will produce research for you, unless it is detroyed by the thorns.",
        "This shows the state of all the towns in the land - if all of them are destoryed, it will spell your doom.",
        "It's a thorn reanimate! Attack with space, or it will get you first!",
        "These are the dread thorns, attack them to chop them down.",
        "This is a scroll, collect them to boost your research score."
    };
    private string[] triggerTags = new string[]{null, "HP", "Research", "Town", "TownCounter", "Zombie", "Thorns", "Scroll"};

    // Start is called before the first frame update
    void Start()
    {
        uiText = GetComponent<Text>();
        currentHalo = GameHelper.Player.transform.GetChild(0).gameObject;
        visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (visible){
            readTimer += Time.deltaTime;
            if (readTimer >= readTime) {
                EndTooltip();
                readTimer = 0;            
            }
        }
        else if (tooltipIndex < tooltipText.Length) {
            if (triggerTags[tooltipIndex] == "Thorns"){
                (bool found, Vector3 pos) = CheckForTileOnscreen(GameHelper.ThornTile);
                if (found) {
                    Debug.LogFormat("Found thorn tile");
                    currentHalo = Instantiate(tileHalo, pos, Quaternion.identity);
                    NextTooltip();
                }
            }
            else
            {
                GameObject found = CheckForTagOnscreen(triggerTags[tooltipIndex]);
                if (found != null) {
                    Debug.LogFormat("Tooltip object: {0}", found);
                    currentHalo = found.transform.GetChild(0).gameObject;
                    NextTooltip();
                }
            }
        }
    }

    void EndTooltip(){
        currentHalo.SetActive(false);
        background.enabled = false;
        uiText.text = "";
        tooltipIndex += 1;
        visible = false;
    }

    void NextTooltip() {
        currentHalo.SetActive(true);
        background.enabled = true;
        uiText.text = tooltipText[tooltipIndex];
        visible = true;
    }

    private GameObject CheckForTagOnscreen(string tag) {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        GameObject bestObj = null;
        float bestDist = Mathf.Infinity;
        foreach (GameObject obj in objs) {
            if ((obj.GetComponent<Renderer>() != null && obj.GetComponent<Renderer>().isVisible) ||
                (obj.GetComponent<CanvasRenderer>() != null)){
                float newDist = (obj.transform.position - GameHelper.Player.transform.position).magnitude;
                if (bestObj == null || newDist < bestDist) {
                    bestObj = obj;
                    bestDist = newDist;
                }            
            }
        }
        return bestObj;
    }

    private (bool, Vector3) CheckForTileOnscreen(TileBase findTile){
        Vector3 playerPos = GameHelper.Player.transform.position;
        Vector3Int playerCell = GameHelper.TopTileMap.WorldToCell(playerPos);

        Vector3 bestPos = Vector3.zero;
        float bestDist = Mathf.Infinity;
        bool found = false;
        for (int i=-1; i<=1; i++) {
            for (int j=-1; j<=1; j++){
                Vector3Int searchCell = new Vector3Int(playerCell.x + i, playerCell.y + j, playerCell.z);
                TileBase tile = GameHelper.TopTileMap.GetTile(searchCell);
                if (tile == findTile) {
                    Vector3 tilePos = GameHelper.TopTileMap.CellToWorld(searchCell) + new Vector3(0.5f, 0.5f, 0);
                    float newDist = (tilePos - GameHelper.Player.transform.position).magnitude;
                    if (newDist < bestDist) {
                        bestPos = tilePos;
                        bestDist = newDist;
                        found = true;
                    }
                }
            }
        }
        return (found, bestPos);
    }
}
