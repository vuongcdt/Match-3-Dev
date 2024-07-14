using System.Collections.Generic;
using GameControllers;
using TMPro;
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

    [SerializeField] private bool isProcessing;
    [SerializeField] private int obstaclesTotal;
    [SerializeField] private int maxListImage;
    [SerializeField] private float sensitivity;
    [SerializeField] private float minSensitivity;
    [SerializeField] private bool isRevertFill;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float timeScale = 2;
    [SerializeField] private float fillTime;
    [SerializeField] private float matchTime;
    [SerializeField] private int stepsTotal = 10;

    private TMP_Text _obstaclesTotalText;
    private TMP_Text _stepsTotalText;
    private TMP_Text _scoreText;
    
    private int _level;
    private bool _isDragged;
    private bool _isGamePlaying;
    private Stack<Cell> _pool = new();

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    public TMP_Text ScoreText
    {
        get => _scoreText;
        set => _scoreText = value;
    }

    public TMP_Text StepsTotalText
    {
        get => _stepsTotalText;
        set => _stepsTotalText = value;
    }

    public int StepsTotal
    {
        get => stepsTotal;
        set => stepsTotal = value;
    }

    public float MatchTime
    {
        get => matchTime;
        set => matchTime = value;
    }

    public TMP_Text ObstaclesTotalText
    {
        get => _obstaclesTotalText;
        set => _obstaclesTotalText = value;
    }

    public float TimeScale
    {
        get => timeScale;
        set => timeScale = value;
    }

    public Stack<Cell> Pool
    {
        get => _pool;
        set => _pool = value;
    }

    public Sprite[] Sprites
    {
        get => sprites;
        set => sprites = value;
    }

    public bool IsDragged
    {
        get => _isDragged;
        set => _isDragged = value;
    }

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

    public int ObstaclesTotal
    {
        get => obstaclesTotal;
        set => obstaclesTotal = value;
    }

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