using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerTileCheckTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> tilesInRange = new List<GameObject>();

    private BoxCollider2D triggerCollider;

    // Start is called before the first frame update
    void Awake()
    {
        triggerCollider = gameObject.AddComponent<BoxCollider2D>();

        triggerCollider.isTrigger = true; // 트리거로 설정

        triggerCollider.offset = new Vector2(-6.5f, 3.5f); // 트리거의 위치
        triggerCollider.size = new Vector2(16, 10f); // 트리거의 크기

        Rigidbody2D triggerRigidbody = gameObject.AddComponent<Rigidbody2D>();
        triggerRigidbody.isKinematic = true; 
        triggerRigidbody.simulated = true; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            GameObject tileObject = other.gameObject;

            tilesInRange.Add(tileObject);

            TileCheckCoroutine tileCoroutine = tileObject.GetComponent<TileCheckCoroutine>();
            if (tileCoroutine != null)
            {
                tileCoroutine.StartTileCoroutine();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            GameObject tileObject = other.gameObject;
            tilesInRange.Remove(tileObject);
        }
    }
}
