using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMatchFinder : IMatchFinder
{
    public bool FindMatch(Candy source, MatchesDTO resultMatches, bool markMatches = true)
    {
        return false;
    }
}
