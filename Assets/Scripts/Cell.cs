using System.Collections;
using QFramework;
using Queries;
using UnityEngine;

public class Cell : MonoBehaviour, IController
{
    [SerializeField] private Sprite[] sprite;
    [SerializeField] private CONSTANTS.CellType _type;
    [SerializeField] private BoxCollider2D box2D;

    private IEnumerator _moveIE;
    private Vector2 _currentPos;
    private SpriteRenderer _avatar;
    private Utils.SettingsGrid _settingsGrid;

    public CONSTANTS.CellType Type
    {
        get => _type;
        set => _type = value;
    }


    private void Awake()
    {
        _avatar = this.GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _settingsGrid = this.SendQuery(new GetSettingsGridQuery());
    }

    public Cell Create(Vector2 pos, Transform transformParent, float cellSize,
        CONSTANTS.CellType cellType)
    {
        var isCellNormal =
            cellType is not (CONSTANTS.CellType.Background or CONSTANTS.CellType.Obstacle or CONSTANTS.CellType.None);

        box2D.enabled = isCellNormal;

        this.transform.localScale = Vector2.one * cellSize;
        var cell = Instantiate(this, pos, Quaternion.identity, transformParent);

        cell._type = cellType;
        cell.SetAvatar(sprite[(int)cellType]);
        cell.name = cellType.ToString();

        _currentPos = pos;

        return cell;
    }

    public void ReSetAvatar()
    {
        SetAvatar(sprite[(int)Type]);
        // this.gameObject.name = _type.ToString();
    }

    private void SetAvatar(Sprite image)
    {
        if (image == null)
        {
            _avatar.sprite = null;
            return;
        }

        _avatar.sprite = image;
    }


    public void Move(Vector2 pos, float time)
    {
        if (_moveIE != null)
        {
            StopCoroutine(_moveIE);
        }

        _moveIE = MoveIE(pos, time);
        StartCoroutine(_moveIE);
    }

    private IEnumerator MoveIE(Vector2 pos, float time)
    {
        time *= 20;
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, pos, t / time);
            yield return null;
        }

        _currentPos = pos;
        this.transform.position = pos;
    }

    private void OnMouseDrag()
    {
        var offset = GetOffset();
        this.transform.position =
            Vector3.ClampMagnitude(offset, _settingsGrid.CellSize) + (Vector3)_currentPos + Vector3.back;
        // this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private float _sensitivity = 2f;

    private void OnMouseUp()
    {
        var offset = GetOffset();
        offset.Normalize();
        var min = Mathf.Abs(offset.x) - Mathf.Abs(offset.y) > 0 ? offset.y : offset.x;

        var directionAxis = (offset - new Vector2(min, min)) * _sensitivity;
        directionAxis.Normalize();
        Debug.Log($"directionAxis {directionAxis} {_settingsGrid.CellSize}");
        InvertedCell(_currentPos, _currentPos + directionAxis * _settingsGrid.CellSize);
    }

    private void InvertedCell(Vector2 sourcePos, Vector2 targetPos)
    {
        Debug.Log($"targetPos {targetPos}");
        // if (Mathf.Abs(targetPos.x) < 1 && Mathf.Abs(targetPos.y) < 1)
        // {
        //     Debug.Log($"return @@@@@@@@@@@@ {targetPos}");
        //     this.transform.position = _currentPos;
        //     return;
        // }

        var grid = this.SendQuery(new GetGridQuery());
        var sourceGridPos = GetGridPos(sourcePos);
        var targetGridPos = GetGridPos(targetPos);
        Debug.Log($"sourceGridPos {sourceGridPos.ToVector2()} targetGridPos {targetGridPos.ToVector2()}");

        var sourceCell = grid[sourceGridPos.x, sourceGridPos.y];
        var targetCell = grid[targetGridPos.x, targetGridPos.y];

        sourceCell.Move(targetPos, 0.1f);
        targetCell.Move(sourcePos, 0.1f);

        grid[sourceGridPos.x, sourceGridPos.y] = targetCell;
        grid[targetGridPos.x, targetGridPos.y] = sourceCell;
    }

    private Utils.GridPos GetGridPos(Vector2 pos)
    {
        var width = (-pos.x / _settingsGrid.CellSize + (_settingsGrid.Width - 1) * 0.5f);
        var height = (-pos.y / _settingsGrid.CellSize + (_settingsGrid.Height - 1) * 0.5f);

        var gridWidth = _settingsGrid.Width - 1 - (int)width;
        var gridHeight = _settingsGrid.Height - 1 - (int)height;
        Debug.Log(new Vector2(gridWidth, gridHeight));

        return new Utils.GridPos(gridWidth, gridHeight);
    }

    private Vector2 GetOffset()
    {
        Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return input - _currentPos;
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}