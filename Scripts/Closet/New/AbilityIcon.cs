using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lop.Game
{
    // 마우스커서를 대었을 때 말풍선을 띄우는 클래스
    public class AbilityIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // 스킬 설명 말풍선
        [SerializeField] private GameObject speechBubble;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnSpeachBubbleToggle(true);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnSpeachBubbleToggle(false);
        }

        private void OnSpeachBubbleToggle(bool value)
        {
            speechBubble.SetActive(value);

            if (value) transform.SetAsLastSibling();
        }
    }
}