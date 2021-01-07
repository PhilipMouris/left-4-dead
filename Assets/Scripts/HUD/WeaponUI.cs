﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    // Start is called before the first frame update
    private Weapon weapon;
    private RawImage weaponImage;
    private TextMeshProUGUI currentAmmo;

    private TextMeshProUGUI totalAmmo;

    private Color green = new Color(0,255,0,255);

    private Color white = new Color(255,255,255,255);

    private int index;

    //public int test;

    private bool isSelected;
    public void Initialize(Weapon weapon, bool isSelected, int index) {
        this.weapon = weapon;
        Texture displayImage = Resources.Load($"{HUDConstants.ICON_PATH}/{weapon.GetType()}") as Texture;
        GameObject weaponImageObject = this.gameObject.transform.GetChild(0).gameObject;
        GameObject currentAmmoObject = this.gameObject.transform.GetChild(1).gameObject;
        GameObject totalAmmoObject = currentAmmoObject.transform.GetChild(0).gameObject;
        currentAmmo = currentAmmoObject.GetComponent<TextMeshProUGUI>();
        totalAmmo = totalAmmoObject.GetComponent<TextMeshProUGUI>();
        SetCurrentAmmo();
        SetTotalAmmo();
        weaponImage = weaponImageObject.GetComponent<RawImage>();
        weaponImage.texture = displayImage;
        //test = weapon.GetCurrentAmmo();
        this.isSelected = isSelected;
        this.index = index;

    }

    private void SetText(int amount, TextMeshProUGUI textObject ){
        if(amount<0){
            textObject.text = "";
            return;
        }
        if(amount < 9 ) {
            textObject.text = "  " + amount; 
            return;
        }
        if(amount < 100) textObject.text = " " + amount;
        else textObject.text = "" + amount;
    }
    private void SetCurrentAmmo(){
        int ammo = weapon.GetCurrentAmmo();
        SetText(ammo, currentAmmo);
      
    }

    private void SetTotalAmmo() {
        int ammo = weapon.GetTotalAmmo();
        SetText(ammo,totalAmmo);
    }

    private void SetColor() {
        if(isSelected) {
            weaponImage.color = green;
        }
        else {
            weaponImage.color = white;
        }
    }

    public void SetIsSelected(bool isSelected){
        this.isSelected = isSelected;

    }

    public Weapon GetWeapon(){
        return weapon;
    }

    public int GetIndex() {
        return index;
    } 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetColor();
        SetCurrentAmmo();
        SetTotalAmmo();
    }
}
