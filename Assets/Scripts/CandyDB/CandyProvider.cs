
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CandyProvider : MonoBehaviour
{
    [SerializeField] private List<CandyGenerationDTO> candyDataBase;

    public GameObject GetResultCandy(int matchCount, CandyType type, MatchDirection matchType)
    {
        GameObject result = null;

        foreach(CandyGenerationDTO candy in candyDataBase)
        {
            CandyType dtoType = candy.Type == CandyType.Contain_All ? type : candy.Type;
            MatchDirection dtoDirection = candy.MatchType == MatchDirection.Two_Sided ? matchType : candy.MatchType;

            if (matchCount == candy.MatchCount && type == dtoType && matchType == dtoDirection)
            {
                return candy.Candy;
            }
        }

        return result;
    }
}
