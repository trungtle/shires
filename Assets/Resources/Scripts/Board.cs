using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BitBoard
{
    public bool isRedTurn;

    // Bitboards
    public List<BitZone> zones;
    public List<BitBoard> possibleMoves;

    public BitBoard ()
    {
        zones = new List<BitZone> ();
        possibleMoves = new List<BitBoard> ();
    }
}

public class Board : MonoBehaviour
{
    public GameObject redZoneObj, blueZoneObj, valleyZoneObj, hillZoneObj, riverZoneObj;

    Zone redZone, blueZone, valleyZone, hillZone, riverZone;
    List<Unit> playableUnits;
    Unit selectedUnit;
    Game game;

    void Awake ()
    {
        DOTween.Init (true, true, LogBehaviour.Default).SetCapacity (200, 10);
    }

    void Start ()
    {
        GetComponents ();
        SetupZones ();
        SetupUnits ();
        SetupFlags ();
        SetupPortals ();
        SetupWalls ();

        hillZone.Shake ();
        riverZone.Shake ();
        valleyZone.Shake ();

        game.OnGameReady ();
    }

    void FixedUpdate ()
    {
    }

    void GetComponents ()
    {
        game = GetComponent<Game> ();
        
        // Zones        
        redZone = redZoneObj.GetComponent<Zone> ();
        blueZone = blueZoneObj.GetComponent<Zone> ();
        riverZone = riverZoneObj.GetComponent<Zone> ();
        hillZone = hillZoneObj.GetComponent<Zone> ();
        valleyZone = valleyZoneObj.GetComponent<Zone> ();
    }

    
    void SetupZones ()
    {
        game.RedPlayer.CaptureZone (redZone);
        game.BluePlayer.CaptureZone (blueZone);
        
    }

    void SetupUnits ()
    {
        // Units
        playableUnits = new List<Unit> ();
        playableUnits.Add (Lord.CreateLord (game.RedPlayer, redZone, 2, 0));
        playableUnits.Add (Rook.CreateRook (game.RedPlayer, redZone, 4, 0));
        playableUnits.Add (Whisperer.CreateWhisperer (game.RedPlayer, redZone, 0, 0));
        playableUnits.Add (Lord.CreateLord (game.BluePlayer, blueZone, 2, 4));
        playableUnits.Add (Rook.CreateRook (game.BluePlayer, blueZone, 0, 4));
        playableUnits.Add (Whisperer.CreateWhisperer (game.BluePlayer, blueZone, 4, 4));
    }
    
    void SetupFlags ()
    {
        Flag.CreateValleyFlag (valleyZone, 2, 2);
        Flag.CreateRiverFlag (riverZone, 2, 2);
        Flag.CreateHillFlag (hillZone, 2, 2);
        Flag.CreateRedFlag (redZone, 2, 2);
        Flag.CreateBlueFlag (blueZone, 2, 2);
    }

    void SetupPortals ()
    {
        // 3 from red zone to each neutral zone
        Portal.CreatePortal (redZone, 2, 4, riverZone, 3, 0);
        Portal.CreatePortal (redZone, 0, 4, hillZone, 1, 0);
        Portal.CreatePortal (redZone, 4, 4, valleyZone, 1, 0);

        // 3 from blue zone to each neutral zone
        Portal.CreatePortal (blueZone, 2, 0, riverZone, 1, 4);
        Portal.CreatePortal (blueZone, 0, 0, hillZone, 3, 4);
        Portal.CreatePortal (blueZone, 4, 0, valleyZone, 3, 4);

        // From each neutral zone to each other
        Portal.CreatePortal (hillZone, 4, 1, riverZone, 0, 1);
        Portal.CreatePortal (riverZone, 4, 3, valleyZone, 0, 3);
        Portal.CreatePortal (valleyZone, 4, 1, hillZone, 0, 3);
    }

