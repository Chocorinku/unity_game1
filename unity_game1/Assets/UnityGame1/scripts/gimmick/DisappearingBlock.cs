using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingBlock : MonoBehaviour
{
    public float time;
    [SerializeField] private GameObject nextBlockObject;
    private Animator anim;
    GameObject child;

    void Start() {
        child = transform.GetChild(0).gameObject;
        anim = child.gameObject.GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        bool isPlayer = (collision.collider.CompareTag(Tags.Player));
        if (isPlayer) {
            nextBlockObject.SetActive(true);
            anim.SetBool("On", true);
            Invoke("Hidden", time);
        }
    }
    void Hidden() {
        anim.SetBool("On", false);
        this.gameObject.SetActive(false);
    }
}
