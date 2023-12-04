using UnityEngine;

public class JumpGimmick : MonoBehaviour
{
    [HideInInspector] public bool isPlayerIn = false;
    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (isPlayerIn)
        {
            Debug.Log("JumpGimmick.isPlayerIn: true");
            animator.SetTrigger("on");
            isPlayerIn = false;
        }
    }
}
