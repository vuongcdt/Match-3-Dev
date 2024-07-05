using TMPro;
using UnityEngine;

namespace BaseScripts
{
    public class CellBase : MonoBehaviour
    {
        protected int Row;
        protected int Column;
        private CONSTANTS.CellType _type;
        private TMP_Text _text;
        private SpriteRenderer _avatar;
        
        public CONSTANTS.CellType Type => _type;

        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
            _avatar = GetComponentInChildren<SpriteRenderer>();
        }

        public virtual CellBase Create(Vector2 pos, Transform transformParent, float cellSize)
        {
            this.transform.localScale = Vector2.one * cellSize;
            return Instantiate(this, pos, Quaternion.identity, transformParent);
        }

        public virtual void SetText(string text)
        {
            _text.text = text;
        }

        public virtual void SetAvatarColor(Color color)
        {
            _avatar.color = color;
        }

        public virtual void SetAvatar(Sprite sprite)
        {
            _avatar.sprite = sprite;
        }
        
        public virtual void SetType(CONSTANTS.CellType type)
        {
           _type = type;
        }

        public virtual void SetCell(int row, int column)
        {
            Column = column;
            Row = row;
        }

        public virtual void SetAll(int row, int column, Color color, string text)
        {
            Column = column;
            Row = row;
            _avatar.color = color;
            _text.text = text;
        }

        public virtual void SetAll(int row, int column, Sprite sprite)
        {
            _type = CONSTANTS.CellType.None;
            Column = column;
            Row = row;
            _avatar.sprite = sprite;
            
            if(_text != null)
            {
                _text.text = null;
            }
        }
    }
}