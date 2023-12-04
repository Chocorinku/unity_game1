using System.Collections;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    public GameObject ball;
    public float createTime = 1f;
    public float offsetX = 0f;
    public float offsetY = 0f;

    void Start()
    {
        StartCoroutine(BallCreate());
    }

    private IEnumerator BallCreate()
    {
        while (true)
        {
            yield return new WaitForSeconds(createTime);

            // 弾（ゲームオブジェクト）の生成
            Vector3 spawnPosition = transform.position + new Vector3(offsetX, offsetY, 0f);
            Instantiate(ball, spawnPosition, Quaternion.identity);
        }
    }

}
