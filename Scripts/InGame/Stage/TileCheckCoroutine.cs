using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCheckCoroutine : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Collider2D tileCollider;

    public void StartTileCoroutine()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        tileCollider = GetComponent<BoxCollider2D>();

        StartCoroutine(CheckPlayerPosition());
    }

    // 플레이어의 좌표를 지속적으로 확인하는 코루틴
    private IEnumerator CheckPlayerPosition()
    {
        while (player.transform.position.x < transform.position.x)
        {
            UpdateTileColliderState();
            yield return null;
        }
    }

    public void UpdateTileColliderState()
    {
        float tileTop = transform.position.y + (transform.localScale.y / 2); // 타일의 제일 위 위치 계산
        float playerY = player.transform.position.y; // 플레이어의 현재 Y 좌표

        // 플레이어가 타일보다 위에 있을 때만 콜라이더 활성화
        if (playerY > tileTop)
        {
            tileCollider.enabled = true;
        }
        else if (playerY < tileTop)
        {
            tileCollider.enabled = false;
        }
    }

}
