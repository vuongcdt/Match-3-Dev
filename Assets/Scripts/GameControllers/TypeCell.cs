using System;
using System.Collections;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameControllers
{
    public class TypeCell : MonoBehaviour, IController
    {
        [SerializeField] private SpriteRenderer avatar;

        private CONSTANTS.CellType _type;
        private CONSTANTS.CellSpecialType _specialType;
        private Animator _animator;

        private static readonly int ClearAnimator = Animator.StringToHash("Clear");
        private static readonly int DefaultAnimator = Animator.StringToHash("Default");

        public Animator Animator => _animator;

        public CONSTANTS.CellSpecialType SpecialType
        {
            get => _specialType;
            set
            {
                _animator.SetTrigger(DefaultAnimator);
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

        private void Awake()
        {
            _animator = this.GetComponentInChildren<Animator>();
            avatar = this.GetComponentInChildren<SpriteRenderer>();
        }

        private void SetAvatar(CONSTANTS.CellType newType)
        {
            var image = ConfigGame.Instance.Sprites[(int)newType];

            if (image == null)
            {
                avatar.sprite = null;
                return;
            }

            avatar.sprite = image;
        }

        public void ClearFish()
        {
            _animator.SetTrigger(ClearAnimator);
            StartCoroutine(SetTypeIE());
        }

        private IEnumerator SetTypeIE()
        {
            yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);
            ClearCell();
        }

        public void ClearCell()
        {
            // this.SpecialType = CONSTANTS.CellSpecialType.Normal;
            this.Type = CONSTANTS.CellType.None;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}