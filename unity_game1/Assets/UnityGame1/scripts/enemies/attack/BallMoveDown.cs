using UnityEngine;

public class BallMoveDown : MonoBehaviour {
    public float speed = 10f;
    public float deleteTime = 12f;

    void Start() {
        Destroy(gameObject, deleteTime);
    }

    void Update() {
        if (gameObject != null) {
            Vector3 lazerPos = transform.position;
            lazerPos.y -= speed * Time.deltaTime;
            transform.position = lazerPos;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        bool isAttackBall = (other.gameObject.CompareTag(Tags.AttackBall));
        bool isEnemy = (other.gameObject.CompareTag(Tags.Enemy));
        bool isMoveFloor = (other.gameObject.CompareTag(Tags.MoveFloor));
        bool isJumpGimmick = (other.gameObject.CompareTag(Tags.JumpGimmick));
        if (!isEnemy && !isMoveFloor && !isJumpGimmick && !isAttackBall) Destroy(this.gameObject);
    }
}
