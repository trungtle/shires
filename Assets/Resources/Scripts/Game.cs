using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// This class maintains game rules
/// </summary>
public class Game : MonoBehaviour
{
    public Text debugLog;
    public Text announcement, helpText;
    public Text blueStatus, redStatus;
    public Button restartButton;
    public Image fader;
    public Image redToken, blueToken, redCardPanel, blueCardPanel;

    bool redTurn;
    Player redPlayer;
    Player bluePlayer;

    int turnCount;
    bool gameStarted;

    public Player RedPlayer {
        get {
            return redPlayer;
        }
    }

    public Player BluePlayer {
        get {
            return bluePlayer;
        }
    }
	
    public int TurnCount {
        get {
            return turnCount;
        }
    }

    public bool HasStarted {
        get {
            return gameStarted;
        }
    }

    // Use this for initialization
    void Awake ()
    {
        redTurn = false;
        redPlayer = new Player (true);
        bluePlayer = new Player (false);
    }

    void Start ()
    {

    }

    void GameStart ()
    {
        gameStarted = true;
        FadeScreen (true);
        announcement.text = "";
        
        OnPlayerCompleteTurn ();
        turnCount = 0;        
    }
		
    void FadeScreen (bool fadeIn)
    {
        if (fadeIn) {
            fader.DOFade (0f, 1.0f);
        } else {
            fader.DOFade (.8f, 1f);
        }
    }
    
    IEnumerator DelayShowRestartButton (float seconds)
    {
        yield return new WaitForSeconds (seconds);
        restartButton.gameObject.SetActive (true);
    }
    // Update is called once per frame
    void FixedUpdate ()
    {
        if (!gameStarted && Input.anyKeyDown) {
            GameStart ();
        }

        if (Input.GetKeyDown (KeyCode.Escape)) {           
            Application.Quit ();
        }

        if (Input.GetKeyDown (KeyCode.R)) {
            Restart ();
        }
        
        blueStatus.text = bluePlayer.Status ();
        redStatus.text = redPlayer.Status ();
    }

    public bool IsRedTurn {
        get {
            return redTurn;
        }
        set {
            redTurn = value;
        }
    }

    public bool CanPlayTurn (Unit unit)
    {
        if (!HasStarted) {
            return false;
        }
        return (unit.Player != null && unit.Player.IsRed && IsRedTurn) || 
            (unit.Player != null && !unit.Player.IsRed && !IsRedTurn);
    }
    
    public void OnPlayerCompleteTurn ()
    {
        IsRedTurn = !IsRedTurn;
        turnCount++;

        // Start a new turn
        if (IsRedTurn) {
            redToken.DOFade (.9f, 1f);
            redCardPanel.DOFade (.8f, 1f);
            blueToken.DOFade (.1f, 1f);
            blueCardPanel.DOFade (.3f, 1f);
                        
            redPlayer.OnTurnStart ();
        } else {
            blueToken.DOFade (.9f, 1f);
            blueCardPanel.DOFade (.8f, 1f);
            redToken.DOFade (.1f, 1f);
            redCardPanel.DOFade (.3f, 1f);

            bluePlayer.OnTurnStart ();
        }
    }

    public void OnGameReady ()
    {
        GameStart ();
    }

    public void OnGameOver (Player winner)
    {
        FadeScreen (false);
        announcement.text = (winner.IsRed ? "Red won!" : "Blue won!");

        // Allow for restarting game
        StartCoroutine (DelayShowRestartButton (1));
    }

    public void Restart ()
    {
        Application.LoadLevel (1);
    }

    public void SetHelpText (string text)
    {
        helpText.text = text;
        helpText.DOFade (1f, .15f);
    }

    public void HideHelpText ()
    {
        helpText.DOFade (0f, .3f);
    }

    public void ShowLordHelp ()
    {
        SetHelpText (Lord.Description ());
    }
    
    public void ShowWhispererHelp ()
    {
        SetHelpText (Whisperer.Description ());
    }

    public void ShowRookHelp ()
    {
        SetHelpText (Rook.Description ());
    }
}
