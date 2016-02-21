using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public const string PORTAL_RENDER_LAYER = "Portal";
    public const string PORTAL = "Portal";

    public Unit end;

    // Use this for initialization
    void Start ()
    {
	
    }
	
    // Update is called once per frame
    void Update ()
    {
	
    }

    public static void CreatePortal (Zone startZ, int startColumn, int startRow, Zone endZ, int endColumn, int endRow)
    {
        // Start
        GameObject obj1 = Instantiate (Resources.Load ("Prefabs/Portal")) as GameObject;
        obj1.GetComponent<Renderer>().sortingLayerName = PORTAL_RENDER_LAYER;
        obj1.tag = PORTAL;
        obj1.transform.SetParent (startZ.transform);
        Unit unit1 = obj1.GetComponent<Unit> ();
        Portal portal1 = obj1.GetComponent<Portal> ();
        startZ.MoveStaticUnit (unit1, startColumn, startRow, false, true);
        unit1.Tile.SetCaptionSize (24);
        unit1.Tile.SetCaptionText ("=>" + endZ.Description ()); // Show the end zone
        
        // End
        GameObject obj2 = Instantiate (Resources.Load ("Prefabs/Portal")) as GameObject;
        obj2.GetComponent<Renderer>().sortingLayerName = PORTAL_RENDER_LAYER;
        obj2.tag = PORTAL;
        obj2.transform.SetParent (endZ.transform);
        Unit unit2 = obj2.GetComponent<Unit> ();
        Portal portal2 = obj2.GetComponent<Portal> ();
        endZ.MoveStaticUnit (unit2, endColumn, endRow, false, true);
        unit2.Tile.SetCaptionSize (24);
        unit2.Tile.SetCaptionText ("=>" + startZ.Description ()); // SHow the start zone
        
        // Link portals
        portal1.end = unit2;
        portal2.end = unit1;
    }

}
