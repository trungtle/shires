using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    const string CAPTION_RENDER_LAYER = "Caption";

    Zone zone;
    int column;
    int row;
    int index; // For bitboard position

    bool validForMove;
    Unit occupyingUnit;
    Portal portal;
    Board board;
    Game game;
    Flag flag;

    // Visual
    Sprite redTileSprite;
    Sprite blueTileSprite;
    SpriteRenderer spriteRenderer;
    TextMesh text; // any caption text goes here

    public int Column {
        get {
            return column;
        }
    }

    public int Row {
        get {
            return row;
        }
    }

    public int Index {
        get {
            return index;
        }
    }


    void Awake ()
    {
        // todo: Cache this so we don't have to load for all tiles
        redTileSprite = Resources.Load ("Sprites.atlas/red@2x", typeof(Sprite)) as Sprite;
        blueTileSprite = Resources.Load ("Sprites.atlas/blue@2x", typeof(Sprite)) as Sprite;
        board = GetComponentInParent<Board> ();
        game = GetComponentInParent<Game> ();
        spriteRenderer = GetComponent<SpriteRenderer> ();
        SetupText ();
    }


    // Use this for initialization
    void Start ()
    {
    }
	
    // Update is called once per frame
    void FixedUpdate ()
    {

    }

    void SetupText ()
    {
        GameObject textObj = new GameObject ();        
        text = textObj.AddComponent<TextMesh> ();
        text.renderer.sortingLayerName = CAPTION_RENDER_LAYER;
        text.renderer.sortingOrder = 0;
        text.font = (Font)Resources.Load ("Fonts/DIN") as Font;
        text.fontStyle = FontStyle.Normal;
        text.renderer.material = text.font.material;
        text.fontSize = 60;
        text.anchor = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.transform.localScale *= .1f;
        text.transform.position = transform.position;
    }
    
    void OnMouseOver ()
    {
        if (flag) {
            game.SetHelpText (flag.Description ());
        }
    }

    void OnMouseExit ()
    {
        game.HideHelpText ();
    }

    void OnMouseDown ()
    {
        if (validForMove) {
            zone.MoveUnit (board.SelectedUnit, column, row);
        } else if (occupyingUnit) {
            board.SelectUnit (occupyingUnit);
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

    public void Setup (Zone zone, int column, int row)
    {
        this.zone = zone;
        this.column = column;
        this.row = row;
        this.index = zone.columns * row + column;
    }

    public void SetCaptionTextColor (Color color)
    {
        this.text.color = color;
    }

    public void SetCaptionSize (int size = 60)
    {
        this.text.fontSize = size;
    }

    public void SetCaptionText (string text, bool animation = false)
    {
        this.text.text = text;
        if (animation) {
            this.text.transform.DOPunchScale (new Vector3 (.03f, .03f, 1f), 3f, 5);
        }
    }

    public void HighlightTile ()
    {
        if (this.portal && this.portal.end.Tile.Occupied ()) {
            // One end of the portal is occupied
            return;
        }
        this.transform.GetChild (0).gameObject.SetActive (true);
        this.validForMove = true;
    }

    public void DeHighlightTile ()
    {
        this.transform.GetChild (0).gameObject.SetActive (false);
        this.validForMove = false;
    }

    public void AttachUnit (Unit unit, bool isLandmark = false)
    {
        if (isLandmark) {
            // N.B We're not checking for null here since it 
            // doesn't matter.
            portal = unit.GetComponent<Portal> (); // Could be null
            flag = unit.GetComponent<Flag> (); // Could be null
            if (flag) {
                flag.zone = zone;
            }
        } else {
            if (occupyingUnit) {
                // Destroy old unit
                occupyingUnit.OnDestroyed (unit);
            }
            occupyingUnit = unit;
        }
    }

    public void DetachUnit ()
    {
        // Remove unit from tile
        occupyingUnit = null;
    }

    public void ChangeColor (bool isRed)
    {
        if (isRed) {
            spriteRenderer.sprite = redTileSprite;
        } else {
            spriteRenderer.sprite = blueTileSprite;
        }
    }

    public void Shake ()
    {
        transform.DOShakeRotation (2f, 60, 8);
    }

    public bool Occupied ()
    {
        if (Portal) {            
            // Check if both ends are not occupied
            Unit end = Portal.end;
            if (end.Zone.Tiles [end.Column, end.Row].occupyingUnit != null) {
                return false;
            }
        }

        return occupyingUnit != null;
    }

    public Unit Unit {
        get {
            return occupyingUnit;
        }
    }

    public Portal Portal {
        get {
            return portal;
        }
    }

    public Flag Flag {
        get {
            return flag;
        }
    }


}
