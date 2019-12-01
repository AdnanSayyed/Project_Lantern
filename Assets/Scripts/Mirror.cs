using UnityEngine;

public class Mirror : MonoBehaviour {

    [SerializeField]
    private GameObject mirrorLight;

    public void TurnOn() {
        mirrorLight.SetActive(true);
    }

    public void TurnOff()
    {
        mirrorLight.SetActive(false);
    }

}
