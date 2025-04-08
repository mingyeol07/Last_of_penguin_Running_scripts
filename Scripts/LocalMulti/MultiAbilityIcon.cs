using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lop.Game
{
    public class MultiAbilityIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject speechBubble;


        /// <summary>
        /// 마우스 커서를 가져다 대면 보이는 팝업 버블
        /// </summary>
        [Header("SpeechBubble")]
        [SerializeField] private TMP_Text txt_abilityName;
        [SerializeField] private TMP_Text txt_abilityDescription;
        [SerializeField] private Image img_ability;
        [SerializeField] private Image img_myIcon;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (speechBubble == null) return;
            speechBubble.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (speechBubble == null) return;
            speechBubble.SetActive(false);
        }

        public void InitSpeechBubble(PenguinData data)
        {
            txt_abilityName.text = data.BattleAbility_name;
            txt_abilityDescription.text = data.BattleAbility;
            img_ability.sprite = data.BattleAbility_sprite;
            img_myIcon.sprite = data.BattleAbility_sprite;
        }
    }
}