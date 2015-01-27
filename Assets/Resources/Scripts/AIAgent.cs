using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIAgent
{

    Board board;

    public AIAgent ()
    {

    }

    void GenerateMinMaxTree (int depth)
    {
        // Generate the next *depth* levels
    }


}

public class MinMaxTree
{
    BitBoard root;

    public MinMaxTree (BitBoard root)
    {
        this.root = root;
    }

    void GeneratePossibleMoves (BitBoard bb)
    {
        if (bb.isRedTurn) {

        }
    }

    void GenerateRedMoves (BitBoard bb)
    {
    }

    void GeneratePossibleLordMoves (BitBoard bb)
    {
        foreach (BitZone bz in bb.zones) {
            BitBoard possiblePlay = new BitBoard ();


        }

    }
}
