using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] GameObject LifeHeartIcon;

    [SerializeField] public int lifeHearChildNum;    //LifeHeartObjの子供の数

    public void ComparisonLife(int life) {
        lifeHearChildNum = this.transform.childCount;
        if (life <= 0 || lifeHearChildNum <= 0) {
            return;
        }
        if (lifeHearChildNum < life) {
            for (int i = 0; i < life - lifeHearChildNum; i++) {
                GameObject playerLifeObj = Instantiate(LifeHeartIcon,transform);
                playerLifeObj.transform.localScale = Vector3.one;
            }
        }else
        if (lifeHearChildNum > life) {
            for (int i = 0; i < lifeHearChildNum - life; i++) {
                
                Destroy(transform.GetChild(lifeHearChildNum - (1+i)).gameObject);
            }
        }
    }
}
