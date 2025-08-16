using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Minimap : UI_Scene
{
    [SerializeField] GameObject minimapObj;

    public void OnMinimap()
    {
        minimapObj.SetActive(!minimapObj.activeSelf);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            minimapObj.SetActive(!minimapObj.activeSelf);
        }
    }
}
