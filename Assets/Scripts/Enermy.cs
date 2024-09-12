using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enermy : MonoBehaviour
{
    public float speed;             // 이동 속도
    public int health;              // 체력
    public Sprite[] sprites;        // 에너미 스프라이트

    SpriteRenderer spriteRenderer;  // 스프라이트 렌더러 컴포넌트 (피격당했을때 색 변경)
    Rigidbody2D rigid;              // 리지드바디 컴포넌트
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    // 스프라이트 렌더러 컴포넌트 가져오기
        rigid = GetComponent<Rigidbody2D>();                // 리지드바디 컴포넌트 가져오기
        rigid.velocity = Vector2.down * speed;              // 아래쪽으로 이동
    }

    void OnHit(int dmg)
    {
        health -= dmg;                          // 체력 감소
        spriteRenderer.sprite = sprites[1];     // 피격당했을때 스프라이트 변경
        Invoke("ReturnSprite", 0.1f);           // 0.1초 후 원래 스프라이트로 변경

        if (health <= 0)                        // 체력이 0 이하이면
        {
            Destroy(gameObject);                // 게임 오브젝트 삭제
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];     // 원래 스프라이트로 변경
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet") // 충돌한 오브젝트의 태그가 BorderBullet이면
        {
            Destroy(gameObject);              // 충돌한 오브젝트 삭제
        }
        else if (collision.gameObject.tag == "PlayerBullet") // 충돌한 오브젝트의 태그가 PlayerBullet이면
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();// 충돌한 오브젝트의 Bullet 컴포넌트 가져오기
            OnHit(bullet.dmg);                                          // 피격 함수 호출
            Destroy(collision.gameObject);                              // 충돌한 오브젝트 삭제
        }
    }
}
