using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatedDialog : MonoBehaviour {
    #region
    [SerializeField] private Animator _animator;
    [SerializeField] private int _layer;    // アニメーターコントローラーのレイヤー(通常は0)
    private static readonly int ParamIsOpen = Animator.StringToHash("IsOpen");  // IsOpenフラグ(アニメーターコントローラー内で定義したフラグ)
    public bool isOpen => gameObject.activeSelf;    // ダイアログは開いているかどうか
    public bool isTransition { get; private set; }  // アニメーション中かどうか
    #endregion

    public void DialogOpen() {
        if(isOpen || isTransition) return;  // 不正操作防止

        gameObject.SetActive(true);     //パネル自体をアクティブにする

        _animator.SetBool(ParamIsOpen, true);   //isOpenフラグをセット

        StartCoroutine(WaitAnimation("Shown")); // アニメーション待機
    }

    public void DialogClose() {
        if (!isOpen || isTransition) return;  // 不正操作防止

        _animator.SetBool(ParamIsOpen, false);  // IsOpenフラグをクリア

        StartCoroutine(WaitAnimation("Hidden", () => gameObject.SetActive(false))); // アニメーション待機し、終わったらパネル自体を非アクティブにする
    }
    
    //開閉アニメーションの待機コルーチン
    private IEnumerator WaitAnimation(string stateName, UnityAction onCompleted = null) {
        isTransition = true;

        yield return new WaitUntil(() => {
            //ステートが変化し、アニメーションが終了するまでループ
            var state = _animator.GetCurrentAnimatorStateInfo(_layer);
            return state.IsName(stateName) && state.normalizedTime >= 1;
        });

        isTransition = false;

        onCompleted?.Invoke();
    }
}
