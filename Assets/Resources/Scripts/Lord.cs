﻿using UnityEngine;
using System.Collections;

public class Lord : MonoBehaviour
{
    Game game;

    // Use this for initialization
    void Start ()
    {
        game = GetComponentInParent<Game> ();
    }
	
    // Update is called once per frame
    void Update ()
    {
	
    }

    void OnMouseOver ()
    {
        game.SetHelpText (Lord.Description ());

    }

    void OnMouseExit ()
    {
        game.HideHelpText ();
    }

    public static int LordMoves (Unit lord)
    {
        int moves = 0x0;

        // All valid tiles radiate from the unit's location,
        // so we just need to find the valid rows and columns
        
        int speed = 2 + lord.ExtraSpeed;
        
        // left
        for (int offset = 1; offset <= speed; offset++) {
            int finalColumn = lord.Column - offset;
            
            if (finalColumn < 0) {
                break;
            }
            
            Tile tile = lord.Zone.Tiles [finalColumn, lord.Row];
            if (lord.CanDestroyUnit () && tile.Unit && tile.Unit.Player != lord.Player) {
                
                // Blocked, if it's a destructable unit, highlight then break
                moves = Zone.SetIndex (moves, tile.Index);
                break;
            } else if (tile.Occupied ()) {
                break;
            }
            
            moves = Zone.SetIndex (moves, tile.Index);
        }
        
        // right
        for (int offset = 1; offset <= speed; offset++) {
            int finalColumn = lord.Column + offset;
            
            if (finalColumn >= lord.Zone.columns) {
                break;
            }
            
            Tile tile = lord.Zone.Tiles [finalColumn, lord.Row];
            if (lord.CanDestroyUnit () && tile.Unit && tile.Unit.Player != lord.Player) {
                
                // Blocked, if it's a destructable unit, highlight then break
                moves = Zone.SetIndex (moves, tile.Index);
                break;
            } else if (tile.Occupied ()) {
                break;
            }
            moves = Zone.SetIndex (moves, tile.Index);
            
        }
        
        // up
        for (int offset = 1; offset <= speed; offset++) {
            int finalRow = lord.Row - offset;
            
            if (finalRow < 0) {
                break;
            }
            
            Tile tile = lord.Zone.Tiles [lord.Column, finalRow];
            if (lord.CanDestroyUnit () && tile.Unit && tile.Unit.Player != lord.Player) {
                
                // Blocked, if it's a destructable unit, highlight then break
                moves = Zone.SetIndex (moves, tile.Index);
                break;
            } else if (tile.Occupied ()) {
                break;
            }
            
            moves = Zone.SetIndex (moves, tile.Index);
            
        }
        
        // down
        for (int offset = 1; offset <= speed; offset++) {
            int finalRow = lord.Row + offset;
            
            if (finalRow >= lord.Zone.rows) {
                break;
            }
            
            Tile tile = lord.Zone.Tiles [lord.Column, finalRow];
            if (lord.CanDestroyUnit () && tile.Unit && tile.Unit.Player != lord.Player) {
                
                // Blocked, if it's a destructable unit, highlight then break
                moves = Zone.SetIndex (moves, tile.Index);
                break;
            } else if (tile.Occupied ()) {
                break;
            }
            
            moves = Zone.SetIndex (moves, tile.Index);
            
        }

        return moves;
    }

    public static void ShowLordMoves (Unit lord)
    {
        // Lord moves up to 2 adjacent orthagonal squares
        lord.Zone.HighlightTile (lord.Column, lord.Row);

        Zone zone = lord.Zone;
        int moves = LordMoves (lord);
        int tilesNum = zone.TilesNum;
        for (int bit = 0; bit < tilesNum; bit++) {
            if (Zone.IsIndexSet (moves, bit)) {
                zone.HighlightTile (zone.ColumnFromIndex (bit), zone.RowFromIndex (bit));
            }
        }

    }

    public static string Description ()
    {
        return "Lord moves in straight line up to 2 tiles. Can capture zones. Cannot attack.\n\nRespawn: " + PlayableUnit.REGEN_TURNS + " turns";
    }

    // Factory methods
    public static Unit CreateLord (Player player, Zone zone, int column, int row)
    {
        string path = "";
        if (player.IsRed) {
            path = "Prefabs/RedLord";
        } else {
            path = "Prefabs/BlueLord";
        }
        GameObject obj = Instantiate (Resources.Load (path)) as GameObject;
        obj.GetComponent<Renderer>().sortingLayerName = Unit.UNIT_RENDER_LAYER;
        obj.tag = player.IsRed ? Unit.RED_LORD : Unit.BLUE_LORD;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        player.OnCapturedUnit (unit);
        zone.SpawnUnit (unit, column, row);
        return unit;
    }

}
