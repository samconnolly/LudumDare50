using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileActivation : MonoBehaviour
{
    public float crumbleTime = 1.0f;
    private List<float> crumbleTimers;
    private List<Vector3Int> crumblingTiles;

    // Start is called before the first frame update
    void Start()
    {
        crumbleTimers = new List<float> { };
        crumblingTiles = new List<Vector3Int> { };
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i < crumbleTimers.Count; i++)
        {
            crumbleTimers[i] += Time.deltaTime;
            if (crumbleTimers[i] >= crumbleTime)
            {
                GetComponent<Tilemap>().SetTile(crumblingTiles[i], null);

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            (TileBase tile, Vector3Int gridPos) = GetTileFromCollision(collision);
            Activation(tile, gridPos);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            (TileBase tile, Vector3Int gridPos) = GetTileFromCollision(collision);
            Activation(tile, gridPos);
        }
    }

    private (TileBase, Vector3Int) GetTileFromCollision(Collision2D collision)
    {
        ContactPoint2D contactPoint;
        Vector2 contactPos;
        Grid tilemapGrid;
        Vector3Int gridPos = Vector3Int.zero;
        TileBase tile = null;
        int i = 0;
        while (tile == null && i < collision.contactCount)
        {
            contactPoint = collision.GetContact(i);
            contactPos = contactPoint.point;
            tilemapGrid = GetComponent<Tilemap>().layoutGrid;
            gridPos = tilemapGrid.WorldToCell(contactPos);
            tile = GetComponent<Tilemap>().GetTile(gridPos);
            i++;
        }
        return (tile, gridPos);
    }

    private void Activation(TileBase tile, Vector3Int gridPos)
    {
        string name;
        if (tile != null)
        {
            name = tile.name;
        }
        else
        {
            name = "none";
        }

        if (name == "LavaBase")
        {
            // GameHelper.gameManager.KillPlayer();
        }
        else if (name == "crumble")
        {
            crumblingTiles.Add(gridPos);
            crumbleTimers.Add(0);
        }

    }

}
