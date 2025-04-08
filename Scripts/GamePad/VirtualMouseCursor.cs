using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lop.Game;

// 게임패드 사용 시 가상 마우스 커서의 이동을 돕는 클래스
public class VirtualMouseCursor : MonoBehaviour {
    [SerializeField] private GameObject cursorObject;
    [SerializeField] private Button homeButton;
    private RectTransform cursorTransform;
    [SerializeField] private List<Canvas> canvasList = new List<Canvas>();
    [SerializeField] private EventSystem eventSystem;
    private float speed = 2f;
    private Vector3 moveDirection = Vector2.zero;

    private void Awake() {
        if(Gamepad.all.Count == 0) {
            Destroy(cursorObject);
            Destroy(gameObject);
        }
    }

    void Start() {
        cursorTransform = cursorObject.GetComponent<RectTransform>();
    }

    private void Update() {

        if (moveDirection != Vector3.zero) {
            // 현재 UI 캔버스 크기 가져오기
            Vector2 canvasSize = ((RectTransform)cursorTransform.parent).sizeDelta;

            // 이동할 다음 위치 계산
            Vector2 nextPos = cursorTransform.anchoredPosition + (Vector2)(moveDirection * speed);

            // 화면 밖으로 나가지 않도록 제한
            float clampedX = Mathf.Clamp(nextPos.x, -canvasSize.x / 2, canvasSize.x / 2);
            float clampedY = Mathf.Clamp(nextPos.y, -canvasSize.y / 2, canvasSize.y / 2);

            // UI 좌표 업데이트
            cursorTransform.anchoredPosition = new Vector2(clampedX, clampedY);
        }
    }

    #region new Input system 
    public void OnMoveCursor(InputValue value) {
        Vector2 input = value.Get<Vector2>();
        //Debug.Log(input);
        if (cursorTransform != null) {
            moveDirection = (Vector3)input;
        }
    }

    public void OnMoveCursorOff(InputValue value) {
        moveDirection = Vector3.zero;
    }

    public void OnClick(InputValue value) {

        if (cursorTransform == null || eventSystem == null) return;

        // PointerEventData 생성
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Camera.main.WorldToScreenPoint(cursorTransform.position)
        };

        List<RaycastResult> results = new List<RaycastResult>();

        // 캔버스별로 레이캐스트를 쏴 버튼이라면 실행
        for (int i = 0; i < canvasList.Count; i++) {
            if (canvasList[i].TryGetComponent(out GraphicRaycaster caster))
                caster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results) {
                if(result.gameObject.TryGetComponent(out Button button)) {
                    if (button != null) {
                        button.onClick.Invoke(); // 버튼 클릭 실행
                        return;
                    }
                }else if (result.gameObject.TryGetComponent(out Toggle toggle)) {
                    if (toggle != null) {
                        toggle.isOn = !toggle.isOn;
                        return;
                    }
                }

            }
        }
    }

    public void OnBack(InputValue value) {
        homeButton?.onClick.Invoke();
    }
    #endregion
}