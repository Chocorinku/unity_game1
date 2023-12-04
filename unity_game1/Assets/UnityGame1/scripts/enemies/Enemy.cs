using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    #region
    [HideInInspector] public bool playerStepOn;
    [SerializeField] private bool possibilityKnockback;   //?m?b?N?o?b?N???????L????????????
    [SerializeField] private bool isBoss;
    public int life = 3;   //???C?t
    bool invincible;    //?_???[?W???????G?t???O
    SpriteRenderer sprite;
    GameObject childSprite; //?_??????????????GameObject
    Collider2D col;
    Rigidbody2D rb;
    public float gravity = 3f;
    public float knockBackPower = 10f;   // ?m?b?N?o?b?N????????
    Vector3 otherPosition;
    [SerializeField] private float torqueAmount = 600.0f;
    public GameObject BreakableWall;
    [SerializeField] GameObject ConversationArea;
    public Color daedColor = new Color(113,113,113,175);
    #endregion

    void Start() {
        if (isBoss) {
            childSprite = transform.Find("Sprites").gameObject;
        } else {
            sprite = GetComponent<SpriteRenderer>();
        }
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate() {
        if (playerStepOn) {
            Debug.Log("Enemy.playerStepOn: true");
            AttackFromPlayers();
        }
    }
    private void AttackFromPlayers() {
        playerStepOn = false;
        if (possibilityKnockback && life > 1) KnokBack();
        LifeDamage(1);
    }
    private void KnokBack() {
        //TODO?@?m?b?N?o?b?N???????r??
        //rb.velocity = Vector3.zero;
        if (otherPosition.x > transform.position.x) {
            rb.velocity = new Vector2(-4f, 4f);
        } else {
            rb.velocity = new Vector2(4f, 4f);
        }//???????m?b?N?o?b?N?????????f
    }
    private void OnTriggerEnter2D(Collider2D other) {
        bool isRetryBoard = (other.gameObject.CompareTag(Tags.RetryBoard));
        bool isAttackBall = (other.gameObject.CompareTag(Tags.PlayerAttackBall));
        if (isRetryBoard && !isBoss) {
            IsDaed();
        }
        if (isAttackBall) {
            AttackFromPlayers();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        bool isPlayer = (collision.collider.CompareTag(Tags.Player));
        if (isPlayer) {
            otherPosition = collision.gameObject.transform.position;
        }
    }

    //TODO  ?????????????????]???????????????????????B?~???????????????????????]??????????????????
    void IsDaed() {
        if (isBoss && BreakableWall != null)
        {
            ConversationArea.SetActive(true);
            BreakableWall.SetActive(false);
        }
        gameObject.GetComponent<Renderer>().material.color = daedColor; // new Color(173, 255, 47);
        col.enabled = false;
        if (otherPosition.x > transform.position.x) {
            rb.velocity = new Vector2(-6f, -gravity);
        } else {
            rb.velocity = new Vector2(6f, -gravity);
        }
        this.rb.AddForce(transform.up * knockBackPower, ForceMode2D.Impulse);
        rb.AddTorque(Mathf.Deg2Rad * torqueAmount, ForceMode2D.Impulse);
        Destroy(gameObject, 3f);
    }

    //Life??????????
    void LifeDamage(int number = 0) {
        if (invincible) {
            return;
        }
        life -= number;
        if (life > 0) {
            StartCoroutine(DamageTimer(25, 0.05f));
        }
        //if (life == 1) 
        if (life <= 0) {
            rb.freezeRotation = false;
            IsDaed();
        }
    }
    //?_???[?W?????????u???????G???????^?C?}?[
    IEnumerator DamageTimer(int num, float interval) {
        //?????_???[?W?????????I??
        if (invincible) {
            yield break;
        }
        invincible = true;
        //animator.SetTrigger("Damage");
        //???G?????????_??
        if (isBoss) {
            for (int i = 0; i < num; i++) {
                childSprite.SetActive(false);
                yield return new WaitForSeconds(interval);
                childSprite.SetActive(true);
                yield return new WaitForSeconds(interval);
            }
        } else {
            for (int i = 0; i < num; i++) {
                sprite.enabled = false;
                yield return new WaitForSeconds(interval);
                sprite.enabled = true;
                yield return new WaitForSeconds(interval);
            }
        }
        invincible = false;
    }

    public bool getInvincible() {
        return invincible;
    }
}
