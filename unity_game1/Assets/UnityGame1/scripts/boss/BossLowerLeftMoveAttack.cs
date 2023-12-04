using UnityEngine;

public class BossLowerLeftMoveAttack : MonoBehaviour
{
    public float speed = 10f;
    public float deleteTime = 6f;
    private Vector3 moveDirection; // 移動方向

    void Start()
    {
        Destroy(gameObject, deleteTime);

        // 左下方向のベクトルを設定します（斜め左下）
        moveDirection = new Vector3(-1f, -1f, 0f).normalized;
    }

    void Update()
    {
        if (gameObject != null)
        {
            // 移動方向に速度を掛けて移動します
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }
}
