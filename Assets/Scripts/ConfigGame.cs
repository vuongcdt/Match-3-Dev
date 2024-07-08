using UnityEngine;
using UnityEngine.UI;

public class ConfigGame : Singleton<ConfigGame>
{
    [SerializeField] private Cell cell;
    [SerializeField] private Transform backgroundBlock;
    [SerializeField] private Transform gridBlock;
    [SerializeField] private Button buttonReset;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private float avatarSize;
    [SerializeField] private float backgroundSize;
    [SerializeField] private float fillTime;
    [SerializeField] private bool isProcessing;
    [SerializeField] private int obstaclesTotal;
    [SerializeField] private int maxListImage;
    [SerializeField] private float sensitivity;
    [SerializeField] private float minSensitivity;
    [SerializeField] private bool isRevertFill;

    private Cell[,] _grid;

    public Button ButtonReset => buttonReset;

    public float Sensitivity => sensitivity;

    public float MinSensitivity => minSensitivity;

    public Cell Cell => cell;

    public Transform BackgroundBlock => backgroundBlock;

    public Transform GridBlock => gridBlock;

    public int Width => width;

    public int Height => height;

    public float CellSize => cellSize;

    public float AvatarSize => avatarSize;

    public float BackgroundSize => backgroundSize;

    public float FillTime => fillTime;

    public int ObstaclesTotal => obstaclesTotal;

    public int MaxListImage => maxListImage;

    public bool IsProcessing
    {
        get => isProcessing;
        set => isProcessing = value;
    }

    public bool IsRevertFill
    {
        get => isRevertFill;
        set => isRevertFill = value;
    }
}