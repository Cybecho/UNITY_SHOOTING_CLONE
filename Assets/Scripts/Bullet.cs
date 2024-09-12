using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;                                         // 총알 데미지

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")      // BorderBullet 태그와 충돌 시
        {
            Destroy(gameObject);                            // 총알 오브젝트 삭제
        }
    }
}
