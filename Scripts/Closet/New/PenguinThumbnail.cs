using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    /// <summary>
    /// 이전에 쓰던 펭귄 썸네일 이동 연출 클래스
    /// 지금은 안씀
    /// </summary>
    public class PenguinThumbnail : MonoBehaviour
    {
        // 위치 배열
        private Vector2[] rectPosArray = { new Vector2(-520, -100), new Vector2(-350, -85), new Vector2(-185, -45), 
            new Vector2(0, 0), new Vector2(185, -45), new Vector2(350, -85), new Vector2(520, -100) };

        // 위치 별 크기 배열
        private Vector2[] rectScaleArray = { new Vector2(90, 90),  new Vector2(100, 100), new Vector2(125, 125), 
            new Vector2(150, 150), new Vector2(125, 125), new Vector2(100, 100), new Vector2(90, 90) };

        private int myIndex;
        public int MyIndex => myIndex;

        private RectTransform myRect;

        private void Awake()
        {
            myRect = GetComponent<RectTransform>();
        }

        // 인덱스 변경
        public void SetLocation(int index, float moveTime)
        {
            myIndex = index;
            if (index < 0 || index > 6) return;

            StartCoroutine(MoveMyIndex(moveTime));
        }

        // 내 인덱스로 이동
        private IEnumerator MoveMyIndex(float moveTime)
        {
            float curTime = 0;

            Vector2 previousPos = myRect.anchoredPosition;
            float width = myRect.rect.width;
            float height = myRect.rect.height;

            while (curTime < moveTime )
            {
                curTime += Time.deltaTime;
                float t = curTime / moveTime;


                myRect.anchoredPosition = Vector2.Lerp(previousPos, rectPosArray[myIndex], t);
                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(width, rectScaleArray[myIndex].x, t));
                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(height, rectScaleArray[myIndex].y, t));

                yield return null;
            }

            myRect.anchoredPosition = rectPosArray[myIndex];
            myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectScaleArray[myIndex].x);
            myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectScaleArray[myIndex].y);
        }
    }
}