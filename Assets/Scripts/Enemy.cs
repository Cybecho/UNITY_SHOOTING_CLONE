using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed; // 이동 속도
    public int health; // 체력
    public Sprite[] sprites; // 스프라이트 배열
    private Rigidbody2D rigid; // 리지드바디 컴포넌트
    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러 컴포넌트
    private Vector2 savedVelocity; // 정지 전 속도를 저장할 변수
    private bool isPaused = false; // 일시정지 여부

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>(); // 리지드바디 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 컴포넌트 가져오기
        rigid.velocity = Vector2.down * speed; // 아래쪽으로 이동
        savedVelocity = rigid.velocity; // 초기 속도 저장
    }

    void Update()
    {
        if (isPaused)
        {
            return;
        }

        // 적의 움직임 로직 추가
    }

    void OnHit(int dmg)
    {
        health -= dmg; // 체력 감소
        spriteRenderer.sprite = sprites[1]; // 피격당했을때 스프라이트 변경
        Invoke("ReturnSprite", 0.1f); // 0.1초 후 원래 스프라이트로 변경

        if (health <= 0) // 체력이 0 이하이면
        {
            Destroy(gameObject); // 게임 오브젝트 삭제
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0]; // 원래 스프라이트로 변경
    }

    public void StopMovement()
    {
        isPaused = true; // 일시정지 상태로 변경
        if (rigid != null) // 리지드바디가 null이 아닌 경우에만 실행
        {
            savedVelocity = rigid.velocity; // 현재 속도를 저장
            rigid.velocity = Vector2.zero; // 속도를 0으로 설정하여 움직임 정지
        }
    }

    public void ResumeMovement()
    {
        isPaused = false; // 일시정지 해제
        if (rigid != null && savedVelocity != Vector2.zero) // 리지드바디가 null이 아니고 저장된 속도가 0이 아닌 경우에만 재시작
        {
            rigid.velocity = savedVelocity; // 저장된 속도로 움직임 재시작
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BorderBullet")) // 충돌한 오브젝트의 태그가 BorderBullet이면
        {
            Destroy(gameObject); // 게임 오브젝트 삭제
        }
        else if (collision.gameObject.CompareTag("PlayerBullet")) // 충돌한 오브젝트의 태그가 PlayerBullet이면
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>(); // 충돌한 오브젝트의 Bullet 컴포넌트 가져오기
            OnHit(bullet.dmg); // 피격 함수 호출
            Destroy(collision.gameObject); // 충돌한 오브젝트 삭제
        }
    }
}