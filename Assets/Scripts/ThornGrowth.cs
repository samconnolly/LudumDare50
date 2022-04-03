using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ThornGrowth : MonoBehaviour
{
    public float growthInterval;
    public float spawnInterval;
    public float perTileSpawnChance;
    public TileBase thornTile;
    public TileBase townTile;
    public TileBase ruinedTownTile;
    public Vector2Int offset;
    public List<TileBase> grassTiles;
    public List<TileBase> dirtTiles;
    public List<TileBase> forestTiles;
    public List<TileBase> riverTiles;
    public List<TileBase> roadTiles;
    public List<TileBase> mountainTiles;
    
    private int[,] groundMap;
    private int[,] growth;
    private int[,] newGrowth;
    private int[,] possibleGrowth;
    private int[,] miniMap;
    private int grassProb = 2;
    private int dirtProb = 3;
    private int forestProb = 3;
    private int riverProb = 1;
    private int roadProb = 1;
    private int mountainProb = 1;
    private float thornSpeedMult = 0.5f;
    private float grassSpeedMult = 1.0f;
    private float roadSpeedMult = 1.5f;
    private float forestSpeedMult = 0.75f;
    private float mountainSpeedMult = 0.25f;
    private int mapSize;

    private float growthTimer;
    private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        mapSize = GameHelper.mapSize;
        growth = new int[mapSize, mapSize];
        newGrowth = new int[mapSize, mapSize];
        groundMap = new int[mapSize, mapSize];
        miniMap = new int[mapSize, mapSize];
        possibleGrowth = new int[mapSize, mapSize];

        
        // get ground map tile types
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                TileBase tile = GameHelper.GroundTileMap.GetTile(new Vector3Int(i + offset.x, j + offset.y, 0));
                if (grassTiles.Contains(tile)){
                    groundMap[i, j] = 0;
                    miniMap[i, j] = 0;
                }
                else if (dirtTiles.Contains(tile)){
                    groundMap[i, j] = 1;
                    miniMap[i, j] = 1;
                }
                else if (forestTiles.Contains(tile)){
                    groundMap[i, j] = 2;
                    miniMap[i, j] = 2;
                }
                else if (riverTiles.Contains(tile)){
                    groundMap[i, j] = 3;
                    miniMap[i, j] = 3;
                }
                else if (roadTiles.Contains(tile)){
                    groundMap[i, j] = 4;
                    miniMap[i, j] = 4;
                }
                else{
                    Debug.Log("No tile in ground map");
                }
                
            }
        }

        // get top map tile types
        int towns = 0;
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                TileBase tile = GameHelper.TopTileMap.GetTile(new Vector3Int(i + offset.x, j + offset.y, 0));
                if (tile == townTile) {
                    growth[i, j] = 2;
                    miniMap[i, j] = 5;
                    towns += 1;
                }
                else if (mountainTiles.Contains(tile)){
                    growth[i, j] = 3;
                    miniMap[i, j] = 6;
                }        
            }
        }
        GameHelper.gameManager.Towns = towns;

        // starting growth - edges
        for (int i=0; i < mapSize; i++){
            newGrowth[0, i] = 1;
            newGrowth[mapSize - 1, i] = 1;
            newGrowth[i, 0] = 1;
            newGrowth[i, mapSize - 1] = 1;
        }

        ApplyNewGrowth();
      
    }

    void Update()
    {
        if (GameHelper.gameRunning) {
            UpdateGrowth();
            UpdateSpawning();
        }
    }

    private void UpdateGrowth() {
        growthTimer += Time.deltaTime;
        if (growthTimer >= growthInterval) {
            GenerateGrowth();
            ApplyNewGrowth();
            growthTimer = 0;
        }
    }
    private void UpdateSpawning() {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval) {
            SpawnEnemies();
            spawnTimer = 0;
        }
    }

    private void ApplyNewGrowth() {
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                if (newGrowth[i, j] == 1) {
                    TileBase tile = GameHelper.TopTileMap.GetTile(new Vector3Int(i + offset.x, j + offset.y, 0));
                    if (tile == townTile){
                        GameHelper.TopTileMap.SetTile(new Vector3Int(i + offset.x, j + offset.y, 0), ruinedTownTile);
                        GameHelper.gameManager.LoseTown();
                        miniMap[i, j] = 7;
                    }
                    else if (mountainTiles.Contains(tile)){
                        // TODO change to overgrown mountain?
                        GameHelper.TopTileMap.SetTile(new Vector3Int(i + offset.x, j + offset.y, 0), thornTile);
                        miniMap[i, j] = 8;
                    }
                    else {
                        GameHelper.TopTileMap.SetTile(new Vector3Int(i + offset.x, j + offset.y, 0), thornTile);
                        miniMap[i, j] = 8;
                    }
                    newGrowth[i, j] = 0;
                    growth[i, j] = 1;
                }
            }
        }

        GameHelper.MinimapTileMap.GetComponent<Minimap>().SetMap(miniMap);
    }

    private void GenerateGrowth() {
        // reset possibleGrowth map
        possibleGrowth = new int[mapSize, mapSize];

        // find all possibilities
        int total = 0;
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                if (growth[i, j] != 1) {  // no growth yet
                    if ((i-1 >= 0 && growth[i-1, j] == 1) ||
                        (j-1 >= 0 && growth[i, j-1] == 1) ||
                        (i+1 < mapSize && growth[i+1, j] == 1) ||
                        (j+1 < mapSize && growth[i, j+1] == 1)) { // adjacent growth
                        if (growth[i, j] == 3) {
                            possibleGrowth[i, j] = mountainProb;
                            total += mountainProb;
                        }
                        else if (groundMap[i, j] == 0) {
                            possibleGrowth[i, j] = grassProb;
                            total += grassProb;
                        }
                        else if (groundMap[i, j] == 1) {
                            possibleGrowth[i, j] = dirtProb;
                            total += dirtProb;
                        }
                        else if (groundMap[i, j] == 2) {
                            possibleGrowth[i, j] = forestProb;
                            total += forestProb;
                        }
                        else if (groundMap[i, j] == 3) {
                            possibleGrowth[i, j] = riverProb;
                            total += riverProb;
                        }
                        else if (groundMap[i, j] == 4) {
                            possibleGrowth[i, j] = roadProb;
                            total += roadProb;
                        }
                    }
                }
            }
        }

        // select growth position
        int selection = Random.Range(1, total + 1);
        // Debug.LogFormat("selection: {0}, total: {1}", selection, total);
        // Debug.LogFormat("Selected position {0} of {1}", selection, total);
        int cumsum = 0;
        bool found = false;
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                cumsum += possibleGrowth[i, j];
                if (cumsum >= selection) {
                    newGrowth[i, j] = 1;
                    // Debug.LogFormat("Selected position is {0}, {1} - {2} growth state {3}", i, j, groundMap[i, j], growth[i, j]);
                    found = true;
                    break;
                }
            }
            if (found) {
                break;
            }
        }

    }

    public void RemoveThorns(Vector3Int coords) {
        if (GameHelper.TopTileMap.GetTile(coords) == thornTile){    
            GameHelper.TopTileMap.SetTile(coords, null);
            growth[coords.x - offset.x, coords.y - offset.y] = 0;
        }
        else {
            Debug.Log("Trying to remove non-thorns tile");
        }
    }

    
    public Vector3 RandomAccessiblePosition() {
        // reset possibleGrowth map
        int[,] possiblePos = new int[mapSize, mapSize];

        // find all possibilities
        int total = 0;
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                if (growth[i, j] == 0 & groundMap[i, j] != 3) { // clear and not a river
                    possiblePos[i, j] = 1;
                    total += 1; 
                }
            }
        }

        // select growth position
        int selection = Random.Range(1, total + 1);
        // Debug.LogFormat("selection: {0}, total: {1}", selection, total);
        // Debug.LogFormat("Selected position {0} of {1}", selection, total);
        int cumsum = 0;
        bool found = false;
        Vector3Int coords = Vector3Int.zero;
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                cumsum += possiblePos[i, j];
                if (cumsum >= selection) {
                    coords = new Vector3Int(i + offset.x, j + offset.y, 0);
                    found = true;
                    break;
                }
            }
            if (found) {
                break;
            }
        }
        Vector3 position = GameHelper.GroundTileMap.CellToLocal(coords) + new Vector3(0.5f, 0.5f, 0);
        return position;

    }

    public bool IsThorny(Vector3 position){
        Vector3Int cellCoords = GameHelper.TopTileMap.LocalToCell(position);
        TileBase tile = GameHelper.TopTileMap.GetTile(cellCoords);
        // Debug.LogFormat("{0}: {1}", fw_reach, tilemap.GetTile(cellCoords));
        if (tile == GameHelper.ThornTile) {
            return true;
        }
        else {
            return false;
        }
    }
    public TileBase GroundType(Vector3 position){
        Vector3Int cellCoords = GameHelper.GroundTileMap.LocalToCell(position);
        TileBase tile = GameHelper.GroundTileMap.GetTile(cellCoords);
        return tile;
    }
    public TileBase TopType(Vector3 position){
        Vector3Int cellCoords = GameHelper.TopTileMap.LocalToCell(position);
        TileBase tile = GameHelper.TopTileMap.GetTile(cellCoords);
        return tile;
    }

    public float SpeedMultiplier(Vector3 position) {
        if (IsThorny(position)){
            return thornSpeedMult;
        }
        else if (mountainTiles.Contains(TopType(position))) {
            return mountainSpeedMult;
        }
        else {
            TileBase groundTile = GroundType(position);
            if (grassTiles.Contains(groundTile)) {
                return grassSpeedMult;
            }
            else if (forestTiles.Contains(groundTile)) {
                return forestSpeedMult;
            }
            else if (roadTiles.Contains(groundTile)) {
                return roadSpeedMult;
            }
            else {
                Debug.LogFormat("Bad tile type for speed multiplier: {0}", groundTile);
                return 1.0f;
            }
        }
        
    }

    public void SpawnEnemies() {        
        for (int i=0; i < mapSize; i++){
            for (int j=0; j < mapSize; j++){
                if (growth[i, j] > 0) { // any overgrown tile
                    if (Random.Range(0.0f, 1.0f) < perTileSpawnChance) {
                        Instantiate(GameHelper.Zombie, GameHelper.TopTileMap.CellToLocal(new Vector3Int(i + offset.x, j + offset.y, 0)), Quaternion.identity);
                    }
                }
            }
        }
    }
}
