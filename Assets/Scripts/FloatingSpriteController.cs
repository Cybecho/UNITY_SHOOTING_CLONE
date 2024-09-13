using UnityEngine;

public class FloatingSpriteController : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatAmount = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Material spriteMaterial;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject!");
            return;
        }

        // Try to find the shader using Resources.Load
        Shader floatingShader = Resources.Load<Shader>("Shaders/FloatingSprite");
        
        if (floatingShader == null)
        {
            Debug.LogError("Could not find shader 'FloatingSprite'. Make sure the shader is properly imported and the name is correct.");
            return;
        }

        // Create a new material instance
        spriteMaterial = new Material(floatingShader);
        
        // Set the new material to the sprite renderer
        spriteRenderer.material = spriteMaterial;

        Debug.Log("FloatingSpriteController initialized successfully.");
    }
}