using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;         // 이동 속도
    public float power;         // 총알 파워
    public float maxShotDelay;  // 최대 총알 발사 딜레이
    public float curShotDelay;  // 현재 총알 발사 딜레이

    public bool isTouchTop;     // 위쪽 벽과 닿았는지 여부
    public bool isTouchBottom;  // 아래쪽 벽과 닿았는지 여부
    public bool isTouchRight;   // 오른쪽 벽과 닿았는지 여부
    public bool isTouchLeft;    // 왼쪽 벽과 닿았는지 여부

    public GameObject bulletObjA; // 총알 프리팹 A
    public GameObject bulletObjB; // 총알 프리팹 B

    Animator anim;              // 애니메이터

    void Awake()
    {
        anim = GetComponent<Animator>();                                        // 애니메이터 컴포넌트 가져오기
    }

    void Update()
    {
        Move();     // 이동 함수
        Fire();     // 발사 함수
        Reload();   // 재장전 함수 (총알 발사 딜레이 설정)
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if ((isTouchRight && h > 0) || (isTouchLeft && h < 0)) h = 0;
        if ((isTouchTop && v > 0) || (isTouchBottom && v < 0)) v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0).normalized * speed * Time.deltaTime;
        
        transform.position = curPos + nextPos;
        
        // 매 프레임마다 애니메이터 파라미터 업데이트
        UpdateAnimatorParameters(h);

        Debug.Log($"h value: {h}, Animator Input parameter: {anim.GetInteger("Input")}");
    }

    // 애니메이터 파라미터 업데이트
    void UpdateAnimatorParameters(float h)
    {
        if (h != 0) anim.SetInteger("Input", (int)Mathf.Sign(h));   // h 값이 0이 아니면 현재 h값을 애니메이터 파라미터 설정
        else anim.SetInteger("Input", 0);                           // h 값이 0이면 애니메이터 파라미터 0으로 설정
    }

    void Fire()
    {
        //! 나중에 특정 범위 내에 적이 있을 때만 자동으로 발사하도록 수정
        if(!Input.GetButton("Fire1")) return;   // Fire1 버튼을 누르지 않으면 함수를 종료
        if(curShotDelay < maxShotDelay) return; // 현재 총알 발사 딜레이가 최대 총알 발사 딜레이보다 작으면 함수를 종료

        switch(power)   // 파워에 따라 총알 오브젝트를 다르게 생성
        {
            case 1: // 일반 공격
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);    // 총알 오브젝트 생성
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                 // 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                   // 위쪽으로 힘을 가함
                break;
            case 2: // 따블 공격
                GameObject bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.15f, transform.rotation);    // 오른쪽 총알 오브젝트 생성
                GameObject bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.15f, transform.rotation);     // 왼쪽 총알 오브젝트 생성
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();                                                        // 오른쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();                                                        // 왼쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                break;
            case 3: //트리플 공격
                GameObject bulletRR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.3f, transform.rotation);    // 오른쪽 총알 오브젝트 생성
                GameObject bulletCC = Instantiate(bulletObjB, transform.position, transform.rotation);                            // 중앙 총알 오브젝트 생성
                GameObject bulletLL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.3f, transform.rotation);     // 왼쪽 총알 오브젝트 생성
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();                                                        // 오른쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();                                                        // 중앙 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();                                                        // 왼쪽 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                           // 위쪽으로 힘을 가함
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   
                break;
            default:
                break;
        }


        
        curShotDelay = 0;                       // 현재 총알 발사 딜레이를 0으로 초기화
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;  // 현재 총알 발사 딜레이에 시간을 더함
    }

    // 충돌 처리 (벽과 닿았을 때)
    void OnTriggerEnter2D(Collider2D collision)
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
    void OnTriggerExit2D(Collider2D collision)
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
}