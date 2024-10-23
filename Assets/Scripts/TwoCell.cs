using UnityEngine;

public class TwoCell : MonoBehaviour
{
    [SerializeField] public State CellState;
    [SerializeField] public int PlayerOneNeighbours;
    [SerializeField] public int PlayerTwoNeighbours;
    
    public void SetState(State cellState)
    {
        CellState = cellState;

        if (CellState == State.PLAYER_1)
        {
            GetComponent<SpriteRenderer>().color = Color.magenta;
        }
        else if (CellState == State.PLAYER_2)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    public enum State
    {
        PLAYER_1, PLAYER_2, NONE
    }
}
