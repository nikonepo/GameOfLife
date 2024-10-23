using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

public class GameTwoPlayers : MonoBehaviour
{
    private static readonly int WIDTH = 479;
    private static readonly int HEIGHT = 269;

    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI ScoreText;

    public Button ChangePlayerButton;
    public TextMeshProUGUI ChangePlayerButtonText;

    public int Speed = 25;
    
    private PauseMenu2 pauseMenu;
    private EndMenu endMenu;

    private GameSession gameSession;
    
    private void Start()
    {
        pauseMenu = FindFirstObjectByType<PauseMenu2>();
        endMenu = FindFirstObjectByType<EndMenu>();
        UpdateSpeedText();

        gameSession = new GameSession();
        gameSession.InitializeBoard();
    }

    public void UpdateGliderPattern()
    {
        gameSession.pattern = Pattern.GLIDER;
    }

    public void UpdateNonePattern()
    {
        gameSession.pattern = Pattern.NONE;
    }

    public void UpdateTubPattern()
    {
        gameSession.pattern = Pattern.TUB;
    }

    public void UpdateShipPattern()
    {
        gameSession.pattern = Pattern.SHIP;
    }

    public void UpdateBlinkerPattern()
    {
        gameSession.pattern = Pattern.BLINKER;
    }

    private void Update()
    {
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (pauseMenu.IsPaused || gameSession.isEnd)
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

        if (Input.GetMouseButtonDown(0) && gameSession.isFirstStep)
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

        if (gameSession.timer >= Speed / 100f)
        {
            if (IsEnd())
            {
                End();
                return;
            }

            gameSession.timer = 0.0f;
            gameSession.UpdateBoard();

            UpdateScoreText();
        }
        else
        {
            gameSession.timer += Time.deltaTime;
        }
    }

    private void End()
    {
        gameSession.isEnd = true;
        var firstPlayerCells = 0;
        var secondPlayerCells = 0;
        for (var x = 0; x < WIDTH; ++x)
        {
            for (var y = 0; y < HEIGHT; ++y)
            {
                var cell = gameSession.board[x, y];
                if (cell.CellState == TwoCell.State.PLAYER_1)
                {
                    firstPlayerCells++;
                }
                else if (cell.CellState == TwoCell.State.PLAYER_2)
                {
                    secondPlayerCells++;
                }
            }
        }
        
        endMenu.Active();

        if (firstPlayerCells == 0 && secondPlayerCells == 0)
        {
            if (gameSession.PlayerOneScore > gameSession.PlayerTwoScore)
            {
                endMenu.EndText.text = "First player wins!";
            }
            else
            {
                endMenu.EndText.text = "Second player wins!";
            }
        }
        else if (firstPlayerCells == 0)
        {
            endMenu.EndText.text = "Second player wins!";
        }
        else if (secondPlayerCells == 0)
        {
            endMenu.EndText.text = "First player wins!";
        }
        
        if (gameSession.PlayerOneScore > gameSession.PlayerTwoScore)
        {
            endMenu.EndText.text = "First player wins!";
        }
        else
        {
            endMenu.EndText.text = "Second player wins!";
        }
        
        gameSession.End();
    }

    public void MinusSpeed()
    {
        Speed += 5;

        if (Speed > 100) Speed = 100;

        UpdateSpeedText();
    }

    public void PlusSpeed()
    {
        Speed -= 5;

        if (Speed <= 0) Speed = 5;

        UpdateSpeedText();
    }

    public void ChangePlayer()
    {
        gameSession.IsOnePlayer = !gameSession.IsOnePlayer;

        UpdateChangePlayerButton();
    }

    private bool IsEnd()
    {
        var firstPlayerCells = 0;
        var secondPlayerCells = 0;
        for (var x = 0; x < WIDTH; ++x)
        {
            for (var y = 0; y < HEIGHT; ++y)
            {
                var cell = gameSession.board[x, y];
                if (cell.CellState == TwoCell.State.PLAYER_1)
                {
                    firstPlayerCells++;
                }
                else if (cell.CellState == TwoCell.State.PLAYER_2)
                {
                    secondPlayerCells++;
                }
            }
        }

        if (firstPlayerCells == 0 || secondPlayerCells == 0)
        {
            return true;
        }

        return false;
    }

