﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
 

    private Animator animator;

    private Animator pistolAnimator;

    private int HP;

    private Weapon[] weapons;

    private Companion companion;

    private Gernade[] gernades;

    private RageMeter rageMeter;

    private bool isWeaponDrawn;

    private GameObject pistol;

    private GameObject weaponCamera;

    private Vector3 rayCenter;

    private GameObject crossHair;

    private bool isEnemyInAimRange;

    private GameObject normalInfectantInRange;





    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag(NormalInfectantConstants.TAG)){
            other.gameObject.GetComponent<NormalInfectant>().Chase();
        }
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag(NormalInfectantConstants.TAG)){
            other.gameObject.GetComponent<NormalInfectant>().UnChase();
        }
    }
    
    
    void Awake()
    {  
        rayCenter = new Vector3(0.5F, 0.7F, 0);
        animator = GetComponent<Animator>();
        pistol = GameObject.Find(PlayerConstants.EQUIPPED);
        pistolAnimator = pistol.GetComponent<Animator>();
        crossHair = GameObject.Find(PlayerConstants.CROSS_HAIR);
        isWeaponDrawn = false;
        isEnemyInAimRange = false;
    }

    void FixedUpdate(){
        HandleRayCast();
      
    }
    
    
    void HandleRayCast() {
        if(!isWeaponDrawn) return;
        Ray ray = Camera.main.ViewportPointToRay(rayCenter);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, PlayerConstants.PISTOL_RANGE)) {
            GameObject collided = hit.collider.gameObject;
             if(collided.CompareTag(NormalInfectantConstants.TAG)){
                   if(!isEnemyInAimRange) {
                        SetCrossHairGreen();
                        isEnemyInAimRange = true;
                        normalInfectantInRange = collided;
                   }
             }
             else {
                 if(isEnemyInAimRange) {
                    SetCrossHairRed();
                    isEnemyInAimRange = false;
                    normalInfectantInRange = null;
                 }
             }
            // Debugging purposes only
            // Debug.DrawLine(ray.origin, hit.point);
          
        }
        else {
              if(isEnemyInAimRange) {
                    SetCrossHairRed();
                    isEnemyInAimRange = false;
                     normalInfectantInRange = null;
                 }

        }

    }
    
     void HandleDrawWeapon(){
        if(Input.GetButtonDown(PlayerConstants.DRAW_WEAPON_INPUT)){
            this.isWeaponDrawn = !isWeaponDrawn;
            animator.SetBool(PlayerConstants.DRAW_PISTOL, isWeaponDrawn);
        }
        
    }    

    void HandleFire(){
        if(Input.GetButtonDown("Fire1") && isWeaponDrawn){
            animator.SetTrigger(PlayerConstants.SHOOT);
            pistolAnimator.SetTrigger(PlayerConstants.FIRE);
            if(normalInfectantInRange){
                Debug.Log("HEREEEEE");
                normalInfectantInRange.GetComponent<NormalInfectant>().GetShot(1000);
            }
        }
    }
    
    void SetCrossHairGreen(){
        SpriteRenderer sprite = crossHair.GetComponent<SpriteRenderer>();
        sprite.color = new Color (0, 255, 0, 1); 
    }

    void SetCrossHairRed() {
         SpriteRenderer sprite = crossHair.GetComponent<SpriteRenderer>();
         sprite.color = new Color (255, 0, 0, 1); 
    }


    // Update is called once per frame
    void Update()
    {   

        HandleDrawWeapon();
        HandleFire();
        
    }


    public void ResetHealth(){
        
    }
}
