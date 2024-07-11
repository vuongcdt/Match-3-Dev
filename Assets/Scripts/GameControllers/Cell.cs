using System;
using System.Collections;
using Commands;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class Cell : MonoBehaviour, IController
    {
        [SerializeField] private BoxCollider2D box2D;

        private PositionCell _positionCell;
        private InvertedCell _invertedCell;
        private CreateCell _createCell;
        private TypeCell _typeCell;
        
        private static readonly int ClearAnimator = Animator.StringToHash("Clear");

        public InvertedCell InvertedCell
        {
            get => _invertedCell;
            set => _invertedCell = value;
        }

        public Utils.GridPos GridPosition
        {
            get => _positionCell.GridPosition;
            set => _positionCell.GridPosition = value;
        }

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

        public Cell Create(Utils.GridPos pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
        {
            var worldPos = GetWorldPos(pos);
            _createCell = this.GetComponent<CreateCell>();
            return _createCell.Create(worldPos, transformParent, cellSize, cellType);
        }

        public void ClearCell()
        {
            var animator = this.GetComponentInChildren<Animator>();
            animator.SetTrigger(ClearAnimator);
            StartCoroutine(SetTypeIE());
        }

        private IEnumerator SetTypeIE()
        {
            yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);
            _typeCell.Type = CONSTANTS.CellType.None;
        }

        public void DeActive()
        {
            _typeCell.Type = CONSTANTS.CellType.None;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        private Vector3 GetWorldPos(Utils.GridPos pos)
        {
            var configGame = ConfigGame.Instance;
            return Utils.GetWorldPosition(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
        }
    }
}