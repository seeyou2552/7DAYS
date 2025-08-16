using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerCon : MonoBehaviour
{
    public bool canResource = false;
    Resource resource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && canResource)
        {
            StartCoroutine(resource.GetResource(() => { }));
            canResource = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            canResource = true;
            resource = other.GetComponent<Resource>();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            canResource = false;
        }
    }
}
