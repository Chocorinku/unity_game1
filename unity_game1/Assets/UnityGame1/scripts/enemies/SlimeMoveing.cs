using UnityEngine;

public class SlimeMoveing : MonoBehaviour
{
    public float speed = 2.0f;  // モンスターの移動速度
    public float moveDistance = 1.0f;  // 移動距離
    private Vector3 initialPosition;  // 最初に配置された位置
    private int direction = 1;  // モンスターの進行方向
    Enemy enemy;

    void Start()
    {
        // 最初に配置された位置を保持
        initialPosition = transform.position;
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (!enemy.getInvincible())
        {
            // モンスターを移動させる
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

            // 移動方向を設定
            if (transform.position.x <= initialPosition.x - moveDistance)
            {
                direction = 1;
            }
            else if (transform.position.x >= initialPosition.x + moveDistance)
            {
                direction = -1;
            }
        }
    }

}
