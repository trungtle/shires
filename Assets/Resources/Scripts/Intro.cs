using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Intro : MonoBehaviour
{
    bool loading;

    void Awake ()
    {
        DOTween.Init (true, true, LogBehaviour.Default).SetCapacity (200, 10);
    }

    // Use this for initialization
    void Start ()
    {
	
    }
	
    // Update is called once per frame
    void Update ()
    {
        if (Input.anyKeyDown && !loading) {
            StartCoroutine (LoadGame ());
        }
    }

    IEnumerator LoadGame ()
    {
        loading = true;
        Camera.main.DOColor (Color.black, 2f);
        yield return new WaitForSeconds (2);
        Application.LoadLevel (1);
    }
}
