using SupanthaPaul;
using UnityEngine;

public class SceneChangeOnTrigger : MonoBehaviour
{
    public string sceneToLoad; // 移動先のシーン名

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Player))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                PlayerPrefs.SetInt(PlayerPrefsTags.AttackCounter, playerController.attackCounter);
                PlayerPrefs.SetInt(PlayerPrefsTags.Life, playerController.life);
            }

            if (string.IsNullOrEmpty(sceneToLoad)) {
                SceneHandler.LoadNextScene();
            } else {
                SceneHandler.LoadScene(sceneToLoad);
            }
        }
    }
}
