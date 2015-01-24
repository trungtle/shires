using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Flag : MonoBehaviour
{
    public const string FLAG_RENDER_LAYER = "Flag";

    public const string VALLEY_FLAG = "ValleyFlag";
    public const string RIVER_FLAG = "RiverFlag";
    public const string HILL_FLAG = "HillFlag";
    public const string RED_FLAG = "RedFlag";
    public const string BLUE_FLAG = "BlueFlag";

    public Zone zone;

    Game game;
    Board board;

    // Use this for initialization
    void Start ()
    {
        game = GetComponentInParent<Game> ();
        board = GetComponentInParent<Board> ();
    }
	
    // Update is called once per frame
    void Update ()
    {
	
    }

    public void OnCaptured (Player byPlayer)
    {
        byPlayer.CaptureZone (zone);

        // Change the whole zone to the new owner
        switch (tag) {
        case VALLEY_FLAG:

            // Double lord speed
            byPlayer.Lord.ExtraSpeed = 2;
            break;

        case HILL_FLAG:

            // +1 rook
            board.GainOneRook (byPlayer);
            break;

        case RIVER_FLAG:

            // Lord can destroy
            break;
        case RED_FLAG:

            // Blue won!
            if (!byPlayer.IsRed) {
                game.OnGameOver (byPlayer);
            }
            break;
        case BLUE_FLAG:

            // Red won!
            if (byPlayer.IsRed) {
                game.OnGameOver (byPlayer);
            }
            break;
        default:
            break;
        }
    }

    public void OnReleased (Player fromPlayer)
    {
        switch (tag) {
        case VALLEY_FLAG:            
            fromPlayer.Lord.ExtraSpeed = 0;
            break;
            
        case HILL_FLAG:            
            // -1 rook
            board.LoseOneRook (fromPlayer);
            break;
            
        case RIVER_FLAG:
            // Lord stop destroy
            break;

        case RED_FLAG:
        case BLUE_FLAG:
        default:
            // Should never reach here
            break;
        }
    }

    public string ShortDescription ()
    {
        switch (tag) {
        case VALLEY_FLAG:
            return "Valley grants Lord's moves";
        case RIVER_FLAG:
            return "River grants Lord's attack";
        case HILL_FLAG:
            return "Hill grants +1 rook";
        case RED_FLAG:
            return "Red temple";
        case BLUE_FLAG:
            return "Blue temple";
        default:
            return "";
        }
    }

    public string Description ()
    {
        switch (tag) {
        case VALLEY_FLAG:
            return "Valley grants Lord moves from Whisperer";
        case RIVER_FLAG:
            return "River allows Lord to attack";
        case HILL_FLAG:
            return "Hill grants +1 rook";
        case RED_FLAG:
            return "Red Temple. Control both temples to win.";
        case BLUE_FLAG:
            return "Blue Temple. Control both temples to win.";
        default:
            return "";
        }

    }

    // Factory methods
    public static void CreateValleyFlag (Zone zone, int column, int row)
    {
        GameObject obj = Instantiate (Resources.Load ("Prefabs/ValleyFlag")) as GameObject;
        obj.renderer.sortingLayerName = FLAG_RENDER_LAYER;
        obj.tag = VALLEY_FLAG;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        zone.Flag = obj.GetComponent<Flag> ();
        zone.MoveStaticUnit (unit, column, row, false, true);
    }
    
    public static void CreateHillFlag (Zone zone, int column, int row)
    {
        GameObject obj = Instantiate (Resources.Load ("Prefabs/HillFlag")) as GameObject;
        obj.renderer.sortingLayerName = FLAG_RENDER_LAYER;
        obj.tag = HILL_FLAG;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        zone.Flag = obj.GetComponent<Flag> ();
        zone.MoveStaticUnit (unit, column, row, false, true);
    }
    
    public static void CreateRiverFlag (Zone zone, int column, int row)
    {
        GameObject obj = Instantiate (Resources.Load ("Prefabs/RiverFlag")) as GameObject;
        obj.renderer.sortingLayerName = FLAG_RENDER_LAYER;
        obj.tag = RIVER_FLAG;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        zone.Flag = obj.GetComponent<Flag> ();
        zone.MoveStaticUnit (unit, column, row, false, true);
    }
    
    public static void CreateRedFlag (Zone zone, int column, int row)
    {
        GameObject obj = Instantiate (Resources.Load ("Prefabs/RedFlag")) as GameObject;
        obj.renderer.sortingLayerName = FLAG_RENDER_LAYER;
        obj.tag = RED_FLAG;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        zone.Flag = obj.GetComponent<Flag> ();
        zone.MoveStaticUnit (unit, column, row, false, true);
    }
    
    public static void CreateBlueFlag (Zone zone, int column, int row)
    {
        GameObject obj = Instantiate (Resources.Load ("Prefabs/BlueFlag")) as GameObject;
        obj.renderer.sortingLayerName = FLAG_RENDER_LAYER;
        obj.tag = BLUE_FLAG;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        zone.Flag = obj.GetComponent<Flag> ();
        zone.MoveStaticUnit (unit, column, row, false, true);
    }
}
