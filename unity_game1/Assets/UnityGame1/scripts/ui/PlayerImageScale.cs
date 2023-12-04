using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImageScale : MonoBehaviour {
    public Vector3 defaultScale;// = Vector3.zero;

    void Start() {
        defaultScale = transform.lossyScale;
    }

    void Update() {
        Vector3 parentLossyScale = transform.lossyScale;
        Vector3 localScale = transform.localScale;

        transform.localScale = new Vector3(
                localScale.x / parentLossyScale.x * defaultScale.x,
                localScale.y / parentLossyScale.y * defaultScale.y,
                localScale.z / parentLossyScale.z * defaultScale.z
        );
    }
}
