using System;
using System.Collections;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class PositionCell : MonoBehaviour, IController
    {
        private Utils.GridPos _gridPos;
        private Vector3 _worldPos;
        private IEnumerator _moveIE;
        private TypeCell _typeCell;

        public Vector3 WorldPosition => _worldPos;

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

        private void Awake()
        {
            _typeCell = this.GetComponent<TypeCell>();
        }

        private void Move(Utils.GridPos pos, float time)
        {
            var worldPos = GetWorldPos(pos);
            _moveIE = MoveIE(worldPos, time);

            StartCoroutine(_moveIE);
            this.gameObject.name = $"{_typeCell.Type.ToString()} {this._gridPos.x}_{this._gridPos.y}";
        }

        private IEnumerator MoveIE(Vector2 pos, float time)
        {
            for (float t = 0; t <= time * ConfigGame.Instance.TimeScale; t += Time.deltaTime)
            {
                this.transform.position =
                    Vector3.Lerp(this.transform.position, pos, t / (time));
                yield return null;
            }

            _gridPos = GetGridPos(pos);
            this.transform.position = pos;
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