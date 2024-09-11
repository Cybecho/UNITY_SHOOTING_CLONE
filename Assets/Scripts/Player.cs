using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; // 이동 속도
    public bool isTouchTop;     // 위쪽 벽과 닿았는지 여부
    public bool isTouchBottom;  // 아래쪽 벽과 닿았는지 여부
    public bool isTouchRight;   // 오른쪽 벽과 닿았는지 여부
    public bool isTouchLeft;    // 왼쪽 벽과 닿았는지 여부

    void Update()
    {
        float h = Input.GetAxis("Horizontal");                                  // 방향 키 입력
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) h = 0;        // 오른쪽 벽과 닿았고 오른쪽 방향키 입력 시
        
        float v = Input.GetAxis("Vertical");                                    // 방향 키 입력
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1)) v = 0;        // 위쪽 벽과 닿았고 위쪽 방향키 입력 시

        Vector3 curPos = transform.position;                                    // 현재 위치
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;        // 다음 위치
    
        transform.position = curPos + nextPos;                                  // 이동
    }

    // 충돌 처리 (벽과 닿았을 때)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")    // Border 태그와 충돌 시
        {
            switch(collision.gameObject.name)       // 충돌한 오브젝트의 이름으로 분기
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
    }

    // 충돌 해제
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")    // Border 태그와 충돌 시
        {
            switch (collision.gameObject.name)       // 충돌한 오브젝트의 이름으로 분기
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
