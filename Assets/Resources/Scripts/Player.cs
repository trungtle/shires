using System.Collections;
using System.Collections.Generic;

public class Player
{
    bool isRed;
    List<Zone> zones;
    List<Unit> units;
    List<Card> cards;
    Unit extraRook;
    int food = 0;

    public bool IsRed {
        get {
            return isRed;
        }
    }

    public Unit Lord {
        get {
            foreach (Unit u in units) {
                if (u.tag == Unit.RED_LORD || u.tag == Unit.BLUE_LORD) {
                    return u;
                }
            }
            return null;
        }
    }

    // We can have more than 1 rook
    public Unit ExtraRook {
        get {
            return extraRook;
        }
        set {
            extraRook = value;
        }
    }

    public Unit Whisperer {
        get {
            foreach (Unit u in units) {
                if (u.tag == Unit.RED_WHISPERER || u.tag == Unit.BLUE_WHISPERER) {
                    return u;
                }
            }
            return null;
        }

    }

    public int FoodPerTurn {
        get {
            return zones.Count;
        }
    }

    public Player (bool isRed)
    {
        this.isRed = isRed;
        this.zones = new List<Zone> ();
        this.units = new List<Unit> ();
    }

    public bool HasValley {
        get {
            foreach (Zone z in zones) {
                if (z.Flag.tag == Flag.VALLEY_FLAG) {
                    return true;
                }
            }
            return false;
        }
    }

    public bool HasHill {
        get {
            foreach (Zone z in zones) {
                if (z.Flag.tag == Flag.HILL_FLAG) {
                    return true;
                }
            }
            return false;
        }
    }

    public bool HasRiver {
        get {
            foreach (Zone z in zones) {
                if (z.Flag.tag == Flag.RIVER_FLAG) {
                    return true;
                }
            }
            return false;
        }
    }

    public void Play (Unit unit, Zone z, int column, int row)
    {
        z.MoveUnit (unit, column, row);
    }

    public void OnCapturedUnit (Unit unit)
    {
        if (unit.Player != null) {
            // Release from previous ownership
            unit.Player.ReleaseUnit (unit);
        }

        unit.Player = this;
        units.Add (unit);
    }

    public void ReleaseUnit (Unit unit)
    {
        units.Remove (unit);
        if (unit == ExtraRook) {
            ExtraRook = null;
        }
    }
    
    public void CaptureZone (Zone zone)
    {
        if (zone.Player != null) {
            // Release from previous ownership
            zone.Player.ReleaseZone (zone);
        }

        zone.Player = this;
        zones.Add (zone);

        // visualize
        zone.ChangeColor (isRed);
    }

    public string Status ()
    {
        string status = "Shires controlled:\n\n";
        foreach (Zone z in zones) {
            if (z.Flag) {
                status += z.Flag.ShortDescription () + "\n";
            }
        }

        return status;
    }

    public void ReleaseZone (Zone zone)
    {
        zones.Remove (zone);

        if (zone.Flag) {
            zone.Flag.OnReleased (this);
        }
    }

    public void OnTurnStart ()
    {
        food += FoodPerTurn;
    }

}
