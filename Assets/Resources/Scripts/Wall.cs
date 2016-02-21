using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Wall : MonoBehaviour
{
    public const string WALL = "Wall";
    public const string WALL_RENDER_LAYER = "Tile";

    public const int REGEN_TURNS = 10;

    bool regenerating;
    int currentTurn;
    int deadTurn; // keep track of how many turns have passed

    Game game;
    Unit unit;

    // Use this for initialization
    void Start ()
    {
        game = GetComponentInParent<Game> ();
        unit = GetComponent<Unit> ();
        currentTurn = 0;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (game.TurnCount != currentTurn) {
            currentTurn = game.TurnCount;
            OnNewTurn ();
        }

    }

    void OnNewTurn ()
    {
        if (regenerating && deadTurn > REGEN_TURNS) {
            
            if (!unit.SpawnTile.Occupied ()) {
                regenerating = false;
                OnRevived ();
            }
        }
        
        if (regenerating) {

            if (!unit.SpawnTile.Occupied ()) {

                // Display cooldown
                int cd = REGEN_TURNS - deadTurn;
                unit.SpawnTile.SetCaptionText (cd.ToString (), true);
                deadTurn++;

            } else {

                // Remove cool down text
                unit.SpawnTile.SetCaptionText ("");
            }
        }
        
        if (unit.IsDead && !regenerating) {
            deadTurn = 0;
            regenerating = true;
            OnDestroyed ();
        }

    
    }

    void OnDestroyed ()
    {
        // Clear status
        unit.Row = 0;
        unit.Column = 0;
        unit.Zone = null;

        // Animation
        transform.DOShakeRotation (10.0f);
        Vector2 randDirection = Random.insideUnitCircle;
        transform.DOMove (new Vector3 (randDirection.x * 100, randDirection.y * 100, transform.position.z), 10f);
    }

    void OnRevived ()
    {
        unit.SpawnTile.SetCaptionText ("");
        unit.IsDead = false;
        // Reattach unit to tile since it was replaced by whichever unit that attacked it earlier
        unit.SpawnTile.Zone.MoveStaticUnit (unit, unit.SpawnTile.Column, unit.SpawnTile.Row);

        // Animation
        transform.DOShakeRotation (3.0f);

    }

    // Factory methods
    public static Unit CreateWall (Zone zone, int column, int row)
    {
        GameObject wallObj = Instantiate (Resources.Load ("Prefabs/Wall")) as GameObject;
        wallObj.GetComponent<Renderer>().sortingLayerName = WALL_RENDER_LAYER;
        wallObj.tag = WALL;
        wallObj.transform.SetParent (zone.transform);
        Unit wall = wallObj.GetComponent<Unit> ();
        zone.SpawnUnit (wall, column, row);
        return wall;
    }
}
