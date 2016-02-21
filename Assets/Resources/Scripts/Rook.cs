using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Rook : MonoBehaviour
{
    Game game;

    void Start ()
    {
        game = GetComponentInParent<Game> ();
    }

    void OnMouseOver ()
    {
        game.SetHelpText (Rook.Description ());
    }

    void OnMouseExit ()
    {
        game.HideHelpText ();
    }

    public static void ShowRookMoves (Unit rook)
    {
        // Rook moves like in chess
        // Can destroy walls and units

        Zone zone = rook.Zone;
        zone.HighlightTile (rook.Column, rook.Row);
        
        int moves = RookMoves (rook);   
        int tilesNum = zone.TilesNum;
        for (int bit = 0; bit < tilesNum; bit++) {
            if ((moves & (0x1 << bit)) != 0) {
                int c = zone.ColumnFromIndex (bit);
                int r = zone.RowFromIndex (bit);
                zone.HighlightTile (c, r);
            }
        }
    }

    public static int RookMoves (Unit rook)
    {
        int moves = 0x0;

        // All valid tiles radiate from the unit's location,
        // so we just need to find the valid rows and columns (not blocked or occupied)
        
        // left
        for (int offset = 1; offset <= rook.Column; offset++) {
            
            int finalColumn = rook.Column - offset;
            Tile tile = rook.Zone.Tiles [finalColumn, rook.Row];
            if (tile.Unit && tile.Unit.Player != rook.Player) {
                
                // Blocked, if it's a destructable unit, highlight then break
                moves = Zone.SetIndex (moves, tile.Index);

                break;
            } else if (tile.Occupied ()) {
                break;
            }
            moves = Zone.SetIndex (moves, tile.Index);

        }
        
        // right
        for (int offset = 1; offset < rook.Zone.columns - rook.Column; offset++) {
            int finalColumn = rook.Column + offset;
            
            Tile tile = rook.Zone.Tiles [finalColumn, rook.Row];
            if (tile.Unit && tile.Unit.Player != rook.Player) {
                
                // Blocked, if it's a destructable unit, highlight then break
                moves = Zone.SetIndex (moves, tile.Index);

                break;
            } else if (tile.Occupied ()) {
                break;
            } 
            moves = Zone.SetIndex (moves, tile.Index);


        }
        
        // up 
        for (int offset = 1; offset <= rook.Row; offset++) {
            int finalRow = rook.Row - offset;
            
            Tile tile = rook.Zone.Tiles [rook.Column, finalRow];
            if (tile.Unit && tile.Unit.Player != rook.Player) {
                
                // Blocked, if it's a destructable unit, highlight then break
                moves = Zone.SetIndex (moves, tile.Index);

                break;
            } else if (tile.Occupied ()) {
                break;
            }
            moves = Zone.SetIndex (moves, tile.Index);

        }        
        
        // down
        for (int offset = 1; offset < rook.Zone.rows - rook.Row; offset++) {
            int finalRow = rook.Row + offset;
            
            Tile tile = rook.Zone.Tiles [rook.Column, finalRow];
            if (tile.Unit && tile.Unit.Player != rook.Player) {
                
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
    

    public static string Description ()
    {
        return "Rook moves in straight lines. Can attack.\n\nRespawn: " + PlayableUnit.REGEN_TURNS + " turns";
    }


    // Factory methods
    public static Unit CreateRook (Player player, Zone zone, int column, int row)
    {
        string path = "";
        if (player.IsRed) {
            path = "Prefabs/RedRook";
        } else {
            path = "Prefabs/BlueRook";
        }
        GameObject obj = Instantiate (Resources.Load (path)) as GameObject;
        obj.GetComponent<Renderer>().sortingLayerName = Unit.UNIT_RENDER_LAYER;
        obj.tag = player.IsRed ? Unit.RED_ROOK : Unit.BLUE_ROOK;
        obj.transform.SetParent (zone.transform);
        Unit unit = obj.GetComponent<Unit> ();
        zone.SpawnUnit (unit, column, row);
        player.OnCapturedUnit (unit);
        return unit;
    }

}
