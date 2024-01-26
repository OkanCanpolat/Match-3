using UnityEngine;

[CreateAssetMenu (fileName = "CandyGenerationDTO")]
public class CandyGenerationDTO : ScriptableObject
{
    public int MatchCount;
    public MatchDirection MatchType;
    public CandyType Type;
    public GameObject Candy;
}
