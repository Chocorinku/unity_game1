using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SupanthaPaul {
    public class PlayerController : MonoBehaviour {
        #region
        [SerializeField] private float speed;
        [Header("Jumping")]
        [SerializeField] private float jumpForce;
        [SerializeField] private float fallMultiplier;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private int extraJumpCount = 1;
        [SerializeField] private GameObject jumpEffect;
        [Header("Dashing")]
        [SerializeField] private float dashSpeed = 30f;
        [Tooltip("Amount of time (in seconds) the player will be in the dashing speed")]
        [SerializeField] private float startDashTime = 0.1f;
        [Tooltip("Time (in seconds) between dashes")]
        [SerializeField] private float dashCooldown = 0.2f;
        [SerializeField] private GameObject dashEffect;
        [HideInInspector] public bool isGrounded;
        [HideInInspector] public float moveInput;
        [HideInInspector] public float updownInput;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool isDashing = false;
        [HideInInspector] public bool actuallyWallGrabbing = false;
        [HideInInspector] public bool isCurrentlyPlayable = false;
        [Header("Wall grab & jump")]
        [Tooltip("Right offset of the wall detection sphere")]
        public Vector2 grabRightOffset = new Vector2(0.16f, 0f);
        public Vector2 grabLeftOffset = new Vector2(-0.16f, 0f);
        public float grabCheckRadius = 0.24f;
        public float slideSpeed = 2.5f;
        public Vector2 wallJumpForce = new Vector2(10.5f, 18f);
        public Vector2 wallClimbForce = new Vector2(4f, 14f);

        private Animator animator;
        private Rigidbody2D m_rb;
        private ParticleSystem m_dustParticle;
        private bool m_facingRight = true;
        private readonly float m_groundedRememberTime = 0.25f;
        private float m_groundedRemember = 0f;
        private int m_extraJumps;
        private float m_extraJumpForce;
        private float m_dashTime;
        private bool m_hasDashedInAir = false;
        private bool m_onWall = false;
        private bool m_onRightWall = false;
        private bool m_onLeftWall = false;
        private bool m_wallGrabbing = false;
        private readonly float m_wallStickTime = 0.25f;
        private float m_wallStick = 0f;
        private bool m_wallJumping = false;
        private float m_dashCooldown;

        [Header("Variables for dash")]
        public float tapTimer; // タップしてる時間の格納
        Vector2 startTouchPos;
        Vector2 endTouchPos;
        private Vector3 mag;
        public float flickGraceTimer = 0.3f;    //フリック猶予時間
        public float flickGraceLength = 250f;    //フリック猶予移動距離
        public bool flickFlag;
        private bool isDoubleTapStart;      //タップ認識中のフラグ
        private bool doubleTapFlag;     //ダブルタップ
        public float doubleTapTimer = 0.15f;
        private float extraJampTimer;
        public float coordinateAngle;    //スワイプしてる角度(座標角度)
        public float circumferentialAngle;  //角度を180のやつで求めることができる。(円周角度)
        bool jumpFlag;
        [SerializeField] private int extraDashCount = 3;
        private int m_extraDashs;
        private float m_extraDashForce;
        public float Mag;   //インセプターでリアルタイムでフリックの距離を見たい時の為の変数
        public bool oneJump = true;     //ジャンプを一回に制御する為の変数
        bool invincible;    //ダメージ中の無敵フラグ
        public int life = 3;   //ライフ
        [SerializeField] PlayerLife lifeHeartObj;
        GameObject childSprite; //点滅させるためのGameObject
        private CapsuleCollider2D capcol;
        private int m_onWallSide = 0;
        private int m_playerSide = 1;
        public int attackCounter = 3;
        public Text attackCounterText;
        public GameObject attackBallRigntObj;
        public GameObject attackBallLeftObj;
        GManager gManager;
        [Header("ジャンプする時に鳴らすSE")] public AudioClip jumpSE;
        [Header("ダメージSE")] public AudioClip damageSE;
        [Header("アイテム獲得SE")] public AudioClip itemSE;
        [Header("魔法弾SE")] public AudioClip magicBallSE;
        [Header("ダッシュSE")] public AudioClip dashSE;
        #endregion
        void Start() {
            // create pools for particles
            PoolManager.instance.CreatePool(dashEffect, 2);
            PoolManager.instance.CreatePool(jumpEffect, 2);

            // if it's the player, make this instance currently playable
            if (transform.CompareTag(Tags.Player))
                isCurrentlyPlayable = true;

            m_extraJumps = extraJumpCount;
            m_extraDashs = extraDashCount;
            m_dashTime = startDashTime;
            m_dashCooldown = dashCooldown;
            m_extraJumpForce = jumpForce * 0.8f;
            m_extraDashForce = dashSpeed * 0.8f;

            m_rb = GetComponent<Rigidbody2D>();
            m_dustParticle = GetComponentInChildren<ParticleSystem>();
            capcol = GetComponent<CapsuleCollider2D>();
            gManager = GameObject.Find("_GameManger").GetComponent<GManager>();

            childSprite = transform.Find("Sprites").gameObject;
            animator = GetComponent<Animator>();
            if (SceneManager.GetActiveScene().name == "Stage1")
            {
                attackCounter = 3; // 初期回数
                life = 3;
                Debug.Log("スタートのライフは " + life);
            } else
            {
                attackCounter = PlayerPrefs.GetInt(PlayerPrefsTags.AttackCounter, 0);
                life = PlayerPrefs.GetInt(PlayerPrefsTags.Life, 3);
            }
            UpdateLifeUIShow();
        }

        private void FixedUpdate() {
            // check if grounded
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            var position = transform.position;
            // check if on wall
            m_onWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, whatIsGround)
                      || Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, whatIsGround);
            m_onRightWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, whatIsGround);
            m_onLeftWall = Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, whatIsGround);

            // calculate player and wall sides as integers
            CalculateSides();

            if ((m_wallGrabbing || isGrounded) && m_wallJumping) {
                m_wallJumping = false;
            }
            // if this instance is currently playable
            if (isCurrentlyPlayable) {

                UpdateMovement(moveInput);  // horizontal movement
                UpdateDash();       // Dashing logic
                UpdateWallGrab();   // wall grab
                UpdateDustParticles();  // enable/disable dust particles
            }
        }

        private void Update() {
            attackCounterText.text = "" + attackCounter;

            jumpFlag = isGrounded;

            /////タップしたら
            if (Input.GetMouseButtonDown(0))    //画面に触れた瞬間に格納
                startTouchPos = Input.mousePosition;

            if (Input.GetMouseButton(0) == true) {
                //時間計測開始
                tapTimer += Time.deltaTime;

                /////double tap
                if (isDoubleTapStart) {
                    extraJampTimer += Time.deltaTime;

                    //ダブルタップ    まだ改善余地がある   ジャンプすると勝手に二段ジャンプになったりする
                    if (extraJampTimer < doubleTapTimer) {
                        if (Input.GetMouseButtonDown(0)) {
                            isDoubleTapStart = false;
                            doubleTapFlag = true;
                            extraJampTimer = 0.0f;
                        }
                    } else {
                        doubleTapFlag = false;
                        //reset
                        isDoubleTapStart = false;
                        extraJampTimer = 0.0f;
                    }
                } else {
                    if (Input.GetMouseButtonDown(0)) {
                        isDoubleTapStart = true;
                    }
                }
            }

            //画面から指を離したら
            if (Input.GetMouseButtonUp(0)) {
                oneJump = true;
                endTouchPos = Input.mousePosition;
                mag = (endTouchPos - startTouchPos);
                coordinateAngle = Mathf.Atan2(mag.y, mag.x);         //タッチしたところを原点として、そこからスワイプしてる角度を求める
                Mag = Mathf.Abs(Vector3.Distance(this.startTouchPos, this.endTouchPos));
                circumferentialAngle = Mathf.Atan2(mag.y, mag.x) * Mathf.Rad2Deg;    //角度を180のやつで求めることができる。(円周角度)
                // Debug.Log("角度は " + circumferentialAngle);
                // Debug.Log("mag.magnitudeでの距離は " + mag.magnitude);

                //フリック条件。時間経過は0.5以下でmagnitudeが10以上なら
                if (tapTimer <= flickGraceTimer && mag.magnitude >= flickGraceLength) {
                    flickFlag = true;
                }

                tapTimer = 0f;  //最後にタイマーを初期化
            }
            if (isGrounded) {
                m_extraJumps = extraJumpCount;
                m_extraDashs = extraDashCount;
            }

            // grounded remember offset (for more responsive jump)
            m_groundedRemember -= Time.deltaTime;
            if (isGrounded)
                m_groundedRemember = m_groundedRememberTime;

            if (!isCurrentlyPlayable) return;
            // if not currently dashing and hasn't already dashed in air once
            DashDecision();
            m_dashCooldown -= Time.deltaTime;
            // if has dashed in air once but now grounded
            if (m_hasDashedInAir && isGrounded)
                m_hasDashedInAir = false;

            if (updownInput <= 0.3f) oneJump = true;
            if (oneJump && updownInput >= 0.31f) 
                UpdateJumping();
            
        }

        public void OnClickAttackBtn()
        {
            Debug.Log("Player AttackBtn Down");
            if (attackCounter > 0)
            {
                gManager.PlaySE(magicBallSE);
                attackCounter--;
                // 弾（ゲームオブジェクト）の生成
                // TODO m_facingRight 元のキャラと左右逆の為、判定が逆になっている。
                if (!m_facingRight)
                {
                    Vector3 spawnPosition = transform.position + new Vector3(0.1f, 1.5f, 0f);
                    Instantiate(attackBallRigntObj, spawnPosition, Quaternion.identity);
                } else
                {
                    Vector3 spawnPosition = transform.position + new Vector3(-0.1f, 1.5f, 0f);
                    Instantiate(attackBallLeftObj, spawnPosition, Quaternion.identity);
                }
                animator.SetTrigger("Attack");
            }
        }

        //死んだ時に呼び出し関数
        void IsDaed() {
            PlayerPrefs.DeleteKey("Life");
            SceneHandler.LoadCurrentScene();
        }

        //ライフの表示の更新
        void UpdateLifeUIShow() {
            lifeHeartObj.ComparisonLife(life);
            Debug.Log("今のライフは " + life);
        }

        void LifeDamage(int number = 0) {
            if (invincible) {
                return;
            } else {
                gManager.PlaySE(damageSE);
            }
            life -= number;
            UpdateLifeUIShow();
            if (life > 0) {
                StartCoroutine(DamageTimer(25, 0.05f));
            }
            if (life <= 0) {
                IsDaed();
            }
        }

        //ダメージを受けた瞬間の無敵時間のタイマー
        IEnumerator DamageTimer(int num, float interval) {
            //既にダメージ状態なら終了
            if (invincible) {
                yield break;
            }
            invincible = true;
            //animator.SetTrigger("Damage");
            //無敵時間中の点滅
            for (int i = 0; i < num; i++) {
                childSprite.SetActive(false);
                yield return new WaitForSeconds(interval);
                childSprite.SetActive(true);
                yield return new WaitForSeconds(interval);
            }
            invincible = false;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            bool isRetryBoard = (other.gameObject.CompareTag(Tags.RetryBoard));
            bool isAttackBall = (other.gameObject.CompareTag(Tags.AttackBall));
            if (isRetryBoard) {
                IsDaed();
            }
            if (isAttackBall) {
                LifeDamage(1);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            bool isJumpGimmick = (collision.collider.CompareTag(Tags.JumpGimmick));
            bool isMoveFloor = (collision.collider.CompareTag(Tags.MoveFloor));
            bool isEnemy = (collision.collider.CompareTag(Tags.Enemy));
            bool isBossAttackBall = (collision.collider.CompareTag(Tags.BossAttackBall));
            if (isJumpGimmick) {
                Debug.Log("OnCollisionEnter2D: " + collision.gameObject.name);
                JumpGimmick jumpGimmick = collision.gameObject.GetComponent<JumpGimmick>();
                if (jumpGimmick != null) {
                    Debug.Log("jumpGimmick in");
                    jumpGimmick.isPlayerIn = true;
                    // TODO ジャンプ処理 関数化して共通にする。
                    m_rb.velocity = new Vector2(m_rb.velocity.x, m_extraJumpForce * 1.5f);
                    m_extraJumps--;
                    PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                    doubleTapFlag = false;
                } else {
                    Debug.Log("jumpGimmick is null");
                }
            }
            if (isMoveFloor) {
                transform.SetParent(collision.transform);
            }

            float stepPosion = groundCheck.position.y + (groundCheckRadius * 1.5f);    //プレイヤーの踏みつけ判定の位置と踏みつけ判定の高さ
            if (isEnemy) {
                foreach (ContactPoint2D p in collision.contacts) {
                    if (stepPosion > p.point.y) {     //対象物よりプレイヤーの踏みつけ判定が上だったら
                        Enemy ene = collision.gameObject.GetComponent<Enemy>();
                        if (ene != null) {
                            ene.playerStepOn = true;    //プレイヤーに踏まれた
                            m_rb.velocity = new Vector2(m_rb.velocity.x, m_extraJumpForce * 0.8f);
                        } else {
                            Debug.Log("Enemy is null");
                        }
                    } else {
                        LifeDamage(1);
                        break;
                    }
                }
            }
            if (isBossAttackBall) {
                LifeDamage(1);
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            bool isMoveFloor = (collision.collider.CompareTag(Tags.MoveFloor));
            if (isMoveFloor) {
                transform.SetParent(null);
            }
        }

        public void SetMoveInput(float input)
        {
            moveInput = input;
        }

        public void SetUpDownInput(float input)
        {
            updownInput = input;
        }

        // アイテムを取得した際に攻撃回数を増やすメソッド
        public void IncreaseAttackCounter(int amount)
        {
            gManager.PlaySE(itemSE);
            attackCounter += amount;
            Debug.Log("attackCounter: " + attackCounter);
        }

        // Jumping
        public void UpdateJumping(string button = "") {
            // Jumping
            /*if ((InputSystem.Jump() && m_extraJumps > 0 && !isGrounded && !m_wallGrabbing) ||
                    (Input.GetMouseButtonDown(0) && m_extraJumps > 0 && !isGrounded && !m_wallGrabbing)) {   // extra jumping
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_extraJumpForce);
                m_extraJumps--;
                // jumpEffect
                PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                doubleTapFlag = false;
            } else */
            if ((InputSystem.Jump() && (isGrounded || m_groundedRemember > 0f)) ||
          updownInput > 0.3f && jumpFlag && (isGrounded || m_groundedRemember > 0f) ||
          button.Equals("jbutton") && jumpFlag && (isGrounded || m_groundedRemember > 0f)) {  // normal single jumping
                if (jumpFlag) {
                    gManager.PlaySE(jumpSE);
                }
                jumpFlag = false;

                m_rb.velocity = new Vector2(m_rb.velocity.x, jumpForce);
                // jumpEffect
                PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
            } else if ((InputSystem.Jump() && m_wallGrabbing && moveInput != m_onWallSide) ||
                    (updownInput > 0.3f && m_wallGrabbing && moveInput != m_onWallSide)) {      // wall jumping off the wall
                m_wallGrabbing = false;
                m_wallJumping = true;
                Debug.Log("Wall jumped");
                if (m_playerSide == m_onWallSide)
                    Flip();
                m_rb.AddForce(new Vector2(-m_onWallSide * wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
            } else if ((InputSystem.Jump() && m_wallGrabbing && moveInput != 0 && (moveInput == m_onWallSide)) ||
                    (updownInput > 0.3f && m_wallGrabbing && moveInput != 0 && (moveInput == m_onWallSide))) {     // wall climbing jump
                m_wallGrabbing = false;
                m_wallJumping = true;
                Debug.Log("Wall climbed");
                if (m_playerSide == m_onWallSide)
                    Flip();
                m_rb.AddForce(new Vector2(-m_onWallSide * wallClimbForce.x, wallClimbForce.y), ForceMode2D.Impulse);
            }
            oneJump = false;
        }
        // movement
        private void UpdateMovement(float moveInput) {
            // horizontal movement
            if (m_wallJumping) {
                m_rb.velocity = Vector2.Lerp(m_rb.velocity, (new Vector2(moveInput * speed, m_rb.velocity.y)), 1.5f * Time.fixedDeltaTime);
            } else {
                if (canMove && !m_wallGrabbing)
                    m_rb.velocity = new Vector2(moveInput * speed, m_rb.velocity.y);
                else if (!canMove)
                    m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
            }
            // better jump physics
            if (m_rb.velocity.y < 0f) {
                m_rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            // Flipping
            if (m_facingRight && moveInput > 0f)
                Flip();
            else if (!m_facingRight && moveInput < 0f)
                Flip();
        }
        // Dashing
        private void DashDecision() {   //ダッシュの判定
            if (m_extraDashs > 0 && !isDashing /*&& !m_hasDashedInAir*/ && m_dashCooldown <= 0f) {
                if(IsDashInput())
                    StartDash();
            }
        }
        private bool IsDashInput() {
            return InputSystem.Dash() || flickFlag;
        }
        private void StartDash() {
            isDashing = true;
            // dash effect
            PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);
            if (isDashing) gManager.PlaySE(dashSE);
            // if player in air while dashing
            if (!isGrounded) {
                m_hasDashedInAir = true;
            }
            // dash logic is in FixedUpdate
            m_extraDashs--;
            UpdateDashMovement();
            m_dashCooldown = dashCooldown; // ダッシュ後のクールダウンを適用
            flickFlag = false;
        }
        private void UpdateDash() {
            // Dashing logic
            if (isDashing) {
                if (m_dashTime <= 0f) {
                    StopDash();
                } else {
                    m_dashTime -= Time.deltaTime;
                    UpdateDashMovement();
                }
            }
        }
        private void StopDash() {
            isDashing = false;
            //m_dashCooldown = dashCooldown;
            m_dashTime = startDashTime;
            m_rb.velocity = Vector2.zero;
            
        }
        private void UpdateDashMovement() {
            // エディタ、実機で処理を分ける
            if (Application.isEditor) {
                if (moveInput == 0f && updownInput != 0f) {
                    m_rb.velocity = new Vector2(0f, updownInput) * dashSpeed;
                } else {
                    if (!m_facingRight) {
                        m_rb.velocity = new Vector2(1f, updownInput) * dashSpeed;
                    } else {
                        m_rb.velocity = new Vector2(-1f, updownInput) * dashSpeed;
                    }
                }
            } else {
                if (!m_facingRight) {
                    if (117.5f >= circumferentialAngle && circumferentialAngle > 67.5f) {
                        m_rb.velocity = new Vector2(0f, 1f) * dashSpeed;
                    } else if (67.5f >= circumferentialAngle && circumferentialAngle > 22.5f) {
                        m_rb.velocity = new Vector2(0.65f, 0.7f) * dashSpeed;
                    } else if (22.5f >= circumferentialAngle && circumferentialAngle > -22.5f) {
                        m_rb.velocity = new Vector2(1f, 0f) * dashSpeed;
                    } else if (-22.5f >= circumferentialAngle && circumferentialAngle > -67.5f) {
                        m_rb.velocity = new Vector2(0.7f, -0.65f) * dashSpeed;
                    } else if (-67.5f >= circumferentialAngle && circumferentialAngle > -112.5f) {
                        m_rb.velocity = new Vector2(0f, -1f) * dashSpeed;
                    }
                    //m_rb.velocity = new Vector2(Mathf.Cos(coordinateAngle),Mathf.Sin(coordinateAngle)) * dashSpeed;
                } else {
                    if (62.5f <= circumferentialAngle && circumferentialAngle < 112.5f) {
                        m_rb.velocity = new Vector2(0f, 1f) * dashSpeed;
                    } else if (112.5f <= circumferentialAngle && circumferentialAngle < 157.5f) {
                        m_rb.velocity = new Vector2(-0.65f, 0.7f) * dashSpeed;
                    } else if (157.5f <= circumferentialAngle || circumferentialAngle < -157.5f) {
                        m_rb.velocity = new Vector2(-1f, 0f) * dashSpeed;
                    } else if (-157.5f <= circumferentialAngle && circumferentialAngle < -112.5f) {
                        m_rb.velocity = new Vector2(-0.7f, -0.65f) * dashSpeed;
                    } else if (-112.5f <= circumferentialAngle && circumferentialAngle < -67.5f) {
                        m_rb.velocity = new Vector2(0f, -1f) * dashSpeed;
                    }
                    //m_rb.velocity = new Vector2(Mathf.Cos(coordinateAngle),Mathf.Sin(coordinateAngle)) * dashSpeed;
                }
            }
            flickFlag = false;
        }
        // wall grab
        private void UpdateWallGrab() {     // wall grab
            if (m_onWall && !isGrounded && m_rb.velocity.y <= 0f && m_playerSide == m_onWallSide) {
                actuallyWallGrabbing = true;    // for animation
                m_wallGrabbing = true;
                m_rb.velocity = new Vector2(moveInput * speed, -slideSpeed);
                m_wallStick = m_wallStickTime;
            } else {
                m_wallStick -= Time.deltaTime;
                actuallyWallGrabbing = false;
                if (m_wallStick <= 0f)
                    m_wallGrabbing = false;
            }

            if (m_wallGrabbing && isGrounded)
                m_wallGrabbing = false;
        }
        // dust particles
        private void UpdateDustParticles() {    // enable/disable dust particles
            float playerVelocityMag = m_rb.velocity.sqrMagnitude;
            if (m_dustParticle.isPlaying && playerVelocityMag == 0f) {
                m_dustParticle.Stop();
            } else if (!m_dustParticle.isPlaying && playerVelocityMag > 0f) {
                m_dustParticle.Play();
            }
        }

        void Flip() {
            m_facingRight = !m_facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        void CalculateSides() {
            if (m_onRightWall)
                m_onWallSide = 1;
            else if (m_onLeftWall)
                m_onWallSide = -1;
            else
                m_onWallSide = 0;

            if (!m_facingRight)
                m_playerSide = 1;
            else
                m_playerSide = -1;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + grabRightOffset, grabCheckRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + grabLeftOffset, grabCheckRadius);
        }

    }
}
