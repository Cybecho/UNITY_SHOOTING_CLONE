using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public float speed = 4.0f; // 이동 속도

    void Update()
    {
        // 위에서 아래로 이동
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 플레이어와 충돌 시
        {
            // 충돌 처리 로직
            Debug.Log("Goal reached by player!");
        }
    }
}