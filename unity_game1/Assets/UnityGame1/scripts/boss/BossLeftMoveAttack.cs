using UnityEngine;

public class BossLeftMoveAttack : MonoBehaviour
{
    public float speed = 10f;
    public float deleteTime = 6f;

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }

    void Update()
    {
        if (gameObject != null)
        {
            Vector3 lazerPos = transform.position;
            lazerPos.x -= speed * Time.deltaTime;
            transform.position = lazerPos;
        }
    }
}
