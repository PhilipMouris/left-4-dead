﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;


public class GameManager : MonoBehaviour
{

    public GameObject CraftingScreen;

    public GameObject HUD;
    
    private GameObject pauseScreen;

    private SoundManager soundManager;

    private bool isPaused;

    private Level1Manager level1Manager;

    private Level2Manager level2Manager;

    private Level3Manager level3Manager;

    private WeaponsManager weaponsManager;

    private GernadeManager gernadeManager;

    private Player player;

    private int level;

    private HUDManager hudManager;

    private Camera FPS;

    private Camera craftingCamera;

    private Camera TPS;

    private float throwingPower = 3;

    public static bool crafting_bool;

    private bool isRaged ;

    private Companion companion;

    private int normalRageIncrease = 10;

    private int specialRageIncrease= 50;
    private bool isDoubleIngredients;



    // Start is called before the first frame update
    void Awake()
    {
        crafting_bool = false;
        FPS = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        craftingCamera = GameObject.Find("CraftingCamera").GetComponent<Camera>();
        TPS = GameObject.Find("ThirdPersonCamera").GetComponent<Camera>();
        FPS.enabled = true;
        TPS.enabled = true;
        craftingCamera.enabled = false;
        //   Debug.Log(FPS.enabled + " FPS");
        //   De
    }

    void InitializeWeapon(string type, bool isSelected)
    {
        Weapon weapon = weaponsManager.InitializeWeapon(type);
        if (isSelected)
            player.SetWeapon(weapon);
        hudManager.AddWeapon(weapon, isSelected);
    }
    void InitializeGernades()
    {
        hudManager.AddAllGernades();
    }


    private void InitializeLevelManagers()
    {
        level1Manager = ScriptableObject.CreateInstance("Level1Manager") as Level1Manager;
        level2Manager = ScriptableObject.CreateInstance("Level2Manager") as Level2Manager;
        level3Manager = ScriptableObject.CreateInstance("Level3Manager") as Level3Manager;
    }


    private void InitializeScene()
    {
        switch (level)
        {
            case 1:
                level1Manager.Initialize();
                break;
            case 2:
            //level2Manager.Initialize();
            default:
                break;
                //level3Manager.Initialize();

        }
    }

    private void HandleSwitchGrenades()
    {
        if (Input.GetButtonDown(PlayerConstants.SWITCH_GRENADE))
        {
            // hudManager.SetHealth(50);
            Debug.Log("Switching");
            Gernade gernade = hudManager.SwitchGrenade();
            player.SetGrenade(gernade);

        }
    }
    public void ResetGrenadeInfo()
    {
        throwingPower = 3f;

    }
    private void HandleThrowGrenade()
    {
        if (!hudManager.CheckAllEmptyGrenades())
        {
            if (Input.GetMouseButton(1))
            {  if (throwingPower < PlayerConstants.THROWING_POWER_MAX)
                {   
                    throwingPower += 0.2f;
                    hudManager.ChangePowerBar(Convert.ToInt32((0.2/7f) * 100));
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                // Debug.Log(gernades.Count + " COUNT?????");
                hudManager.ChangePowerBar(-100);
                player.ThrowGrenade(throwingPower);
                hudManager.RemoveCurrentGernade();
                this.ResetGrenadeInfo();
            }
        }
        else
        {
            
        }
    }


    private void HandleSwitchWeapons()
    {
        if (Input.GetButtonDown(PlayerConstants.DRAW_WEAPON_INPUT))
        {
            // hudManager.SetHealth(50);
            if (!player.GetIsweaponDrawn())
            {
                player.HandleDrawWeapon();
            }
            else
            {
                Weapon weapon = hudManager.SwitchWeapon();
                player.SetWeapon(weapon);
            }
        }
    }


    private void HandleCraftingScreen()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {   
            this.gameObject.GetComponent<Craft>().SetIsDoubleIngredients(isDoubleIngredients);
            crafting_bool = !crafting_bool;
            FPS.enabled = false;
            TPS.enabled = false;
            craftingCamera.enabled = true;
            CraftingScreen.SetActive(crafting_bool);
            GameObject.Find("FPSController").GetComponent<FirstPersonController>().isCrafting = crafting_bool;

            HandlePause();
        }
    }


    private void HandlePickUpWeapon() {
        if(Input.GetKeyDown(KeyCode.E)){
            Weapon weapon = player.GetWeaponInRange();
            if(!weapon) return;
            Weapon oldWeapon = weaponsManager.GetWeapon(weapon.GetType());
            if(!oldWeapon) {
                InitializeWeapon(weapon.GetType(),false);
                weaponsManager.PickUp(weapon,false);
            }
            else {
                oldWeapon.Reset();
               weaponsManager.PickUp(weapon, true);
            }
         
        }
    }

    public void EnemyDead(string type) {
        if(type=="normal"){
            hudManager.ChangeRage(normalRageIncrease);
        }
        else hudManager.ChangeRage(specialRageIncrease);
    }

    private void HandleActivateRage() {
        if(isRaged) return;
        if(Input.GetKeyDown(KeyCode.F)) hudManager.ActivateRage();
    }

    public void SetRage(bool rage) {
        this.isRaged = rage;
    }

    public bool GetIsRaged() {
        return isRaged;
    }


    



