using SupanthaPaul;
using UnityEngine;

public class ShowLog : MonoBehaviour {

    [SerializeField]
    string signLanguage_jp = "ここにセリフ";
    [SerializeField]
    string signLanguage_eng = "adkfjeofeeff";
    GManager gManager;
    [SerializeField] bool isPlayerLine;     //プレイヤーが会話するかどうか

    private void Start() {
        gManager = GameObject.Find("_GameManger").GetComponent<GManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        bool isPlayer = (other.gameObject.CompareTag(Tags.Player));
        if (isPlayer) {
            gManager.ReceiveText(signLanguage_jp, signLanguage_eng);
            gManager.PauseGame(isPlayerLine);
        }
        if(isPlayerLine) Destroy(this.gameObject);
    }
}
