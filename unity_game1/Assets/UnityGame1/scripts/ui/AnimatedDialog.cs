using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatedDialog : MonoBehaviour {
    #region
    [SerializeField] private Animator _animator;
    [SerializeField] private int _layer;    // �A�j���[�^�[�R���g���[���[�̃��C���[(�ʏ��0)
    private static readonly int ParamIsOpen = Animator.StringToHash("IsOpen");  // IsOpen�t���O(�A�j���[�^�[�R���g���[���[���Œ�`�����t���O)
    public bool isOpen => gameObject.activeSelf;    // �_�C�A���O�͊J���Ă��邩�ǂ���
    public bool isTransition { get; private set; }  // �A�j���[�V���������ǂ���
    #endregion

    public void DialogOpen() {
        if(isOpen || isTransition) return;  // �s������h�~

        gameObject.SetActive(true);     //�p�l�����̂��A�N�e�B�u�ɂ���

        _animator.SetBool(ParamIsOpen, true);   //isOpen�t���O���Z�b�g

        StartCoroutine(WaitAnimation("Shown")); // �A�j���[�V�����ҋ@
    }

    public void DialogClose() {
        if (!isOpen || isTransition) return;  // �s������h�~

        _animator.SetBool(ParamIsOpen, false);  // IsOpen�t���O���N���A

        StartCoroutine(WaitAnimation("Hidden", () => gameObject.SetActive(false))); // �A�j���[�V�����ҋ@���A�I�������p�l�����̂��A�N�e�B�u�ɂ���
    }
    
    //�J�A�j���[�V�����̑ҋ@�R���[�`��
    private IEnumerator WaitAnimation(string stateName, UnityAction onCompleted = null) {
        isTransition = true;

        yield return new WaitUntil(() => {
            //�X�e�[�g���ω����A�A�j���[�V�������I������܂Ń��[�v
            var state = _animator.GetCurrentAnimatorStateInfo(_layer);
            return state.IsName(stateName) && state.normalizedTime >= 1;
        });

        isTransition = false;

        onCompleted?.Invoke();
    }
}
