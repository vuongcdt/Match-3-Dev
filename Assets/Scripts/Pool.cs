using System.Collections.Generic;
using GameControllers;
using UnityEngine;

public class Pool : Singleton<Pool>
{
    [SerializeField]private Cell cell;
    
    private Stack<Cell> _pool = new();

    private Cell Rent()
    {
        return _pool.Count > 0 ? _pool.Pop() : Instantiate(ConfigGame.Instance.Cell, Vector3.zero, Quaternion.identity);
    }

    public void Return(Cell cellReturn)
    {
        _pool.Push(cellReturn);
    }

    public Cell Create(Utils.GridPos gridPos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
    {
        var worldPos = GetWorldPos(gridPos);
        var isCellNormal =
            cellType is not (CONSTANTS.CellType.Background or CONSTANTS.CellType.Obstacle
                or CONSTANTS.CellType.None);

        var cell = Rent();

        cell.transform.SetParent(transformParent);
        cell.transform.position = worldPos;
        cell.transform.localScale = Vector2.one * cellSize;
        cell.GetComponent<BoxCollider2D>().enabled = isCellNormal;
        cell.GridPosition = GetGridPos(worldPos);
        cell.Type = cellType;
        cell.InvertedCell.StopMoveIE();

        if (cellType == CONSTANTS.CellType.Rainbow)
        {
            cell.SpecialType = CONSTANTS.CellSpecialType.Rainbow;
        }

        return cell;
    }

    private Vector3 GetWorldPos(Utils.GridPos pos)
    {
        var configGame = ConfigGame.Instance;
        return Utils.GetWorldPosition(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
    }
    
    private Utils.GridPos GetGridPos(Vector3 pos)
    {
        var configGame = ConfigGame.Instance;
        return Utils.GetGridPos(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
    }

}