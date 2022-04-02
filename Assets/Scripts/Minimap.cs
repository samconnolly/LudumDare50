using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Minimap : MonoBehaviour
{
    public TileBase[] mapTiles;
    public Vector2Int offset;

    private int[,] map;
    private Tilemap tileMap;
    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMap(int[,] newMap){
        map = newMap;
        for (int i=0; i<GameHelper.mapSize; i++) {
            for (int j=0; j<GameHelper.mapSize; j++) {
                Vector3Int tileCoords = new Vector3Int(i + offset.x, j + offset.y, 0);
                TileBase currentTile = tileMap.GetTile(tileCoords);
                TileBase newTile = mapTiles[map[i, j]];
                if (newTile != currentTile){
                    tileMap.SetTile(tileCoords, newTile);
                }
            }
        }
    }
}
