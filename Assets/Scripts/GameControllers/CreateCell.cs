using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class CreateCell : MonoBehaviour, IController
    {
        [SerializeField] private BoxCollider2D box2D;
        private TypeCell _typeCell;

        private BoxCollider2D Box2D => box2D;

        public Cell Create(Vector2 pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
        {
            var isCellNormal =
                cellType is not (CONSTANTS.CellType.Background or CONSTANTS.CellType.Obstacle
                    or CONSTANTS.CellType.None);

            var cell = Pool.Instance.Rent();

            cell.transform.SetParent(transformParent);
            cell.transform.position = pos;
            cell.transform.localScale = Vector2.one * cellSize;
            cell.CreateCell.Box2D.enabled = isCellNormal;
            cell.GridPosition = GetGridPos(pos);
            cell.Type = cellType;
            cell.InvertedCell.StopMoveIE();

            if (cellType == CONSTANTS.CellType.Rainbow)
            {
                cell.SpecialType = CONSTANTS.CellSpecialType.Rainbow;
            }

            return cell;
        }

        private Utils.GridPos GetGridPos(Vector3 pos)
        {
            var configGame = ConfigGame.Instance;
            return Utils.GetGridPos(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}