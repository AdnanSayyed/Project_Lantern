using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorLight : MonoBehaviour {

    [SerializeField]
    string LampName = "LampPickUp";

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == LampName)
        {
            if(other.gameObject.GetComponent<Lamp>().isDisabled)
                other.gameObject.GetComponent<Lamp>().EnableOutline();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == LampName)
        {
            if(!other.gameObject.GetComponent<Lamp>().isDisabled)
                other.gameObject.GetComponent<Lamp>().DisableOutline();
        }
    }

}
