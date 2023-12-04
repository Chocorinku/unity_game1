using UnityEngine;

public class PlayerBallMove : MonoBehaviour
{
    public bool facingRight = true;
    private float speed = 18f;
    private float deleteTime = 8f;
    public float ballRate = 5f;
    void Start()
    {
        Destroy(gameObject, deleteTime);
    }

    void Update()
    {
        if (gameObject != null)
        {
            Vector3 lazerPos = transform.position;

            // プレイヤーの向きによって移動方向を切り替え
            if (facingRight)
            {
                lazerPos.x += speed * Time.deltaTime;
            }
            else
            {
                lazerPos.x -= speed * Time.deltaTime;
            }
            transform.Rotate(0f, 0f, ballRate);
            transform.position = lazerPos;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        bool isPlayer = (other.gameObject.CompareTag(Tags.Player));
        bool isSign = (other.gameObject.CompareTag(Tags.Sign));
        bool isMoveFloor = (other.gameObject.CompareTag(Tags.MoveFloor));
        bool isJumpGimmick = (other.gameObject.CompareTag(Tags.JumpGimmick));
        if (!isPlayer && !isSign && !isMoveFloor && !isJumpGimmick)
            Destroy(this.gameObject);
    }
}
