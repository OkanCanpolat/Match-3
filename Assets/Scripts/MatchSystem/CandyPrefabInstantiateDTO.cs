using UnityEngine;

public struct CandyPrefabInstantiateDTO 
{
    public GameObject Candy;
    public int Column;
    public int Row;
    public CandyPrefabInstantiateDTO(GameObject Candy, int Column, int Row)
    {
        this.Candy = Candy;
        this.Column = Column;
        this.Row = Row;
    }

}
