using System;
using System.Collections;
using BaseScripts;
using UnityEngine;

public class Cell : MonoBehaviour
{
    protected int Row;
    protected int Column;
    private CONSTANTS.CellType _type;

    public CONSTANTS.CellType Type
    {
        get => _type;
        set => _type = value;
    }

    private SpriteRenderer _avatar;
    [SerializeField] private Sprite[] sprite;

    private void Awake()
    {
        _avatar = GetComponentInChildren<SpriteRenderer>();
    }

    public Cell Create(Vector2 pos, Transform transformParent, float cellSize,
        CONSTANTS.CellType cellType)
    {
        this.transform.localScale = Vector2.one * cellSize;
        var cell = Instantiate(this, pos, Quaternion.identity, transformParent);

        cell.SetAvatar(sprite[(int)cellType]);
        return cell;
    }

    public Cell CreateBackground(Vector2 pos, Transform transformParent, float cellSize)
    {
        this.transform.localScale = Vector2.one * cellSize;
        var obstacle = Instantiate(this, pos, Quaternion.identity, transformParent);
        obstacle.name = "Background";
        obstacle.SetAvatar(sprite[(int)CONSTANTS.CellType.Background]);
        return obstacle;
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

    private IEnumerator _moveIE;
    private float t;

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
        time *= 10;
        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, pos, t / time);
            yield return null;
        }

        this.transform.position = pos;
    }
}