using UnityEngine;

public class DeadEnemyController : MonoBehaviour
{
    public float flySpeed = 50f;
    public float rotateSpeed = 720f;
    public float lifetime = 1.5f;
    public bool isDead = false; // 외부에서 상태 변경 가능

    private Vector3 flyDirection;
    private float elapsedTime = 0f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Set random rotation direction
        float randomRotation = Random.Range(-1f, 1f);
        rotateSpeed *= randomRotation;

        // Set slightly random fly direction (mostly upwards)
        flyDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1, 0).normalized;

        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead)
        {
            // Move upwards
            transform.Translate(flyDirection * flySpeed * Time.deltaTime, Space.World);

            // Rotate
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

            // Check lifetime
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= lifetime)
            {
                // 렌더링 비활성화
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }
                Destroy(gameObject);
            }
        }
    }
}