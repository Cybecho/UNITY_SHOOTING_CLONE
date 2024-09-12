using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoss : MonoBehaviour
{
    public float shadowInterval = 0.1f;                     // 잔상 생성 간격
    private int shadowCount = 0;                            // 현재 생성된 잔상의 수

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = Random.Range(1f, 5f);         // 중력 값을 1에서 5 사이의 랜덤 값으로 설정
        }

        StartCoroutine(CreateShadows());                    // 잔상 생성 코루틴 시작
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")      // BorderBullet 태그와 충돌 시
        {
            Destroy(gameObject);                            // 총알 오브젝트 삭제
        }
    }

    IEnumerator CreateShadows()
    {
        while (shadowCount < 2)
        {
            GameObject shadow = Instantiate(gameObject, transform.position, transform.rotation);
            shadow.transform.localScale = transform.localScale * 0.5f; // 크기를 절반으로 줄임
            Collider2D shadowCollider = shadow.GetComponent<Collider2D>();
            if (shadowCollider != null)
            {
                shadowCollider.enabled = false;             // 콜라이더 비활성화
            }
            shadowCount++;
            Destroy(shadow, 1f);                            // 잔상 오브젝트를 1초 후에 삭제
            yield return new WaitForSeconds(shadowInterval);
        }
    }
}