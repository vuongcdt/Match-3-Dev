using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIGame.Scripts
{
    public class CardItem : MonoBehaviour
    {
        [SerializeField] private Sprite starActive, starDeActive, lockLevelImg;
        [SerializeField] private Color bgColorChecked, bgColorChecking, bgColorCanCheck;
        [SerializeField] private Color levelTextColorChecked, levelTextColorChecking, levelTextColorCanCheck;

        [SerializeField] private GameObject focus, groupImageEffect, checkIcon, bgGlow, starBlock, lockLevelBlock;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Image[] starIconImages;
        [SerializeField] private Image bgCard, cornerBg;

        public void SetLevelCardItem(int levelNum)
        {
            levelText.text = levelNum.ToString();
        }

        public void SetCardItem(CONSTANTS.TypeCard typeCard, int levelNum, int starsNum)
        {
            var button = this.GetComponent<Button>();
            button.onClick.AddListener(Onclick);

            if (typeCard == CONSTANTS.TypeCard.CanCheck)
            {
            }

            if (typeCard == CONSTANTS.TypeCard.Checking)
            {
                bgCard.color = bgColorChecked;
                cornerBg.color = Color.white;
                levelText.color = levelTextColorChecked;

                checkIcon.SetActive(true);
                bgGlow.SetActive(false);
                focus.SetActive(true);
                starBlock.SetActive(true);
                lockLevelBlock.SetActive(false);
            }

            if (typeCard == CONSTANTS.TypeCard.Checked)
            {
                bgCard.color = bgColorChecking;
                cornerBg.color = Color.white;
                levelText.color = levelTextColorChecking;

                bgGlow.SetActive(false);
                groupImageEffect.SetActive(true);
                starBlock.SetActive(true);
                lockLevelBlock.SetActive(false);
            }


            levelText.text = levelNum.ToString();
            for (var index = 0; index < starIconImages.Length; index++)
            {
                starIconImages[index].sprite = index < starsNum ? starActive : starDeActive;
            }
        }

        private void Onclick()
        {
            var homeScreen = this.GetComponentInParent<HomeScreen>();
            homeScreen.OnClickPlayLevel(int.Parse(levelText.text));
        }
    }
}