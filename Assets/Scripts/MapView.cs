using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ShipType
{
    None = 0,
    Scout,
    Cruiser,
    Carrier
}

public enum Marker
{
    Target = 6,
    Hit = 7,
    Miss = 8
}

public class MapView : MonoBehaviour
{
    public GameSceneController controller;
    public Tilemap cursorLayer;
    public Tilemap fleetLayer;
    public Tilemap debugLayer;
    public Tilemap markerLayer;
	public Tile[] cursorTiles;
    public Tile cursorTile;
    public int size = 8;

	private string mode = "disabled";

    private Grid grid;
    private Vector3Int minCoordinate;
    private Vector3Int maxCoordinate;

    void Start()
    {
        grid = GetComponent<Grid>();
		SetPlacementMode();
    }

    void Update()
    {
		if (mode == "disabled") return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);

        cursorLayer.ClearAllTiles();
        cursorLayer.SetTile(coordinate, cursorTile);
    }

    void OnMouseDown()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);

        if (mode == "place")
        {
            controller.PlaceShip(coordinate);
        }
        else if (mode == "attack")
        {
            // outside code thinks coordinates are 0, 0 based
            controller.TakeTurn(coordinate - new Vector3Int(0, size, 0));
        }
    }

    public void SetDisabled()
	{
		mode = "disabled";
		cursorLayer.ClearAllTiles();
	}

	public void SetPlacementMode()
	{
		mode = "place";
		minCoordinate = new Vector3Int(0, 0, 0);
		maxCoordinate = new Vector3Int(size - 1, size - 1, 0);
	}

    public void SetAttackMode()
    {
        mode = "attack";
        cursorTile = cursorTiles[(int)Marker.Target];
        minCoordinate = new Vector3Int(0, size, 0);
        maxCoordinate = new Vector3Int(size - 1, size + size - 1, 0);
    }

    public void SetMarker(int index, Marker marker, bool radar)
    {
        Vector3Int coordinate = new Vector3Int(index % size, Mathf.FloorToInt(index / size), 0);
        SetMarker(coordinate, marker, radar);
    }

    public void SetMarker(Vector3Int coordinate, Marker marker, bool radar)
    {
        if (radar)
        {
            coordinate += new Vector3Int(0, size, 0); // offset position
        }

        markerLayer.SetTile(coordinate, cursorTiles[(int)marker]);
    }

    public void SetShipCursor(ShipType shipType, bool horizontal)
    {
        int index = ((int)shipType-1) * 2 + (horizontal ? 0 : 1);
        cursorTile = cursorTiles[index];
    }

    public void SetDebugTile(Vector3Int coordinate, Tile tile)
    {
        debugLayer.SetTile(coordinate, tile);
    }

    public void SetShip(ShipType shipType, Vector3Int coordinate, bool horizontal)
    {
        int index = ((int)shipType-1) * 2 + (horizontal ? 0 : 1);
        Tile tile = cursorTiles[index];
        fleetLayer.SetTile(coordinate, tile);
    }
}
