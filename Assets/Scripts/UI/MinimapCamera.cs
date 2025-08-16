using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0, 20f, 0); // 위에서 내려다보는 고정 위치

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}

