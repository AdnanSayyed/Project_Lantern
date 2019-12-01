using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour {

    [SerializeField]
    GameObject lampMesh;

    [SerializeField]
    float outlineLifeTime = 5f;

    Outline outline;

    public bool isDisabled = true;

    void Start()
    {
        outline = lampMesh.GetComponent<Outline>();
    }

    public void EnableOutline()
    {
        if (isDisabled)
        {
            outline.enabled = true;
            isDisabled = false;
        }

    }

    public void DisableOutline()
    {
        if (!isDisabled)
        {
            Invoke("Disable", outlineLifeTime);
            isDisabled = true;
        }
    }

    private void Disable()
    {
        if (isDisabled)
        {
            outline.enabled = false;
        }
    }


}
