using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public GameObject bulletObj;
    public GameObject target; // 따라야 할 대상

    public float followSpeed = 5.0f; // Follower의 따라오는 속도
    public float yOffset = 1.0f; // Follower가 대상의 뒤에 있을 거리

    void Update()
    {
        Follow();
        AutoFire();
        Reload();
    }

    void Follow()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y - yOffset, target.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = Instantiate(bulletObj, transform.position, transform.rotation); // 총알 오브젝트 생성
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>(); // 총알 오브젝트의 Rigidbody2D 컴포넌트 가져오기
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // 위쪽으로 힘을 가함

        curShotDelay = 0; // 현재 총알 발사 딜레이 초기화
    }

    void AutoFire()
    {
        curShotDelay += Time.deltaTime; // 현재 총알 발사 딜레이에 시간을 더함

        if (curShotDelay >= maxShotDelay)
        {
            Fire(); // 발사 함수 호출
            curShotDelay = 0; // 현재 총알 발사 딜레이 초기화
        }
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime; // 현재 총알 발사 딜레이에 시간을 더함
    }
}