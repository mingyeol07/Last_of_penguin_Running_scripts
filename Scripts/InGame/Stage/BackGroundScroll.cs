using Lop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroll : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform[] backgrounds;
    public bool IsStop;

    private void FixedUpdate()
    {
        if (IsStop) return;

        Scrolling();
    }

    private void Scrolling()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].Translate(Vector2.left * speed);
            if (backgrounds[i].localPosition.x <= -19)
            {
                backgrounds[i].localPosition = new Vector3(38, 0, 0);
            }
        }
    }
}