    public void ReloadGame()
    {
        Speed = 25;
        gameSession.End();
    }

    public void UpdateSpeedText()
    {
        SpeedText.text = 105 - Speed + "/100";
    }

    public void UpdateScoreText()
    {
        ScoreText.text = gameSession.PlayerOneScore + " : " + gameSession.PlayerTwoScore;
    }

    public void UpdateChangePlayerButton()
    {
        var colors = ChangePlayerButton.colors;
        if (gameSession.IsOnePlayer)
        {
            colors.normalColor = Color.magenta;
            ChangePlayerButtonText.text = "1";
        }
        else
        {
            colors.normalColor = Color.white;
            ChangePlayerButtonText.text = "2";
        }

        ChangePlayerButton.colors = colors;
    }

    private class GameSession
    {
        public readonly TwoCell[,] board = new TwoCell[WIDTH, HEIGHT];
        public bool isFirstStep = true;

        public bool IsOnePlayer = true;

        public bool isStopped = true;

        public bool isEnd = false;

        public int PlayerOneScore;
        public int PlayerTwoScore;
        private readonly Random random = new();

        public float timer;
        
        public Pattern pattern = Pattern.NONE;
        
        private void UpdateNeighbours()
        {
            for (var x = 0; x < WIDTH; ++x)
            for (var y = 0; y < HEIGHT; ++y)
            {
                var player1Neighbours = 0;
                var player2Neighbours = 0;

                if (y + 1 < HEIGHT)
                {
                    if (board[x, y + 1].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x, y + 1].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                if (y - 1 >= 0)
                {
                    if (board[x, y - 1].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x, y - 1].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                if (x + 1 < WIDTH)
                {
                    if (board[x + 1, y].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x + 1, y].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                if (x - 1 >= 0)
                {
                    if (board[x - 1, y].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x - 1, y].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                if (y + 1 < HEIGHT && x + 1 < WIDTH)
                {
                    if (board[x + 1, y + 1].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x + 1, y + 1].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                if (y + 1 < HEIGHT && x - 1 >= 0)
                {
                    if (board[x - 1, y + 1].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x - 1, y + 1].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                if (y - 1 >= 0 && x + 1 < WIDTH)
                {
                    if (board[x + 1, y - 1].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x + 1, y - 1].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                if (y - 1 >= 0 && x - 1 >= 0)
                {
                    if (board[x - 1, y - 1].CellState == TwoCell.State.PLAYER_1)
                        player1Neighbours++;
                    else if (board[x - 1, y - 1].CellState == TwoCell.State.PLAYER_2)
                        player2Neighbours++;
                }

                board[x, y].PlayerOneNeighbours = player1Neighbours;
                board[x, y].PlayerTwoNeighbours = player2Neighbours;
            }
        }

        public void InitializeBoard()
        {
            for (var x = 0; x < WIDTH; ++x)
            for (var y = 0; y < HEIGHT; ++y)
            {
                var cell = Instantiate(
                    Resources.Load<TwoCell>("Prefabs/TwoCell"),
                    new Vector2(x, y),
                    Quaternion.identity
                );

                board[x, y] = cell;
                cell.SetState(TwoCell.State.NONE);
            }
        }

        public void RandomInitializeBoard()
        {
            for (var x = 0; x < WIDTH; ++x)
            for (var y = 0; y < HEIGHT; ++y)
            {
                var cell = board[x, y];

                if (random.Next(2) == 0)
                {
                    if (random.Next(2) == 0)
                        cell.SetState(TwoCell.State.PLAYER_1);
                    else
                        cell.SetState(TwoCell.State.PLAYER_2);
                }
                else
                {
                    cell.SetState(TwoCell.State.NONE);
                }
            }
        }

        public void UpdateBoard()
        {
            UpdateNeighbours();
            UpdateCells();
        }

        private void UpdateCells()
        {
            for (var x = 0; x < WIDTH; ++x)
            for (var y = 0; y < HEIGHT; ++y)
            {
                var cell = board[x, y];
                var countNeighbours = cell.PlayerOneNeighbours + cell.PlayerTwoNeighbours;
                if (cell.CellState == TwoCell.State.NONE && countNeighbours == 3)
                {
                    if (cell.PlayerTwoNeighbours < cell.PlayerOneNeighbours)
                    {
                        cell.SetState(TwoCell.State.PLAYER_1);
                        PlayerOneScore++;
                    }
                    else if (cell.PlayerTwoNeighbours > cell.PlayerOneNeighbours)
                    {
                        cell.SetState(TwoCell.State.PLAYER_2);
                        PlayerTwoScore++;
                    }
                }
                else if (countNeighbours == 2 || countNeighbours == 3)
                {
                }
                else
                {
                    cell.SetState(TwoCell.State.NONE);
                }
            }
        }

        public void ClickCell()
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var x = Mathf.RoundToInt(mousePosition.x);
            var y = Mathf.RoundToInt(mousePosition.y);

            if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
            {
                if (IsOnePlayer)
                {
                    ClickPattern(pattern, TwoCell.State.PLAYER_1, x, y);
                    // board[x, y].SetState(TwoCell.State.PLAYER_1);
                }
                else
                {
                    ClickPattern(pattern, TwoCell.State.PLAYER_2, x, y);
                    // board[x, y].SetState(TwoCell.State.PLAYER_2);
                }
            }
        }

        private void ClickPattern(Pattern pattern, TwoCell.State state, int x, int y)
        {
            switch (this.pattern)
            {
                case Pattern.NONE:
                    board[x, y].SetState(state);
                    break;
                
                case Pattern.GLIDER:
                    board[x, y + 1].SetState(state);
                    board[x + 1, y].SetState(state);
                    board[x - 1, y - 1].SetState(state);
                    board[x, y - 1].SetState(state);
                    board[x + 1, y - 1].SetState(state);
                    break;
                
                //  .
                // . .
                //  .
                case Pattern.TUB:
                    board[x + 1, y].SetState(state);
                    board[x - 1, y].SetState(state);
                    board[x, y + 1].SetState(state);
                    board[x, y - 1].SetState(state);
                    break;
                
                //    .
                //  .   .
                // .
                // .    .
                // .....
                case Pattern.SHIP:
                    board[x - 2, y - 2].SetState(state);
                    board[x - 1, y - 2].SetState(state);
                    board[x, y - 2].SetState(state);
                    board[x + 1, y - 2].SetState(state);
                    board[x + 2, y - 2].SetState(state);
                    
                    board[x - 2, y - 1].SetState(state);
                    board[x + 3, y - 1].SetState(state);
                    
                    board[x - 2, y].SetState(state);
                    
                    board[x - 1, y + 1].SetState(state);
                    board[x + 3, y + 1].SetState(state);
                    board[x + 1, y + 2].SetState(state);
                    break;
                    
                case Pattern.BLINKER:
                    board[x - 1, y].SetState(state);
                    board[x, y].SetState(state);
                    board[x + 1, y].SetState(state);
                    break;
            }
        }

        public void End()
        {
            isStopped = true;
            isFirstStep = true;
            isEnd = false;
            timer = 0f;

            for (var x = 0; x < WIDTH; ++x)
            {
                for (var y = 0; y < HEIGHT; ++y)
                {
                    board[x, y].PlayerOneNeighbours = 0;
                    board[x, y].PlayerTwoNeighbours = 0;
                    board[x, y].SetState(TwoCell.State.NONE);
                }
            }

            PlayerOneScore = 0;
            PlayerTwoScore = 0;
        }
    }

    public enum Pattern
    {
        GLIDER, TUB, SHIP, BLINKER, NONE
    }
}