using System.Collections;
using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class TypeCell : MonoBehaviour, IController
    {
        private CONSTANTS.CellType _type;
        private CONSTANTS.CellSpecialType _specialType;
        private static readonly int RowAnimator = Animator.StringToHash("Row");
        private static readonly int ColumnAnimator = Animator.StringToHash("Column");
        private static readonly int ClearAnimator = Animator.StringToHash("Clear");

        public CONSTANTS.CellSpecialType SpecialType
        {
            get => _specialType;
            set
            {
                var isRow = value == CONSTANTS.CellSpecialType.Row;
                var isColumn = value == CONSTANTS.CellSpecialType.Column;
                if (isColumn || isRow)
                {
                    StartCoroutine(SetTriggerIE(isRow));
                }

                _specialType = value;
            }
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

        private IEnumerator SetTriggerIE(bool isRow)
        {
            var animator = this.GetComponentInChildren<Animator>();
            animator.SetTrigger(ClearAnimator);
            yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);
            animator.SetTrigger(isRow ? RowAnimator : ColumnAnimator);
        }
        
        private void SetAvatar(CONSTANTS.CellType newType)
        {
            var avatar = this.GetComponentInChildren<SpriteRenderer>();
            var image = ConfigGame.Instance.Sprites[(int)newType];

            if (image == null)
            {
                avatar.sprite = null;
                return;
            }

            avatar.sprite = image;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}