    void Update()
    {
        HandleCraftingScreen();
        HandleSwitchWeapons();
        HandleSwitchGrenades();
        HandleThrowGrenade();
        HandlePickUpWeapon();
        HandleActivateRage();
        //HandleCompanionShoot();

        // if(Input.GetKeyDown(KeyCode.H)){
        //     hudManager.ChangeRage(+30);
        // }
    }

    private void HandlePause()
    {
        this.isPaused = !this.isPaused;
        //soundManager.HandlePause(this.isPaused);
        if (isPaused)
        {
            Time.timeScale = 0;
            //pauseScreen.SetActive(true);
            HUD.SetActive(false);
            return;
        }
        Time.timeScale = 1;
        //pauseScreen.SetActive(false);
        HUD.SetActive(true);
    }



    private void InitializeCompanion(string type) {
        GameObject companionLoad = Resources.Load(CompanionConstants.COMPANION_PATHS[type]) as GameObject;
        (Vector3,Vector3) transformations = CompanionConstants.COMPANION_TRANSFORMATION[type];
        GameObject companionInstance =  Instantiate(companionLoad,transformations.Item1, Quaternion.identity);
        companionInstance.transform.localRotation = Quaternion.Euler(transformations.Item2);
        companion = companionInstance.AddComponent<Companion>();
        Weapon companionWeapon = GameObject.Find("WeaponEQCompanion").transform.GetChild(0).gameObject.AddComponent<Weapon>();
        GameObject companionMuzzle = GameObject.Find("CompanionMuzzle");
        if(companionMuzzle){
            companionWeapon.SetMuzzle(companionMuzzle);
            companionMuzzle.SetActive(false);
        }
        companionWeapon.InitializeCompanionWeapon(CompanionConstants.COMPANION_WEAPONS[type]);
        //INITIALIZE
        companion.Initialize(companionWeapon,type);
        hudManager.InitializeCompanion(type,companionWeapon);

        if(type == "louis") {
                InvokeRepeating("IncreaseHealthBy1", 1, 1);
        }
        if(type== "ellie") {
            normalRageIncrease = 2*normalRageIncrease;
            specialRageIncrease = 2*normalRageIncrease;
        }
        if(type=="zoey") isDoubleIngredients = true;
    }

    private void IncreaseHealthBy1(){
        SetHealth(1);
    }
       
    public int AddEnemyToCompanion(NormalInfectant normal,int id) {
        return companion.AddEnemy(normal, id);

    }

    public void RemoveNormalFromCompanion(int id) {
        companion.RemoveEnemy("normal",id);
    }



    void Start()
    {

        player = GameObject.Find(EngineConstants.PLAYER).GetComponent<Player>();
        hudManager = GameObject.Find(EngineConstants.HUD).GetComponent<HUDManager>();
        weaponsManager = GameObject.Find(EngineConstants.WEAPONS_MANAGER).GetComponent<WeaponsManager>();
        InitializeCompanion("louis");
        SetHealth(-50);
        //level = 1;
        //isPaused = false;
        //pauseScreen = GameObject.Find(EngineConstants.PAUSE);
        //SetButtonListeners();
        //pauseScreen.SetActive(false);
        //this.soundManager = GameObject.Find(MenuConstants.AUDIO_MANAGER).GetComponent<SoundManager>();
        //InitializeLevelManagers();
        //InitializeScene();
        //InitializePistol();
      
        hudManager.SetPlayer(player);

        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["PISTOL"], true);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["ASSAULT_RIFLE"], false);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["SMG"], false);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["HUNTING_RIFLE"], false);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["SHOTGUN"], false);

        InitializeGernades();
        // InitializeGernade(WeaponsConstants.WEAPON_TYPES["MOLOTOV_COCKTAIL"],false);
        // InitializeGernade(WeaponsConstants.WEAPON_TYPES["PIPE_BOMB"],false);
        // InitializeGernade(WeaponsConstants.WEAPON_TYPES["STUN_BOMB"],false);


        InitializeWeapon(WeaponsConstants.WEAPON_TYPES["PISTOL"], true);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["ASSAULT_RIFLE"],false);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["SMG"],false);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["HUNTING_RIFLE"],false);
        // InitializeWeapon(WeaponsConstants.WEAPON_TYPES["SHOTGUN"],false);
        

    }

    private void onQuit()
    {
        soundManager.PlayButtonClick();
        SceneManager.LoadScene(MenuConstants.MENU_SCENE);
    }

    private void onResume()
    {
        this.soundManager.PlayButtonClick();
        HandlePause();
    }

    private void SetButtonListeners()
    {
        // GameObject[] restartButtons = GameObject.FindGameObjectsWithTag(Constants.RESTART_BUTTON);
        GameObject[] quitButtons = GameObject.FindGameObjectsWithTag(Constants.QUIT_BUTTON);

        for (int i = 0; i < quitButtons.Length; i++)
        {

            quitButtons[i].GetComponent<Button>().onClick.AddListener(onQuit);
        }

        GameObject.Find(EngineConstants.RESUME).GetComponent<Button>().onClick.AddListener(onResume);

    }


    // Update is called once per frame

    public void SetHealth(int health)
    {
        hudManager.ChangeHealth(health);
    }

    public int GetHealth()
    {
        return hudManager.GetHealth();
    }

}
