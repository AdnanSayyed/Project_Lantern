using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerEquipment))]
public class NotepadManager : MonoBehaviour {

    [SerializeField]
    private List<GameObject> artifacts = new List<GameObject>();

    [SerializeField]
    private List<GameObject> collectibles = new List<GameObject>();

    [SerializeField]
    private List<GameObject> pages = new List<GameObject>();

    [SerializeField]
    private GameObject map;

    [SerializeField]
    private Sprite activeTab;

    [SerializeField]
    private Sprite deactiveTab;

    private PlayerEquipment equipmentManager;
    private PlayerController controller;

    private void Start()
    {
        GameObject _player = GameObject.Find("Player");

        if (_player == null) {
            Debug.LogError("No player found!");
            return;
        }

        equipmentManager = _player.GetComponent<PlayerEquipment>();
        controller = _player.GetComponent<PlayerController>();
    }

    public void EnableItem(string itemName) {

        foreach (GameObject item in artifacts)
        {
            item.GetComponent<Image>().sprite = deactiveTab;
        }

        foreach (GameObject item in pages)
        {
            item.SetActive(false);
        }

        switch (itemName) {

            case "mirror":
                artifacts[0].GetComponent<Image>().sprite = activeTab;
                EquipItem(itemName);
                pages[1].SetActive(true);
                break;

            case "lamp":
                artifacts[1].GetComponent<Image>().sprite = activeTab;
                EquipItem(itemName);
                pages[2].SetActive(true);
                break;

        }

    }

    private void EquipItem(string itemName) {

        equipmentManager.Equip(itemName);
        equipmentManager.ToggleInventoryUI(false);
        controller.SetUsingNotepad(false);

    }

    public void AddItem(string type, GameObject item) {

        switch (type) {

            case "artifact":
                artifacts.Add(item);
                item.SetActive(true);
                break;

        }

    }

    public void EnableBasePage()
    {
        foreach (GameObject item in pages)
        {
            item.SetActive(false);
        }

        pages[0].SetActive(true);
    }

    public void EnableArtifacts() {

        foreach (GameObject item in collectibles)
        {
            item.SetActive(false);
        }

        foreach (GameObject item in artifacts) {
            item.SetActive(true);
        }

    }

    public void EnableCollectibles()
    {

        foreach (GameObject item in artifacts)
        {
            item.SetActive(false);
        }

        foreach (GameObject item in collectibles)
        {
            item.SetActive(true);
        }

    }

}
