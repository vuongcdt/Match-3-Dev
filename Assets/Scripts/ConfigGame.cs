using UnityEngine;
using UnityEngine.Serialization;

public class ConfigGame : Singleton<ConfigGame>
{
    [SerializeField] private Cell cell;
    [SerializeField] private Transform backgroundBlock;
    [SerializeField] private Transform gridBlock;
    
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

    public float Sensitivity
    {
        get => sensitivity;
        set => sensitivity = value;
    }

    public float MinSensitivity
    {
        get => minSensitivity;
        set => minSensitivity = value;
    }

    public Cell Cell
    {
        get => cell;
        set => cell = value;
    }

    public Transform BackgroundBlock
    {
        get => backgroundBlock;
        set => backgroundBlock = value;
    }

    public Transform GridBlock
    {
        get => gridBlock;
        set => gridBlock = value;
    }

    public int Width
    {
        get => width;
        set => width = value;
    }

    public int Height
    {
        get => height;
        set => height = value;
    }

    public float CellSize
    {
        get => cellSize;
        set => cellSize = value;
    }

    public float AvatarSize
    {
        get => avatarSize;
        set => avatarSize = value;
    }

    public float BackgroundSize
    {
        get => backgroundSize;
        set => backgroundSize = value;
    }

    public float FillTime
    {
        get => fillTime;
        set => fillTime = value;
    }

    public bool IsProcessing
    {
        get => isProcessing;
        set => isProcessing = value;
    }

    public int ObstaclesTotal
    {
        get => obstaclesTotal;
        set => obstaclesTotal = value;
    }

    public int MaxListImage
    {
        get => maxListImage;
        set => maxListImage = value;
    }

    public bool IsRevertFill
    {
        get => isRevertFill;
        set => isRevertFill = value;
    }

    public void Deconstruct(out Cell cell, out Transform backgroundBlock, out Transform gridBlock, out int width,
        out int height, out float cellSize, out float avatarSize, out float backgroundSize, out float fillTime,
        out bool isProcessing, out int obstaclesTotal, out int maxListImage, out bool isRevertFill)
    {
        cell = this.cell;
        backgroundBlock = this.backgroundBlock;
        gridBlock = this.gridBlock;
        width = this.width;
        height = this.height;
        cellSize = this.cellSize;
        avatarSize = this.avatarSize;
        backgroundSize = this.backgroundSize;
        fillTime = this.fillTime;
        isProcessing = this.isProcessing;
        obstaclesTotal = this.obstaclesTotal;
        maxListImage = this.maxListImage;
        isRevertFill = this.isRevertFill;
    }
}