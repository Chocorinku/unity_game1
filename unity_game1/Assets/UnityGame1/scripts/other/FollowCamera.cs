using SupanthaPaul;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    GameObject playerObj;
    PlayerController player;
    Transform playerTransform;

    [SerializeField]
    public Vector3 offset;
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag(Tags.Player);
        player = playerObj.GetComponent<PlayerController>();
        playerTransform = playerObj.transform;
    }
    void LateUpdate()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        transform.position = new Vector3(playerTransform.position.x + offset.x, playerTransform.position.y + offset.y, transform.position.z);
    }
}
