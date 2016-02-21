using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class BitZone
{
    public int redLord, redRook, redRookExtra, redWhisperer;
    public int blueLord, blueRook, blueRookExtra, blueWhisperer;

    public BitZone ()
    {

    }
}

public class Zone :MonoBehaviour
{
    public GameObject[] tileObjs;
    public int rows;
    public int columns;

    // Store unit locations in tiles
    Tile[,] tiles;
    Board board;
    Game game;
    Unit hoverUnit;
    Player player;
    Flag flag;

    public Tile[,] Tiles {
        get {
            return tiles;
        }
    }

    public Player Player {
        get {
            return player;
        }
        set {
            player = value;
        }
    }

    public Flag Flag {
        get {
            return flag;
        }
        set {
            flag = value;
        }
    }

    public int TilesNum {
        get {
            return columns * rows;
        }
    }

    void Awake ()
    {
        tiles = new Tile[rows, columns];
        board = GetComponentInParent<Board> ();
        game = GetComponentInParent<Game> ();

        for (int i = 0; i < tileObjs.Length; ++i) {

            // Add script component
            Tile tile = tileObjs [i].AddComponent <Tile>() as Tile;

            // Setup tile
            int row = i / columns;
            int column = i % columns;
            tile.Setup (this, column, row);            
            tiles [column, row] = tile;
        }
    }

    void Start ()
    {
        DOTween.Init (true, true, LogBehaviour.Verbose).SetCapacity (200, 10);

    }

    void Update ()
    {

    }

    Vector3 TopLeft ()
    {
        return tileObjs [0].transform.position;
    }

    float TileSize ()
    {
        return tileObjs [0].GetComponent<Renderer>().bounds.size.x;
    }

    public Unit HoverUnit {
        get {
            return hoverUnit;
        }
        set {
            hoverUnit = value;
        }
    }

    public void Shake ()
    {
        foreach (Tile tile in tiles) {
            tile.Shake ();
        }
    }

    public void HighlightTile (int column, int row)
    {
        Tile tile = tiles [column, row];
        tile.HighlightTile ();

    }

    public void ClearHighlights ()
    {
        foreach (Tile tile in tiles) {
            tile.DeHighlightTile ();
        }
    }

    public void ChangeColor (bool isRed)
    {
        foreach (Tile tile in Tiles) {
            tile.ChangeColor (isRed);
        }
        Shake ();
    }

    public void MoveUnit (Unit unit, int column, int row)
    {
        // Special landmark handling
        Portal maybePortal = tiles [column, row].Portal;
        Flag maybeFlag = tiles [column, row].Flag;
        bool success;

        if (maybePortal) {

            Unit end = maybePortal.end;
            success = AttachUnitToTile (unit, column, row); // Skip default animation
            if (!success) {
                return;
            }

            success = end.Zone.AttachUnitToTile (unit, end.Column, end.Row); // Skip default animation

            if (!success) {
                return;
            }

            // Portal pnimation
            Sequence portalAnimation = DOTween.Sequence ();
            portalAnimation.Append (unit.transform.DOMove (tiles [column, row].transform.position, 0.5f));
            portalAnimation.Append (unit.transform.DORotate (new Vector3 (0, 0, 180.0f), 0.2f));
            portalAnimation.Append (unit.transform.DORotate (new Vector3 (0, 0, 360.0f), 0.2f));
            portalAnimation.Append (unit.transform.DOMove (end.Zone.Tiles [end.Column, end.Row].transform.position, 0.5f));
            portalAnimation.Play ();

            // Sound
            unit.PlayableUnit.OnTunneling ();
            
        } else if (maybeFlag && unit.CanCaptureZone ()) {

            success = AttachUnitToTile (unit, column, row); // Skip default animation

            if (!success) {
                return;
            }

            // Capture flag animation
            Sequence flagAnimation = DOTween.Sequence ();
            flagAnimation.Append (unit.transform.DOMove (tiles [column, row].transform.position, 0.5f));
            flagAnimation.Append (unit.transform.DOPunchScale (new Vector3 (.1f, .1f, 0), 2f));
            flagAnimation.Play ();

            // Sound
            unit.PlayableUnit.OnCapturing ();

            // Signal flag to activate benefits
            maybeFlag.OnCaptured (unit.Player);    


        } else {
            success = MoveStaticUnit (unit, column, row, true);

            if (!success) {
                return;
            }

            unit.PlayableUnit.OnCompleteMove ();

        }
        
        ClearHighlights ();
        unit.OnCompleteMove ();
        board.OnUnitCompleteMove ();
    }

    /// <summary>
    /// Move Unit that has no interaction
    /// </summary>
    /// <param name="unit">Unit.</param>
    /// <param name="column">Column.</param>
    /// <param name="row">Row.</param>
    public bool MoveStaticUnit (Unit unit, int column, int row, bool animation = false, bool isLandmark = false)
    {
        bool success = AttachUnitToTile (unit, column, row, isLandmark);

        if (!success) {
            return false;
        }

        unit.MoveToLocation (unit.Tile.transform.position, animation);

        return true;
    }

    public bool SpawnUnit (Unit unit, int column, int row)
    {
        bool success = MoveStaticUnit (unit, column, row);

        if (!success) {
            return false;
        }

        unit.OnSpawnComplete ();
        return true;
    }

    bool AttachUnitToTile (Unit unit, int column, int row, bool isLandmark = false)
    {
        Tile tile = tiles [column, row];

        // Prevent moving to a tile that is occupied, unless the unit is a rook
        if (tile.Occupied () && unit.CanDestroyUnit () == false) {
            return false;
        }
        
        if (unit.Zone) {
            // Delete old location in the previous zone
            unit.Zone.Tiles [unit.Column, unit.Row].DetachUnit (); 
        }
        
        unit.Column = column;
        unit.Row = row;
        unit.Zone = this;

        // Add unit to tile
        tile.AttachUnit (unit, isLandmark);
        return true;
    }

    public string Description ()
    {
        return tag;
    }

    public int IndexFromColumnRow (int column, int row)
    {
        return columns * row + column;
    }        

    public int RowFromIndex (int index)
    {
        return index / columns;
    }

    public int ColumnFromIndex (int index)
    {
        return index % columns;
    }

    public static bool IsIndexSet (int zoneIndexes, int index)
    {
        return (zoneIndexes & 0x1 << index) != 0;
    }

    public static int SetIndex (int zoneIndexes, int index)
    {
        zoneIndexes |= 0x1 << index;
        return zoneIndexes;
    }

    // Bitboard representation
    public BitZone BitZone ()
    {
        BitZone bz = new BitZone ();

        Zone.SetIndex (bz.redLord, game.RedPlayer.Lord.Tile.Index);
        Zone.SetIndex (bz.redWhisperer, game.RedPlayer.Whisperer.Tile.Index);
        Zone.SetIndex (bz.redRook, game.RedPlayer.Rook.Tile.Index);
        Zone.SetIndex (bz.redRookExtra, game.RedPlayer.ExtraRook.Tile.Index);
        
        Zone.SetIndex (bz.blueLord, game.BluePlayer.Lord.Tile.Index);
        Zone.SetIndex (bz.blueWhisperer, game.BluePlayer.Whisperer.Tile.Index);
        Zone.SetIndex (bz.blueRook, game.BluePlayer.Rook.Tile.Index);
        Zone.SetIndex (bz.blueRookExtra, game.BluePlayer.ExtraRook.Tile.Index);

        return bz;
    }
}
