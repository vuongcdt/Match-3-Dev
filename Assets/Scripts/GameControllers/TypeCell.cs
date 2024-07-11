using QFramework;
using UnityEngine;

namespace GameControllers
{
    public class TypeCell : MonoBehaviour, IController
    {
        private CONSTANTS.CellType _type;
        private CONSTANTS.CellSpecialType _specialType;
        private static readonly int ClearAnimator = Animator.StringToHash("Clear");
        
        public CONSTANTS.CellSpecialType SpecialType
        {
            get => _specialType;
            set => _specialType = value;
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