    void SetupWalls ()
    {
        // Add wall in a V-shape layout
        Wall.CreateWall (redZone, 0, 2);
        Wall.CreateWall (redZone, 2, 3);
        Wall.CreateWall (redZone, 4, 2);

        Wall.CreateWall (blueZone, 0, 2);
        Wall.CreateWall (blueZone, 2, 1);
        Wall.CreateWall (blueZone, 4, 2);

        Wall.CreateWall (riverZone, 1, 1);
        Wall.CreateWall (riverZone, 3, 1);
        Wall.CreateWall (riverZone, 1, 3);
        Wall.CreateWall (riverZone, 3, 3);

        Wall.CreateWall (hillZone, 1, 1);
        Wall.CreateWall (hillZone, 3, 1);
        Wall.CreateWall (hillZone, 1, 3);
        Wall.CreateWall (hillZone, 3, 3);

        Wall.CreateWall (valleyZone, 1, 1);
        Wall.CreateWall (valleyZone, 3, 1);
        Wall.CreateWall (valleyZone, 1, 3);
        Wall.CreateWall (valleyZone, 3, 3);
    }

    void BringPlayableUnitsFG ()
    {
        foreach (Unit unit in playableUnits) {
            unit.BringUnitFG ();
        }
    }

    void BringPlayableUnitsBG ()
    {
        foreach (Unit unit in playableUnits) {
            unit.BringUnitBG ();
        }
    }

    void DeselectUnit ()
    {
        selectedUnit.OnDeselected ();
        selectedUnit = null;
        BringPlayableUnitsFG (); // change z-order to select units
    }

    public Unit SelectedUnit {
        get {
            return selectedUnit;
        }
    }

    // Return true on successfully selecting a unit
    public bool SelectUnit (Unit unit)
    {
        if (game.CanPlayTurn (unit)) {

            if (selectedUnit == unit) {

                // Toggle selection
                DeselectUnit ();

                return false;

            } else {

                if (selectedUnit) {
                    DeselectUnit ();
                }

                // Select a new unit
                selectedUnit = unit;
                BringPlayableUnitsBG (); // change z-order to select tiles
                selectedUnit.OnSelected ();

                return true;
            }
        }

        return false;
    }

    public void OnUnitCompleteMove ()
    {
        if (selectedUnit) {
            selectedUnit.OnDeselected ();
            selectedUnit = null;
        }
        BringPlayableUnitsFG ();
        game.OnPlayerCompleteTurn ();
    }

    public bool CanHover (Unit unit)
    {
        return (game.CanPlayTurn (unit) && !selectedUnit && unit.Zone != null);
    }

    public void GainOneRook (Player player)
    {
        Unit rook = null;

        // todo: Do not spawn on top of another unit, maybe wait before spawining happen?
        int c, r;
        if (player.IsRed) {
            for (c = 0; c < hillZone.columns; c++) {
                for (r = 0; r < hillZone.rows; r++) {
                    if (!hillZone.Tiles [c, r].Occupied ()) {
                        rook = Rook.CreateRook (player, hillZone, c, r);
                        playableUnits.Add (rook);
                        player.ExtraRook = rook;
                        return;
                    } 
                }
            }

        } else {
            for (r = hillZone.rows - 1; r >= 0; r--) {
                for (c = 0; c <= hillZone.columns; c++) {
                    if (!hillZone.Tiles [c, r].Occupied ()) {
                        rook = Rook.CreateRook (player, hillZone, c, r);
                        playableUnits.Add (rook);
                        player.ExtraRook = rook;
                        return;
                    } 
                }
            }
        }
    }
    
    public void LoseOneRook (Player player)
    {
        Unit rook = player.ExtraRook;
        playableUnits.Remove (rook);
        rook.RemoveFromGame (true);
    }

    public BitBoard BitBoard {                    
        get {
            BitBoard bitBoard = new BitBoard ();
            bitBoard.zones.Add (redZone.BitZone ());
            bitBoard.zones.Add (blueZone.BitZone ());
            bitBoard.zones.Add (hillZone.BitZone ());
            bitBoard.zones.Add (valleyZone.BitZone ());
            bitBoard.zones.Add (riverZone.BitZone ());
            bitBoard.isRedTurn = game.IsRedTurn;
            return bitBoard;
        }
    }
}