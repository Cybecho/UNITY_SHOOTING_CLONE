using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public GameObject bulletObj;
    public GameObject player; // Player 오브젝트를 참조

    public float followSpeed = 2.0f; // Follower의 따라오는 속도
    public float yOffset = 1.0f; // Follower가 Player의 뒤에 있을 거리

    void Update()
    {
        Follow();
        AutoFire();
        Reload();
    }

    void Follow()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y - yOffset, player.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    void Fire()
    {
        
    }

    void AutoFire()
    {
        
    }

    void Reload()
    {
        
    }
}