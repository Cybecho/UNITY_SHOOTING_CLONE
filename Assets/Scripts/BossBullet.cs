using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public int dmg;                                         // 총알 데미지

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = Random.Range(0.8f, 1.8f);         // 중력 값을 랜덤 값으로 설정
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")      // BorderBullet 태그와 충돌 시
        {
            Destroy(gameObject);                            // 총알 오브젝트 삭제
        }
    }
}