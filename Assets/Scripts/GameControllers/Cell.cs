using System.Collections;
using Commands;
using QFramework;
using UnityEngine;

public class Cell : MonoBehaviour, IController
{
    [SerializeField] private Sprite[] sprite;
    [SerializeField] private BoxCollider2D box2D;

    private CONSTANTS.CellType _type;
    private CONSTANTS.CellSpecialType _specialType;
    private IEnumerator _moveIE;
    private Vector3 _currentCellPos;
    private Vector3 _currentPointPos;
    private SpriteRenderer _avatar;
    private ConfigGame _configGame;

    private Vector3 _clampMagnitude;
    private bool _isDragged;

    public Vector3 Position => _currentCellPos;

    public CONSTANTS.CellSpecialType SpecialType
    {
        get => _specialType;
        set => _specialType = value;
    }

    public CONSTANTS.CellType Type
    {
        get => _type;
        set => _type = value;
    }


    private void Awake()
    {
        _avatar = this.GetComponentInChildren<SpriteRenderer>();
        _configGame = ConfigGame.Instance;
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
        cell.name = cellType.ToString();
        cell.SetAvatar(sprite[(int)cellType]);

        _currentCellPos = pos;

        return cell;
    }

    public void DeActive()
    {
        this.Type = CONSTANTS.CellType.None;
        this.gameObject.SetActive(false);
    }

    public void ReSetAvatar()
    {
        SetAvatar(sprite[(int)Type]);
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

        _currentCellPos = pos;
        this.transform.position = pos;
    }

    private void OnMouseDown()
    {
        if (_isDragged)
        {
            return;
        }

        _isDragged = true;
        _currentPointPos = GetWorldPoint();
    }

    private Vector3 GetWorldPoint()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        if (!_isDragged)
        {
            return;
        }

        var offset = GetOffset();

        _clampMagnitude = this.SendCommand(new GetClampMagnitudeVectorCommand(offset));
        var newPoint = _clampMagnitude * _configGame.CellSize * 0.9f + _currentCellPos;

        this.transform.position = newPoint;
    }

    private Vector3 GetOffset()
    {
        var input = GetWorldPoint();
        return input - _currentPointPos;
    }

    private void OnMouseUp()
    {
        if (!_isDragged)
        {
            return;
        }

        _isDragged = false;
        this.transform.position = _currentCellPos;
        var directionAxis = _clampMagnitude * _configGame.Sensitivity;

        directionAxis.Normalize();

        var targetPos = _currentCellPos + directionAxis * _configGame.CellSize;

        // var targetGrid = Utils.GetGridPos(targetPos.x, targetPos.y, _configGame.Width, _configGame.Height,
        //     _configGame.CellSize);
        // var grid = this.SendQuery(new GetGridQuery());
        // var targetCell = grid[targetGrid.x, targetGrid.y];
        //
        // var spriteSource = this.GetComponentInChildren<SpriteRenderer>();
        // spriteSource.color = Color.black;
        // var spriteTarget = targetCell.GetComponentInChildren<SpriteRenderer>();
        // spriteTarget.color = Color.black;
        //
        // StartCoroutine(SetColorIE(spriteSource, spriteTarget));

        StartCoroutine(InvertedCellIE(targetPos));
    }

    private IEnumerator SetColorIE(SpriteRenderer spriteSource, SpriteRenderer spriteTarget)
    {
        yield return new WaitForSeconds(_configGame.FillTime * 5);
        spriteSource.color = Color.white;
        spriteTarget.color = Color.white;
    }

    private IEnumerator InvertedCellIE(Vector3 targetPos)
    {
        yield return new WaitForSeconds(_configGame.FillTime);

        this.SendCommand(new InvertedCellCommand(_currentCellPos, targetPos));
        var matchGridCommandIE = this.SendCommand(new MatchGridCommandIE());
        StartCoroutine(matchGridCommandIE);
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}