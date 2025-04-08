using Lop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected int contactPenguinNumber;
    protected GameManagerParent gameManager;

    private void Start()
    {
        gameManager = GameManagerParent.Instance;
    }

    /// <summary>
    /// 플레이어와 닿았을 때 실행되는 함수
    /// </summary>
    protected abstract void ContactPlayer();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            contactPenguinNumber = collision.transform.parent.parent.GetComponent<PenguinBase>().PlayerNumber;

            ContactPlayer();
            Destroy(this.gameObject);
        }
    }
}
