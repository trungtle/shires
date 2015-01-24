using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    const int HIGH_Z = 1;
    const int LOW_Z = -1;

    public const string UNIT_RENDER_LAYER = "Unit";

    public const string RED_LORD = "RedLord";
    public const string RED_ROOK = "RedRook";
    public const string RED_WHISPERER = "RedWhisperer";
    public const string BLUE_LORD = "BlueLord";
    public const string BLUE_ROOK = "BlueRook";
    public const string BLUE_WHISPERER = "BlueWhisperer";

    int column;
    int row;
    int extraSpeed = 0;

    Zone zone;
    Board board;
    Player player;
    PlayableUnit playableUnit;
    Tile spawnTile;

    bool isDead;
    bool didMoveSinceSpawned; // Has moved since spawn

    public int ExtraSpeed {
        get {
            return extraSpeed;
        }
        set {
            extraSpeed = value;
        }
    }

    public int Column {
        get {
            return column;
        }
        set {
            column = value;
        }
    }

    public int Row {
        get {
            return row;
        }
        set {
            row = value;
        }
    }

    public Zone Zone {
        get {
            return zone;
        }
        set {
            zone = value;
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

    public Tile Tile {
        get {
            if (zone != null) {
                return zone.Tiles [column, row];
            } else {
                return null;
            }

        }
    }

    public Tile SpawnTile {
        get {
            return spawnTile;
        }
    }

    public bool IsDead {
        get {
            return isDead;
        }
        set {
            isDead = value;
        }
    }

    public bool DidMoveSinceSpawned {
        get {
            return didMoveSinceSpawned;
        }
    }

    public PlayableUnit PlayableUnit {
        get {
            return playableUnit;
        }
    }

    void Start ()
    {
        board = GetComponentInParent<Board> ();
        playableUnit = GetComponent<PlayableUnit> ();
    }
    
    void Update ()
    {
    }
    
    void ChangeZOrder (int newZ)
    {
        transform.position = new Vector3 (transform.position.x, transform.position.y, newZ);
    }

    public bool CanCaptureZone ()
    {
        return (tag == RED_LORD || tag == BLUE_LORD) && player != zone.Player;
    }
        
    public bool CanDestroyUnit ()
    {
        return (this.Player != null && this.Player.HasRiver) || tag == RED_ROOK || tag == BLUE_ROOK || tag == RED_WHISPERER || tag == BLUE_WHISPERER;
    }

    public void OnDestroyed (Unit byUnit)
    {
        // Obstacles are destructable by rooks. Regenerate after 5 turns.
        switch (tag) {
        case Wall.WALL:
        case RED_LORD:
        case BLUE_LORD:   
        case RED_ROOK:
        case BLUE_ROOK:
        case RED_WHISPERER:
        case BLUE_WHISPERER:
            isDead = true;
            break;
        }
    }

    public void OnSelected ()
    {
        playableUnit.OnSelected ();
    }

    public void OnDeselected ()
    {
        playableUnit.OnDeselected ();
    }
    
    public void OnCompleteMove ()
    {
        didMoveSinceSpawned = true;
    }

    public void MoveToLocation (Vector3 location, bool animation = false)
    {
        // Animation
        if (animation) {
            transform.DOMove (location, 0.5f);            
        } else {
            transform.position = location;
        }
        BringUnitFG ();
    }
        
    public void BringUnitFG ()
    {
        ChangeZOrder (LOW_Z);
    }

    public void BringUnitBG ()
    {
        ChangeZOrder (HIGH_Z);
    }

    public void RemoveFromGame (bool animation = false)
    {
        // Animation
        if (animation) {
            transform.DOShakeRotation (5.0f);
            Vector2 randDirection = Random.insideUnitCircle;
            transform.DOMove (new Vector3 (randDirection.x * 200, randDirection.y * 200, transform.position.z), 5f);
        }

        if (playableUnit != null) {
            playableUnit.OnRemovedFromGame (true);
        }
        
        Destroy (gameObject, 5f);
    } 

    public void OnSpawnComplete ()
    {
        spawnTile = Tile;
    }
};
