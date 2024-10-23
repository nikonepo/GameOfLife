using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] public bool isAlive = true;
    [SerializeField] public int neighbours;

    public void SetAlive(bool alive)
    {
        isAlive = alive;

        if (isAlive)
            GetComponent<SpriteRenderer>().color = Color.white;
        else
            GetComponent<SpriteRenderer>().color = Color.black;
    }
}