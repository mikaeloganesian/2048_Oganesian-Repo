using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameField : MonoBehaviour
{
    private int score = 0;
    public TextMeshProUGUI scoreText;
    
    [Header("Field Properties")] 
    public float CellSize;
    public float Spacing;
    public int FieldSize;
    public int InitCellsCount;

    [Space(10)] 
    [SerializeField] private Cell cellPref;

    [SerializeField] private RectTransform rt;
    
    private Cell[,] field;
    
    private Vector2 mouseStartPos;
    private bool isMouseSwiping = false;

	#if UNITY_EDITOR
	public Cell GetTestCell(int x, int y) => field[x,y];
	public void SetTestCellValue(int x, int y, int value) => field[x,y].SetValue(x,y,value);
	#endif

    void Start()
    {
        CreateField();
        GenerateField();
    }

    private void CreateField()
    {
        field = new Cell[FieldSize, FieldSize];

        float fieldWidth = FieldSize * (CellSize + Spacing) + Spacing;
        rt.sizeDelta = new Vector2(fieldWidth, fieldWidth);

        float startX = -(fieldWidth / 2) + (CellSize / 2) + Spacing;
        float startY = (fieldWidth / 2) - (CellSize / 2) - Spacing;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                var cell = Instantiate(cellPref, transform, false);
                var position = new Vector2(startX + (x * (CellSize + Spacing)), startY - (y * (CellSize + Spacing)));
                cell.transform.localPosition = position;

                field[x, y] = cell;
                cell.SetValue(x, y, 0);
            }
        }
    }

    public void GenerateField()
    {
        if (field == null)
        {
            CreateField();
        }

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                field[x, y].SetValue(x, y, 0);
            }
        }

        for (int i = 0; i < InitCellsCount; i++)
        {
            GenerateRandomCell();
        }
    }

    private void GenerateRandomCell()
    {
        var emptyCells = new List<Cell>();

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (field[x, y].isEmpty)
                {
                    emptyCells.Add(field[x, y]);
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            throw new System.Exception("No empty cells left!");
        }

        int value = UnityEngine.Random.Range(0, 10) == 0 ? 2 : 1;
        var cell = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];
        cell.SetValue(cell.X, cell.Y, value);
    }
    
    public void MoveCells(Vector2Int direction)
    {
        bool cellsMoved = false;
        bool[,] merged = new bool[FieldSize, FieldSize];

        int startX = direction.x > 0 ? FieldSize - 1 : 0;
        int startY = direction.y > 0 ? FieldSize - 1 : 0;
        int endX = direction.x > 0 ? -1 : FieldSize;
        int endY = direction.y > 0 ? -1 : FieldSize;
        int stepX = direction.x > 0 ? -1 : 1;
        int stepY = direction.y > 0 ? -1 : 1;

        for (int x = startX; x != endX; x += stepX)
        {
            for (int y = startY; y != endY; y += stepY)
            {
                if (!field[x, y].isEmpty)
                {
                    var cell = field[x, y];
                    int newX = x;
                    int newY = y;

                    while (true)
                    {
                        int nextX = newX + direction.x;
                        int nextY = newY + direction.y;

                        if (nextX < 0 || nextX >= FieldSize || nextY < 0 || nextY >= FieldSize)
                            break;

                        var nextCell = field[nextX, nextY];

                        if (nextCell.isEmpty)
                        {
                            newX = nextX;
                            newY = nextY;
                        }
                        else if (nextCell.Value == cell.Value && !merged[nextX, nextY])
                        {
                            cell.SetValue(nextX, nextY, cell.Value + 1);
                            nextCell.SetValue(nextX, nextY, 0);
                            merged[nextX, nextY] = true;
                            cellsMoved = true;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (newX != x || newY != y)
                    {
                        field[newX, newY].SetValue(newX, newY, cell.Value);
                        field[x, y].SetValue(x, y, 0);
                        cellsMoved = true;
                    }
                }
            }
        }



        if (cellsMoved)
        {
            GenerateRandomCell();
            UpdateScore();
        }
    }

    private void UpdateScore()
    {
        score = 0;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (!field[x, y].isEmpty)
                {
                    score += field[x, y].Points;
                }
            }
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public string CheckWin()
    {
        if (field == null) return string.Empty;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (field[x, y].Value >= 11)
                {
                    return "You Win! Your score: " + score.ToString();
                }
            }
        }
        return string.Empty;
    }

    public string CheckLose()
    {
        if (field == null) return string.Empty;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (field[x, y].isEmpty)
                {
                    return string.Empty;
                }

                if (x < FieldSize - 1 && field[x, y].Value == field[x + 1, y].Value)
                {
                    return string.Empty;
                }

                if (y < FieldSize - 1 && field[x, y].Value == field[x, y + 1].Value)
                {
                    return string.Empty;
                }
            }
        }
        return "You Lose! Your score: " + score.ToString();
    }
    
    void Update()
    {
        if (field == null) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCells(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCells(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCells(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCells(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveCells(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveCells(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveCells(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveCells(Vector2Int.right);
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPos = Input.mousePosition;
            isMouseSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isMouseSwiping)
        {
            Vector2 mouseEndPos = Input.mousePosition;
            Vector2 swipeDirection = mouseEndPos - mouseStartPos;

            if (swipeDirection.magnitude > 50)
            {
                if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                {
                    if (swipeDirection.x > 0)
                    {
                        MoveCells(Vector2Int.right);
                    }
                    else
                    {
                        MoveCells(Vector2Int.left);
                    }
                }
                else
                {
                    if (swipeDirection.y > 0)
                    {
                        MoveCells(Vector2Int.down);
                    }
                    else
                    {
                        MoveCells(Vector2Int.up);
                    }
                }
            }

            isMouseSwiping = false;
        }

        string winMessage = CheckWin();
        if (!string.IsNullOrEmpty(winMessage))
        {
            Debug.Log(winMessage);
            if (scoreText != null)
            {
                scoreText.text = "You win!";
            }
        }

        string loseMessage = CheckLose();
        if (!string.IsNullOrEmpty(loseMessage))
        {
            Debug.Log(loseMessage); 
            if (scoreText != null)
            {
                scoreText.text = "You lose with score: " + score.ToString();
            }
        }
    }
}