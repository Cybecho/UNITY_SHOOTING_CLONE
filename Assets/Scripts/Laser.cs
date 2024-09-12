using UnityEngine;

public class Laser : MonoBehaviour
{
    public float damage = 10f; // 레이저 데미지
    public float duration = 2f; // 레이저 지속 시간

    private LineRenderer lineRenderer;
    private float timer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        timer = duration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject); // 레이저 오브젝트 삭제
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnHit((int)damage); // 적에게 데미지 주기
            }
        }
    }
}