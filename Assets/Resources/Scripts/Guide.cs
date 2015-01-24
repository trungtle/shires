using UnityEngine;
using System.Collections;

public class Guide : MonoBehaviour
{

    public GameObject[] guides;

    Game game;

    // Use this for initialization
    void Start ()
    {
        game = GetComponent<Game> ();
    }
	
    // Update is called once per frame
    void Update ()
    {
        if (game.TurnCount > 0) {

            // Remove guides after play first turn
            foreach (GameObject g in guides) {
                g.SetActive (false);
            }
        }
    }
}
