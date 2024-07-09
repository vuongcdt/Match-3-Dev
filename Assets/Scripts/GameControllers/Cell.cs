using System.Collections;
using Commands;
using QFramework;
using Queries;
using UnityEngine;

public class Cell : MonoBehaviour, IController
{
    [SerializeField] private BoxCollider2D box2D;

    private CONSTANTS.CellType _type;
    private CONSTANTS.CellSpecialType _specialType;
    private IEnumerator _moveIE;
    private Utils.GridPos _gridPos;
    private Vector3 _worldPos;
    private ConfigGame _configGame;

    private Vector3 _clampMagnitude;
    private bool _isDragged;

    public Utils.GridPos GridPosition
    {
        get => _gridPos;
        set
        {
            _gridPos = value;
            _worldPos = GetWorldPos(value);
            Move(value, ConfigGame.Instance.FillTime);
        }
    }

    public Vector3 WorldPosition
    {
        get => _worldPos;
        set
        {
            SetWorldPosition(value);
            _worldPos = value;
            Move(value, ConfigGame.Instance.FillTime);
        }
    }


    public CONSTANTS.CellSpecialType SpecialType
    {
        get => _specialType;
        set => _specialType = value;
    }

    public CONSTANTS.CellType Type
    {
        get => _type;
        set
        {
            _type = value;
            SetAvatar(value);
        }
    }

    private void Awake()
    {
        _configGame = ConfigGame.Instance;
    }

    private void SetWorldPosition(Vector3 value)
    {
        var gridPos = GetGridPos(value);
        var configGame = ConfigGame.Instance;

        if (gridPos.x >= 0 && gridPos.x < configGame.Width && gridPos.y >= 0 && gridPos.y < configGame.Height)
        {
            _gridPos = gridPos;
        }
    }

    public Cell Create(Utils.GridPos pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
    {
        var worldPos = GetWorldPos(pos);
        return Create(worldPos, transformParent, cellSize, cellType);
    }

    private Cell Create(Vector2 pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
    {
        var isCellNormal =
            cellType is not (CONSTANTS.CellType.Background or CONSTANTS.CellType.Obstacle or CONSTANTS.CellType.None);

        box2D.enabled = isCellNormal;

        this.transform.localScale = Vector2.one * cellSize;
        var cell = Instantiate(this, pos, Quaternion.identity, transformParent);

        cell._worldPos = pos;
        cell.Type = cellType;
        cell.name = cellType.ToString();

        return cell;
    }

    private void SetAvatar(CONSTANTS.CellType type)
    {
        var avatar = this.GetComponentInChildren<SpriteRenderer>();
        var image = ConfigGame.Instance.Sprites[(int)type];
        if (image == null)
        {
            avatar.sprite = null;
            return;
        }

        avatar.sprite = image;
    }

    public void DeActive()
    {
        this.Type = CONSTANTS.CellType.None;
        this.gameObject.SetActive(false);
    }

    private void Move(Utils.GridPos pos, float time)
    {
        var worldPos = GetWorldPos(pos);
        Move(worldPos, time);
    }

    private void Move(Vector2 pos, float time)
    {
        if (_moveIE != null)
        {
            StopCoroutine(_moveIE);
        }

        _moveIE = MoveIE(pos, time);
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(_moveIE ?? MoveIE(pos, time));
        }
    }

    private IEnumerator MoveIE(Vector2 pos, float time)
    {
        time *= 20;
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, pos, t / time);
            yield return null;
        }

        _worldPos = pos;
        this.transform.position = pos;
    }

    private void OnMouseDown()
    {
        if (_configGame.IsDragged)
        {
            return;
        }

        _configGame.IsDragged = true;
        _worldPos = GetWorldPoint();
    }

    private Vector3 GetWorldPoint()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        if (!_configGame.IsDragged)
        {
            return;
        }

        var offset = GetOffset();

        _clampMagnitude = this.SendCommand(new GetClampMagnitudeVectorCommand(offset));
        var newPoint = _clampMagnitude * _configGame.CellSize * 0.9f + _worldPos;

        this.transform.position = newPoint;
    }

    private Vector3 GetOffset()
    {
        var input = GetWorldPoint();
        return input - _worldPos;
    }

    private void OnMouseUp()
    {
        if (!_configGame.IsDragged)
        {
            return;
        }

        this.transform.position = _worldPos;
        var directionAxis = _clampMagnitude * _configGame.Sensitivity;

        directionAxis.Normalize();

        var targetPos = _worldPos + directionAxis * _configGame.CellSize;

        var targetGrid = Utils.GetGridPos(targetPos.x, targetPos.y, _configGame.Width, _configGame.Height,
            _configGame.CellSize);
        var grid = this.SendQuery(new GetGridQuery());
        var targetCell = grid[targetGrid.x, targetGrid.y];

        var spriteSource = this.GetComponentInChildren<SpriteRenderer>();
        spriteSource.color = Color.black;
        var spriteTarget = targetCell.GetComponentInChildren<SpriteRenderer>();
        spriteTarget.color = Color.black;

        StartCoroutine(SetColorIE(spriteSource, spriteTarget));
    }

    private IEnumerator SetColorIE(SpriteRenderer spriteSource, SpriteRenderer spriteTarget)
    {
        yield return new WaitForSeconds(_configGame.FillTime * 3);
        spriteSource.color = Color.white;
        spriteTarget.color = Color.white;
    }

    private void InvertedCell(Vector3 targetPos)
    {
        this.SendCommand(new InvertedCellCommand(_worldPos, targetPos));
        this.SendCommand(new MatchGridCommandIE());
        _configGame.IsDragged = false;
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    private Utils.GridPos GetGridPos(Vector3 pos)
    {
        var configGame = ConfigGame.Instance;
        return Utils.GetGridPos(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
    }

    private Vector3 GetWorldPos(Utils.GridPos pos)
    {
        var configGame = ConfigGame.Instance;
        return Utils.GetWorldPosition(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
    }
}