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

    public static void ShowWhispererMoves (Unit whisperer)
    {
        Zone zone = whisperer.Zone;
        
        // Whisperer must move exactly 3 squares but cannot land on walls. It can moves through obstacle.
        // Can only capture playable units
        zone.HighlightTile (whisperer.Column, whisperer.Row);
        
        int speed = 3;
        
        for (int step = 0; step <= speed; ++step) {
            
            // SW
            int row = whisperer.Row + step;
            int column = whisperer.Column - (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    zone.HighlightTile (column, row);
                }
            }
            
            // SE
            row = whisperer.Row + step;
            column = whisperer.Column + (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    zone.HighlightTile (column, row);
                }
            }
            
            // NE
            row = whisperer.Row - step;
            column = whisperer.Column + (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    zone.HighlightTile (column, row);
                }
            }
            
            // NW
            row = whisperer.Row - step;
            column = whisperer.Column - (speed - step);
            if (column < zone.columns && row < zone.rows && column >= 0 && row >= 0) {
                Tile tile = zone.Tiles [column, row];
                if (tile.Unit == null || (tile.Unit.Player != null && tile.Unit.Player != whisperer.Player && whisperer.CanDestroyUnit ())) {
                    zone.HighlightTile (column, row);
                }
            }
        }        
    }

    public static string Description ()
    {
        return "Whisperer moves exactly 3 tiles in any directions. Can attack.\n\nRespawn: " + PlayableUnit.REGEN_TURNS + " turns";

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
