using Events;
using Interfaces;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIGame.Scripts
{
    public class CardItem : MonoBehaviour, IController, ICanSendEvent
    {
        [SerializeField] private Sprite starActive, starDeActive, unLockLeverImg;
        [SerializeField] private Color bgColorChecked, bgColorUnlock, levelTextColorChecked, levelTextColorUnLock, cornerBgUnlock;

        [SerializeField] private GameObject focus, groupImageEffect, bgGlow, starBlock;
        [SerializeField] private RectTransform lockLevelBlock;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Image[] starIconImages;
        [SerializeField] private Image bgCard, cornerBg, lockLeverIcon;

        private IGameModel _gameModel;

        public void SetLevelCardItem(int levelNum)
        {
            levelText.text = levelNum.ToString();
        }

        public void SetCardItem(CONSTANTS.TypeCard typeCard, int levelNum, int starsNum)
        {
            var button = this.GetComponent<Button>();
            button.onClick.AddListener(Onclick);

            if (typeCard == CONSTANTS.TypeCard.UnLock)
            {
                bgCard.color = bgColorUnlock;
                cornerBg.color = cornerBgUnlock;
                levelText.color = levelTextColorUnLock;

                lockLeverIcon.sprite = unLockLeverImg;
                lockLevelBlock.sizeDelta = new Vector2(100, 100);
                
                bgGlow.SetActive(true);
            }

            if (typeCard == CONSTANTS.TypeCard.Checked)
            {
                bgCard.color = bgColorChecked;
                cornerBg.color = Color.white;
                levelText.color = levelTextColorChecked;

                lockLevelBlock.gameObject.SetActive(false);

                focus.SetActive(true);
                bgGlow.SetActive(false);
                groupImageEffect.SetActive(true);
                starBlock.SetActive(true);
            }


            levelText.text = levelNum.ToString();
            for (var index = 0; index < starIconImages.Length; index++)
            {
                starIconImages[index].sprite = index < starsNum ? starActive : starDeActive;
            }
        }

        private void Onclick()
        {
            var level = int.Parse(levelText.text);

            this.SendEvent(new LevelSelectEvent(level));
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}