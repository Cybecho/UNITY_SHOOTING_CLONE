using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFollower : MonoBehaviour
{
    public GameObject parentObject; // 따라갈 부모 객체

    // Start is called before the first frame update
    void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object is not assigned.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (parentObject != null)
        {
            // 부모 객체의 위치를 현재 객체의 위치로 설정
            transform.position = parentObject.transform.position;
        }
    }
}