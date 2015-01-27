using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Whisperer : MonoBehaviour
{
    Game game;

    void Start ()
    {
        game = GetComponentInParent<Game> ();
    }

    void OnMouseOver ()
    {
        game.SetHelpText (Whisperer.Description ());
    }

    void OnMouseExit ()
    {
        game.HideHelpText ();
    }

    public static int WhispererMoves (Unit whisperer)
    {
        int moves = 0x0;
        Zone zone = whisperer.Zone;
        
        // Whisperer must move exactly 3 squares but cannot land on walls. It can moves through obstacle.
        // Can only capture playable units

        int speed = 3;
        
        for (int step = 0; step <= speed; ++step) {
            
            // SW
            int row = whisperer.Row + step;
            int column = whisperer.Column - (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    moves = Zone.SetIndex (moves, tile.Index);
                }
            }
            
            // SE
            row = whisperer.Row + step;
            column = whisperer.Column + (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    moves = Zone.SetIndex (moves, tile.Index);
                }
            }
            
            // NE
            row = whisperer.Row - step;
            column = whisperer.Column + (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    moves = Zone.SetIndex (moves, tile.Index);
                }
            }
            
            // NW
            row = whisperer.Row - step;
            column = whisperer.Column - (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    moves = Zone.SetIndex (moves, tile.Index);
                }
            }
        }  
        return moves;
    }

    public static void ShowWhispererMoves (Unit whisperer)
    {
        Zone zone = whisperer.Zone;
        zone.HighlightTile (whisperer.Column, whisperer.Row);
        int moves = WhispererMoves (whisperer);       
        int tilesNum = zone.TilesNum;
        for (int bit = 0; bit < tilesNum; bit++) {
            if (Zone.IsIndexSet (moves, bit)) {
                zone.HighlightTile (zone.ColumnFromIndex (bit), zone.RowFromIndex (bit));
            }
        }
    }

    public static string Description ()
    {
        return "Knight moves exactly 3 tiles in any directions. Can attack.\n\nRespawn: " + PlayableUnit.REGEN_TURNS + " turns";

    }

    // Factory methods
    public static Unit CreateWhisperer (Player player, Zone zone, int column, int row)
    {
        string path = "";
        if (player.IsRed) {
            path = "Prefabs/RedWhisperer";
        } else {
            path = "Prefabs/BlueWhisperer";
        }
        GameObject obj = Instantiate (Resources.Load (path)) as GameObject;
        obj.renderer.sortingLayerName = Unit.UNIT_RENDER_LAYER;
        obj.tag = player.IsRed ? Unit.RED_WHISPERER : Unit.BLUE_WHISPERER;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        player.OnCapturedUnit (unit);
        zone.SpawnUnit (unit, column, row);
        return unit;
    }

}
