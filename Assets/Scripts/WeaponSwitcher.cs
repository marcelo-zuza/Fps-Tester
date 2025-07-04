using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private List<GameObject> ownedWeapons = new List<GameObject>();
    private int currentWeaponIndex = 0;

    void Start()
    {
        // if (ownedWeapons > 0)
        // {

        // }

    }

    // Update is called once per frame
    void Update()
    {
        if (ownedWeapons.Count <= 1) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % ownedWeapons.Count;
            SelectWeapon(currentWeaponIndex);
            // Debug.Log("Current weapon index: " + currentWeaponIndex);
        }
    }

    void SelectWeapon(int index)
    {
        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            ownedWeapons[i].SetActive(i == index);
        }
    }

    public void AddWeapon(GameObject newWeapon)
    {
        if (!ownedWeapons.Contains(newWeapon))
        {
            ownedWeapons.Add(newWeapon);
            newWeapon.SetActive(false);

        }
    }
}
