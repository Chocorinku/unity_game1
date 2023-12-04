using SupanthaPaul;
using UnityEditor;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int attackCounterIncrease = 1; // アイテムによって増加させる攻撃回数

    public SpriteRenderer spriteRenderer;
    private Vector2 originalSpriteSize;
    public float sim = 0.1f;
    public float max = 1.3f;

    private void Start() {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        originalSpriteSize = spriteRenderer.size;
    }

    void Update() {
        if ((originalSpriteSize.x * max) > spriteRenderer.size.x) {
            spriteRenderer.size += new Vector2(spriteRenderer.size.x ,spriteRenderer.size.y ) * sim;
        }else  {
            spriteRenderer.size -= new Vector2(spriteRenderer.size.x, spriteRenderer.size.y) * sim;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Player)) // プレイヤーとの衝突を検出
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.IncreaseAttackCounter(attackCounterIncrease); // attackCounterを増加
                Destroy(gameObject); // アイテムを削除
            }
        }
    }
}