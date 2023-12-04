using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GManager : MonoBehaviour {
    #region
    public Text displayText_Jp;
    public Text displayText_Eng;
    public GameObject dialogue;
    bool isPause = false;
    [SerializeField] UIControllerTest uIControllerTest;
    string words_Jp;
    string words_Eng;
    private AudioSource audioSource = null;
    #endregion

    void Start() {
        uIControllerTest = dialogue.GetComponent<UIControllerTest>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && isPause) {
            ResumeGame();
        }
    }
    public void PauseGame(bool playerline = false) {
        displayText_Jp.gameObject.SetActive(true);
        displayText_Eng.gameObject.SetActive(true);
        displayText_Jp.enabled = true;
        displayText_Eng.enabled = true;
        displayText_Jp.text = words_Jp;
        displayText_Eng.text = words_Eng;

        isPause = true;
        dialogue.SetActive(true);
        uIControllerTest.Open(playerline);
        Time.timeScale = 0;
    }
    public void ResumeGame() {
        isPause = false;
        displayText_Jp.enabled = false;
        displayText_Eng.enabled = false;
        displayText_Jp.gameObject.SetActive(false);
        displayText_Eng.gameObject.SetActive(false);
        uIControllerTest.Close();
        Time.timeScale = 1;
    }
    public void ReceiveText(string jp, string eng) {
        words_Jp = jp;
        words_Eng = eng;
    }
    public void PlaySE(AudioClip clip) {
        if(audioSource != null) {
            audioSource.PlayOneShot(clip);
        } else {
            Debug.Log("Audio source is not set");
        }
    }
}
