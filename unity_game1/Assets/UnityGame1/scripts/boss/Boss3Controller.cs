using System.Collections;
using UnityEngine;

public class Boss3Controller : MonoBehaviour {
    [SerializeField] private Transform _upLeft;
    [SerializeField] private Transform _upRight;
    [SerializeField] private Transform _downLeft;
    [SerializeField] private Transform _downRight;
    [SerializeField] private Transform[] attackPoints;
    [SerializeField] private Transform[] attackLeftPoints;
    public GameObject attackKori1;
    public GameObject attackKori2;
    public GameObject attackKori3;
    public GameObject attackKori4;
    public GameObject attackLeftKori;

    public float moveSpeed = 6.0f;
    public float moveStateChangeInterval = 2.5f;
    public float attackObjDestroyTime = 4.0f;

    private Animator animator;
    private MOVE_STATE currentMoveState;
    private MOVE_STATE lastMoveState;
    private bool isFirstAttackInvalid = true;
    Enemy enemy;
    public enum MOVE_STATE {
        IDLE,
        UP_RIGHT,
        UP_LEFT,
        DOWN_RIGHT,
        DOWN_LEFT
    }

    void Start() {
        animator = GetComponent<Animator>();
        lastMoveState = MOVE_STATE.DOWN_RIGHT;
        InvokeRepeating("ChangeMoveState", 0f, moveStateChangeInterval);
        enemy = this.gameObject.GetComponent<Enemy>();
    }

    void Update() { }

    void FixedUpdate() {
        switch (currentMoveState) {
            case MOVE_STATE.IDLE:
                break;
            case MOVE_STATE.UP_RIGHT:
                transform.position = Vector3.MoveTowards(transform.position, _upRight.position, moveSpeed * Time.fixedDeltaTime);
                break;
            case MOVE_STATE.UP_LEFT:
                transform.position = Vector3.MoveTowards(transform.position, _upLeft.position, moveSpeed * Time.fixedDeltaTime);
                break;
            case MOVE_STATE.DOWN_RIGHT:
                transform.position = Vector3.MoveTowards(transform.position, _downRight.position, moveSpeed * Time.fixedDeltaTime);
                break;
            case MOVE_STATE.DOWN_LEFT:
                transform.position = Vector3.MoveTowards(transform.position, _downLeft.position, moveSpeed * Time.fixedDeltaTime);
                break;
            default:
                break;
        }
    }

    private void ChangeMoveState() {
        do {
            // 現在のステートとは異なるステートをランダムに選ぶまで繰り返す
            currentMoveState = (MOVE_STATE)Random.Range(0, 5);
        } while (currentMoveState == lastMoveState);
        lastMoveState = currentMoveState;

        // 攻撃処理
        switch (currentMoveState) {
            case MOVE_STATE.IDLE:
                // Attack1();
                break;
            case MOVE_STATE.UP_RIGHT:
                Attack1();
                break;
            case MOVE_STATE.UP_LEFT:
                // Attack1();
                break;
            case MOVE_STATE.DOWN_RIGHT:
                Attack2();
                break;
            case MOVE_STATE.DOWN_LEFT:
                // Attack2();
                break;
            default:
                break;
        }

        Debug.Log("Move State: " + currentMoveState.ToString());
    }

    private void Attack1() {
        if (isFirstAttackInvalid || enemy.life <= 0) {
            // ボスステージすぐに横攻撃が来ると即死の可能性がある為、最初塞ぐ
            Debug.Log("Boss FirstAttackInvalid");
            isFirstAttackInvalid = false;
            return;
        }
        Debug.Log("Boss Attack1");
        animator.SetTrigger(BossAnimTags.Attack);

        int attackCount = Random.Range(4, 9);
        for (int i = 0; i < attackCount; i++) {
            int randomIndex = Random.Range(0, attackPoints.Length);
            GameObject attackObj = Instantiate(GetRandomAttackObject(), attackPoints[randomIndex].position, Quaternion.identity);
            Destroy(attackObj, attackObjDestroyTime);
        }
    }

    private void Attack2() {
        if (isFirstAttackInvalid || enemy.life <= 0) {
            // ボスステージすぐに横攻撃が来ると即死の可能性がある為、最初塞ぐ
            Debug.Log("Boss FirstAttackInvalid");
            isFirstAttackInvalid = false;
            return;
        }

        Debug.Log("Boss Attack2");
        animator.SetTrigger(BossAnimTags.Attack);

        int attackCount = Random.Range(2, 6);
        for (int i = 0; i < attackCount; i++) {
            // コルーチンの起動
            StartCoroutine(DelayCoroutine());
        }
    }

    // コルーチン本体
    private IEnumerator DelayCoroutine() {
        int consecutiveAttacks = Random.Range(2, 5);    //弾の連続数
        for (int n = 0; n < consecutiveAttacks; n++) {
            // 50フレーム待つ
            //for (var i = 0; i < 50; i++) {
            //    yield return null;
            //}
            // N秒間待つ
            yield return new WaitForSeconds(0.5f);
            BulletStrikeAttack2();
        }
    }
    private void BulletStrikeAttack2() {    //弾の生成
        int randomIndex = Random.Range(0, attackLeftPoints.Length);
        if (transform.position.x < attackLeftPoints[randomIndex].position.x) {
            // 自分の位置より左から発射しようとしている場合、自分より左側から攻撃が出るようにする。
            Vector3 attackPosition = new Vector3(transform.position.x - 5f, attackLeftPoints[randomIndex].position.y, attackLeftPoints[randomIndex].position.z);
            Instantiate(attackLeftKori, attackPosition, Quaternion.Euler(0f, 0f, -90f));
        } else {
            Instantiate(attackLeftKori, attackLeftPoints[randomIndex].position, Quaternion.Euler(0f, 0f, -90f));
        }
    }

    private GameObject GetRandomAttackObject() {
        GameObject[] attackObjects = { attackKori1, attackKori2, attackKori3, attackKori4 };
        int randomIndex = Random.Range(0, attackObjects.Length);
        return attackObjects[randomIndex];
    }
}
