using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerTest : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Animator playerImageAni;
    [SerializeField] GameObject playerImg;
    bool monologue;     //âÔòbÅAì∆ÇËåæ

    public void Open(bool isplayerline) {
        monologue = isplayerline; 
        animator.SetBool("IsOpen", true);
        
    }
    public void Close() {
        animator.SetBool("IsOpen", false);
    }
    public void OnPlayerImg() {
        if (monologue) {
            playerImg.SetActive(true);
            playerImageAni.SetBool("ImageMove", true);
        }
    }
    public void OffPlayerImg() {
        if (monologue) {
            playerImageAni.SetBool("ImageMove", false);
            playerImg.SetActive(false);
            monologue = false;
        }
    }
}
