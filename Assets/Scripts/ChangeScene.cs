using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    void Update()
    {
        // 현재 씬의 이름을 가져옴
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬이 "GameOver" 또는 "GameClear"인지 확인
        if (currentSceneName == "GameOver" || currentSceneName == "GameClear")
        {
            // SpaceBar 또는 마우스 클릭 이벤트 감지
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                // "SampleScene"으로 이동
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
}