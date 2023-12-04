using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTracking : MonoBehaviour {
    public float speed = 10f;
    public float deleteTime = 12f;

    //�v���C���[�I�u�W�F�N�g
    public GameObject player;

    //��b���Ƃɒe�𔭎˂��邽�߂̂���
    private float targetTime = 1.0f;
    private float currentTime = 0;
    Vector2 vec;

    void Start() {
        player = GameObject.Find("Player");
        Destroy(gameObject, deleteTime);
        //�G�̍��W��ϐ�pos�ɕۑ�
        var pos = this.gameObject.transform.position;

        //�G����v���C���[�Ɍ������x�N�g��������
        //�v���C���[�̈ʒu����G�̈ʒu�i�e�̈ʒu�j������
        vec = player.transform.position - pos;
    }

    void Update() {
        //��b�o���Ƃɒe�𔭎˂���
        //currentTime += Time.deltaTime;
        //if (targetTime < currentTime) {
            //currentTime = 0;

            //�e��RigidBody2D�R���|�l���g��velocity�ɐ�����߂��x�N�g�������ė͂�������
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
