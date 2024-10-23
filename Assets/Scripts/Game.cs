using TMPro;
using UnityEngine;
using Random = System.Random;

public class Game : MonoBehaviour
{
    private static int WIDTH = 479;
    private static int HEIGHT = 269;
    
    private PauseMenu pauseMenu;
    
    public TextMeshProUGUI SpeedText;
    public int Speed = 25;

    private GameSession gameSession;
    
    void Start()
    {
        pauseMenu = FindFirstObjectByType<PauseMenu>();
        UpdateSpeedText();
        
        gameSession = new GameSession();
        gameSession.InitializeBoard();
    }

    void Update()
    {
        if (pauseMenu.IsPaused)
        {
            return;
        }
        
        if (gameSession.isFirstStep && !gameSession.isStopped)
        {
            gameSession.isFirstStep = false;
            
            if (IsEnd())
            {
                gameSession.RandomInitializeBoard();
            }
        }
        
        if (Input.GetMouseButtonDown(0) && gameSession.isStopped)
        {
            gameSession.ClickCell();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameSession.isStopped = !gameSession.isStopped;
        }
        
        if (gameSession.isStopped)
        {
            return;
        }

        if (gameSession.timer >= (Speed / 100f))
        {
            gameSession.timer = 0.0f;
            gameSession.UpdateBoard();
        }
        else
        {
            gameSession.timer += Time.deltaTime;
        }
    }

    public void MinusSpeed()
    {
        Speed += 5;

        if (Speed > 100)
        {
            Speed = 100;
        }
        
        UpdateSpeedText();
    }

    public void PlusSpeed()
    {
        Speed -= 5;

        if (Speed <= 0)
        {
            Speed = 5;
        }
        
        UpdateSpeedText();
    }

    private bool IsEnd()
    {
        for (int x = 0; x < WIDTH; ++x)
        {
            for (int y = 0; y < HEIGHT; ++y)
            {
                if (gameSession.board[x, y].isAlive)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void ReloadGame()
    {
        Speed = 25;
        gameSession.End();
    }

    public void UpdateSpeedText()
    {
        SpeedText.text = (105 - Speed) + "/100";
    }

    class GameSession
    {
        private Random random = new Random();
        
        public bool isStopped = true;
        public bool isFirstStep = true;
    
        public Cell[,] board = new Cell[WIDTH, HEIGHT];
        
        public float timer = 0f;
        
        private void UpdateNeighbours()
        {
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    int neighbours = 0;

                    if (y + 1 < HEIGHT && board[x, y + 1].isAlive)
                    {
                        neighbours++;
                    }

                    if (y - 1 >= 0 && board[x, y - 1].isAlive)
                    {
                        neighbours++;
                    }

                    if (x + 1 < WIDTH && board[x + 1, y].isAlive)
                    {
                        neighbours++;
                    }

                    if (x - 1 >= 0 && board[x - 1, y].isAlive)
                    {
                        neighbours++;
                    }

                    if (y + 1 < HEIGHT && x + 1 < WIDTH && board[x + 1, y + 1].isAlive)
                    {
                        neighbours++;
                    }

                    if (y + 1 < HEIGHT && x - 1 >= 0 && board[x - 1, y + 1].isAlive)
                    {
                        neighbours++;
                    }

                    if (y - 1 >= 0 && x + 1 < WIDTH && board[x + 1, y - 1].isAlive)
                    {
                        neighbours++;
                    }

                    if (y - 1 >= 0 && x - 1 >= 0 && board[x - 1, y - 1].isAlive)
                    {
                        neighbours++;
                    }
                
                    board[x, y].neighbours = neighbours;
                }
            }
        }
        
        public void InitializeBoard()
        {
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    Cell cell = Instantiate(
                        Resources.Load<Cell>("Prefabs/Cell"), 
                        new Vector2(x, y), 
                        Quaternion.identity
                    );
                
                    board[x, y] = cell;
                    cell.SetAlive(false);
                }
            }
        }

        public void RandomInitializeBoard()
        {
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    var cell = board[x, y];
                
                    if (random.Next(2) == 0)
                    {
                        cell.SetAlive(true);
                    }
                    else
                    {
                        cell.SetAlive(false);
                    }
                }
            }
        }

        public void UpdateBoard()
        {
            UpdateNeighbours();
            UpdateAliveCells();
        }
        
        private void UpdateAliveCells()
        {
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    if (board[x, y].isAlive)
                    {
                        if (board[x, y].neighbours == 2 || board[x, y].neighbours == 3)
                        {
                            continue;
                        }
                    
                        board[x, y].SetAlive(false);
                    }
                    else
                    {
                        if (board[x, y].neighbours == 3)
                        {
                            board[x, y].SetAlive(true);
                        }
                    }
                }
            }
        }

        public void ClickCell()
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            int x = Mathf.RoundToInt(mousePosition.x);
            int y = Mathf.RoundToInt(mousePosition.y);

            if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
            {
                board[x, y].SetAlive(!board[x, y].isAlive);
            }
        }

        public void End()
        {
            isStopped = true;
            isFirstStep = true;
            timer = 0f;
            
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    board[x, y].neighbours = 0;
                    board[x, y].SetAlive(false);
                }
            }
        }
    }
}
