using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    // 기존이라면 메인메뉴의 캐릭터를 마우스로 돌릴 수 있게 돕는 클래스 지금은 안씀
    public class MainSceneCharacter : MonoBehaviour
    {
        private readonly Vector3 penguinPosition = new Vector3(3.6f, 15, 85);
        private readonly Vector3 penguinRotation = new Vector3(5, 190, 0);
        private readonly Vector3 penguinScale = new Vector3(2, 2, 2);

        [SerializeField] private PenguinData[] penguins;

        private GameObject mainPenguin;

        [SerializeField] private float rotSpeed = 3f;

        private void Start()
        {
            SetPenguin();
        }
        private void SetPenguin()
        {
            int penguinId = PlayerPrefs.GetInt(PlayerPrefsKey.My_PenguinID);
            GameObject penguin = Instantiate(penguins[penguinId].PenguinPrefab);

            penguin.transform.position = penguinPosition;
            penguin.transform.eulerAngles = penguinRotation;
            penguin.transform.localScale = penguinScale;

            mainPenguin = penguin;
        }

        void Update()
        {
            //if (Input.GetMouseButton(0))
            //{
            //    mainPenguin.transform.Rotate(0f, -Input.GetAxis("Mouse X") * rotSpeed, 0f, Space.World);
            //}
        }
    }
}
