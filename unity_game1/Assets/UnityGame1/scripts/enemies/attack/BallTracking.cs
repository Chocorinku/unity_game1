using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTracking : MonoBehaviour {
    public float speed = 10f;
    public float deleteTime = 12f;

    //プレイヤーオブジェクト
    public GameObject player;

    //一秒ごとに弾を発射するためのもの
    private float targetTime = 1.0f;
    private float currentTime = 0;
    Vector2 vec;

    void Start() {
        player = GameObject.Find("Player");
        Destroy(gameObject, deleteTime);
        //敵の座標を変数posに保存
        var pos = this.gameObject.transform.position;

        //敵からプレイヤーに向かうベクトルをつくる
        //プレイヤーの位置から敵の位置（弾の位置）を引く
        vec = player.transform.position - pos;
    }

    void Update() {
        //一秒経つごとに弾を発射する
        //currentTime += Time.deltaTime;
        //if (targetTime < currentTime) {
            //currentTime = 0;

            //弾のRigidBody2Dコンポネントのvelocityに先程求めたベクトルを入れて力を加える
            this.gameObject.GetComponent<Rigidbody2D>().velocity = vec * speed;
        //}
    }
    private void OnTriggerEnter2D(Collider2D other) {
        bool isAttackBall = (other.gameObject.CompareTag(Tags.AttackBall));
        bool isEnemy = (other.gameObject.CompareTag(Tags.Enemy));
        bool isMoveFloor = (other.gameObject.CompareTag(Tags.MoveFloor));
        bool isJumpGimmick = (other.gameObject.CompareTag(Tags.JumpGimmick));
        if (!isEnemy && !isMoveFloor && !isJumpGimmick && !isAttackBall) Destroy(this.gameObject);
    }
}
