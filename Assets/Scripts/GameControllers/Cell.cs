using System;
using System.Collections;
using Commands;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class Cell : MonoBehaviour, IController
    {
        [SerializeField] private Utils.GridPos _gridPos;
        [SerializeField] private Vector3 _worldPos;

        private CONSTANTS.CellType _type;
        private CONSTANTS.CellSpecialType _specialType;
        private IEnumerator _moveIE;
        private Animator _animator;

        private Vector3 _clampMagnitude;
        private bool _isDragged;
        private static readonly int RowAnimator = Animator.StringToHash("Row");
        private static readonly int ColumnAnimator = Animator.StringToHash("Column");
        private static readonly int ClearAnimator = Animator.StringToHash("Clear");

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

        public Vector3 WorldPosition => _worldPos;

        public CONSTANTS.CellSpecialType SpecialType
        {
            get => _specialType;
            set => StartCoroutine(SetTriggerIE(value));
        }

        public CONSTANTS.CellType Type
        {
            get => _type;
            set
            {
                SetAvatar(value);
                _type = value;
            }
        }

        public Animator Animator => _animator;

        private void Awake()
        {
            _animator = this.GetComponentInChildren<Animator>();
        }

        private IEnumerator SetTriggerIE(CONSTANTS.CellSpecialType specialType)
        {
            var isRow = specialType == CONSTANTS.CellSpecialType.Row;
            var isColumn = specialType == CONSTANTS.CellSpecialType.Column;

            if (isRow || isColumn)
            {
                this.Animator.SetTrigger(ClearAnimator);
                yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);

                this.Animator.SetTrigger(isRow ? RowAnimator : ColumnAnimator);
            }
            _specialType = specialType;

        }
        private void SetAvatar(CONSTANTS.CellType newType)
        {
            var avatar = this.GetComponentInChildren<SpriteRenderer>();
            var image = ConfigGame.Instance.Sprites[(int)newType];

            if (image == null) //TODO
            {
                avatar.sprite = null;
                return;
            }

            avatar.sprite = image;
        }

        public void ClearObstacle()
        {
            StartCoroutine(SetTypeIE());
        }

        public void ClearCell()
        {
            StartCoroutine(SetTypeIE());
        }

        private IEnumerator SetTypeIE()
        {
            _animator.SetTrigger(ClearAnimator);
            _type = CONSTANTS.CellType.None;
            yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);
            this.Type = CONSTANTS.CellType.None;
        }

        public void DeActive()
        {
            this.Type = CONSTANTS.CellType.None;
        }

        public void StopMoveIE()
        {
            if (_moveIE != null)
            {
                StopCoroutine(_moveIE);
            }
        }

        private void Move(Utils.GridPos pos, float time)
        {
            var worldPos = GetWorldPos(pos);

            _moveIE = MoveIE(worldPos, time);
            StartCoroutine(_moveIE);
            this.gameObject.name = $"{this._type.ToString()} {this._gridPos.x}_{this._gridPos.y}";
        }

        private IEnumerator MoveIE(Vector2 pos, float time)
        {
            for (float t = 0; t <= time * ConfigGame.Instance.TimeScale; t += Time.deltaTime)
            {
                this.transform.position =
                    Vector3.Lerp(this.transform.position, pos, t / (time));
                yield return null;
            }

            _worldPos = pos;
            _gridPos = GetGridPos(pos);
            // this.transform.position = pos;
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

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}