using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class Cell : MonoBehaviour, IController
    {
        private PositionCell _positionCell;
        private InvertedCell _invertedCell;
        private TypeCell _typeCell;

        public InvertedCell InvertedCell => _invertedCell;

        public Utils.GridPos GridPosition
        {
            get => _positionCell.GridPosition;
            set => _positionCell.GridPosition = value;
        }

        public Animator Animator => _typeCell.Animator;

        public CONSTANTS.CellSpecialType SpecialType
        {
            get => _typeCell.SpecialType;
            set => _typeCell.SpecialType = value;
        }
        
        public CONSTANTS.CellType Type
        {
            get => _typeCell.Type;
            set => this.GetComponent<TypeCell>().Type = value;
        }

        private void Awake()
        {
            _positionCell = this.GetComponent<PositionCell>();
            _invertedCell = this.GetComponent<InvertedCell>();
            _typeCell = this.GetComponent<TypeCell>();
        }

        public void ClearFish()
        {
            _typeCell.ClearFish();
        }
        
        public void ClearObstacle()
        {
            _typeCell.ClearObstacle();
        }

        public void ClearCell()
        {
            _typeCell.ClearCell();
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}