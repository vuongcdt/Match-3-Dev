using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class CreateCell: MonoBehaviour, IController
    {
        [SerializeField] private BoxCollider2D box2D;

        private Cell _cell;
        
        public Cell Create(Vector2 pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
        {
            box2D = this.GetComponent<BoxCollider2D>();
            _cell = this.GetComponent<Cell>();
            
            var configGame = ConfigGame.Instance;
            var isCellNormal =
                cellType is not (CONSTANTS.CellType.Background or CONSTANTS.CellType.Obstacle
                    or CONSTANTS.CellType.None);

            this.transform.localScale = Vector2.one * cellSize;

            Cell cell = null;
            if (configGame.Pool.Count > 0)
            {
                cell = configGame.Pool.Pop();
                cell.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                box2D.enabled = isCellNormal;
                cell = Instantiate(_cell, pos, Quaternion.identity, transformParent);
            }
            
            cell.InvertedCell.StopMoveIE();
            cell.transform.position = pos;
            cell.Type = cellType;
            
            if (cellType == CONSTANTS.CellType.Rainbow)
            {
                // cell._specialType = CONSTANTS.CellSpecialType.Rainbow;
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