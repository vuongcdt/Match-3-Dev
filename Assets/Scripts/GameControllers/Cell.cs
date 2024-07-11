using System.Collections;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class Cell : MonoBehaviour, IController
    {
        private PositionCell _positionCell;
        private InvertedCell _invertedCell;
        // private CreateCell _createCell;
        private TypeCell _typeCell;

        // public CreateCell CreateCell => _createCell;

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
            // _createCell = this.GetComponent<CreateCell>();
        }

        // public Cell Create(Utils.GridPos pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
        // {
        //     var worldPos = GetWorldPos(pos);
        //     return _createCell.Create(worldPos, transformParent, cellSize, cellType);
        // }

        public void ClearFish()
        {
            _typeCell.ClearFish();
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