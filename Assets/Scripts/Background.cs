using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;         // 배경 스크롤 속도
    public Transform sprite;    // 배경 스프라이트

    float viewHeight;           // 카메라의 세로 크기
    private bool isPaused = false; // 배경 스크롤 일시정지 여부
    public Vector3 initialPosition; // 초기 위치 저장

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;

        // sprite 변수가 할당되지 않은 경우 자동으로 할당
        if (sprite == null)
        {
            sprite = GetComponentInChildren<Transform>();
        }

        initialPosition = transform.position; // 초기 위치 저장
    }
    
    void Update()
    {
        if (!isPaused)
        {
            Move();
        }
    }

    void Move()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (transform.position.y < -viewHeight)
        {
            // 스프라이트의 위치를 맨 위로 옮김
            transform.position = new Vector3(transform.position.x, transform.position.y + viewHeight * 3, transform.position.z);
        }
    }

    public void StopMovement()
    {
        isPaused = true;
    }

    public void ResumeMovement()
    {
        isPaused = false;
    }

    public void AdjustPosition(float offsetY)
    {
        transform.position = new Vector3(transform.position.x, initialPosition.y + offsetY, transform.position.z);
    }
}