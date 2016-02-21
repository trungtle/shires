using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayableUnit : MonoBehaviour
{
    public const int REGEN_TURNS = 3;
    
    int deadTurn;
    int currentTurn;
    bool regenerating;
    Game game;
    Board board;
    Unit unit;

    bool removingFromGame;

    AudioClip moveClip, tunnelClip, captureClip;

    public Player player {
        get {
            return unit.Player;
        }
    }

    public Zone zone {
        get {
            return unit.Zone;
        }
    }

    // Use this for initialization
    void Start ()
    {
        game = GetComponentInParent<Game> ();
        board = GetComponentInParent<Board> ();
        unit = GetComponent<Unit> ();
        currentTurn = 0;
        moveClip = (AudioClip)Resources.Load ("Sounds/Jump") as AudioClip;
        tunnelClip = (AudioClip)Resources.Load ("Sounds/Portal") as AudioClip;
        captureClip = (AudioClip)Resources.Load ("Sounds/Powerup") as AudioClip;
    }
    
    // Update is called once per frame
    void FixedUpdate ()
    {       
        if (removingFromGame) {
            return;
        }
        
        if (currentTurn != game.TurnCount) {
            currentTurn = game.TurnCount;
            OnNewTurn ();
        }
        
    }

    void OnMouseOver ()
    {
        // Highlight if there's currently no selected unit
        if (board.CanHover (unit)) {
            ShowMoves ();
        }
    }
    
    void OnMouseDown ()
    {
        board.SelectUnit (unit);
    }

    void OnMouseExit ()
    {
        if (board && !board.SelectedUnit && zone) {
            
            // Clear hover effect
            zone.HoverUnit = null;
            zone.ClearHighlights ();
        }
    }

    void ShowMoves ()
    {
        switch (tag) {
        case Unit.RED_LORD:
        case Unit.BLUE_LORD:
            zone.HoverUnit = unit;
            if (player.HasValley) {
                // Also has whisperer's move
                Whisperer.ShowWhispererMoves (unit);
            } else {
                Lord.ShowLordMoves (unit);
            }
            break;
        case Unit.RED_ROOK:
        case Unit.BLUE_ROOK:
            zone.HoverUnit = unit;
            Rook.ShowRookMoves (unit);
            break;
        case Unit.RED_WHISPERER:
        case Unit.BLUE_WHISPERER:
            zone.HoverUnit = unit;
            Whisperer.ShowWhispererMoves (unit);
            break;
        default:
            break;
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

            if (!unit.SpawnTile) {
                Debug.Log (tag);
                return;
            }

            if (!unit.SpawnTile.Occupied ()) {

                // Show cooldown text
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
        // Show cooldown text
        unit.SpawnTile.SetCaptionText (REGEN_TURNS.ToString (), false);

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
        // Stop showing cooldown text
        unit.SpawnTile.SetCaptionText ("");

        unit.IsDead = false;

        // Reattach unit to tile since it was replaced by whichever unit that attacked it earlier
        unit.SpawnTile.Zone.MoveStaticUnit (unit, unit.SpawnTile.Column, unit.SpawnTile.Row);
        
        // Animation
        transform.DOShakeRotation (3.0f);
    }

    public void OnCompleteMove ()
    {
        GetComponent<AudioSource>().clip = moveClip;
        GetComponent<AudioSource>().Play ();
    }

    public void OnTunneling ()
    {
        GetComponent<AudioSource>().clip = tunnelClip;
        GetComponent<AudioSource>().Play ();
    }

    public void OnCapturing ()
    {
        GetComponent<AudioSource>().clip = captureClip;
        GetComponent<AudioSource>().Play ();
    }

    public void OnRemovedFromGame (bool delayed = false)
    {
        unit.SpawnTile.SetCaptionText ("");

        if (delayed) {
            removingFromGame = true;
        }

        if (player != null) {
            player.ReleaseUnit (unit);
        }
        
    }

    public void OnSelected ()
    {
        transform.DOScale (new Vector3 (1.3f, 1.3f, 1), .5f);
        unit.BringUnitFG ();
        ShowMoves ();
    }

    public void OnDeselected ()
    {
        transform.DOScale (new Vector3 (1.0f, 1.0f, 1), 0.5f);
        zone.ClearHighlights ();
    }
    
}
