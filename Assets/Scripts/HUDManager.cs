using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance { get; set; }

    public GameObject HUDCanvas;
    public GameObject weaponPanel;

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unactiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;
    public Sprite greySlot;

    private Sprite M4A1_Weapon, FN57_Weapon, Baretta_Weapon,
        PistolAmmo,
        RifleAmmo, GrenadeSprite, SmokeSprite, FlashbangSprite;

    public GameObject crosshair;

    [Header("Effects")]
    public GameObject flashbangEffect;
    [SerializeField] private Image afterEffectImg;
    private Animator flashbangAnimator;

    [Header("PauseUtils")]
    public GameObject pauseMenu;

    [Header("Game Over")]
    public GameObject deathScreen;
    public GameObject winScreen;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Start()
    {
        M4A1_Weapon = Resources.Load<GameObject>("M4A1_Weapon").GetComponent<SpriteRenderer>().sprite;
        FN57_Weapon = Resources.Load<GameObject>("FN57_Weapon").GetComponent<SpriteRenderer>().sprite;
        Baretta_Weapon = Resources.Load<GameObject>("Baretta_Weapon").GetComponent<SpriteRenderer>().sprite;
        PistolAmmo = Resources.Load<GameObject>("Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;
        RifleAmmo = Resources.Load<GameObject>("Rifle_Ammo").GetComponent<SpriteRenderer>().sprite;
        GrenadeSprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
        SmokeSprite = Resources.Load<GameObject>("Smoke").GetComponent<SpriteRenderer>().sprite;
        FlashbangSprite = Resources.Load<GameObject>("Flashbang").GetComponent<SpriteRenderer>().sprite;

        flashbangAnimator = flashbangEffect.GetComponent<Animator>();

        UpdateThrowables();
    }

    // Update is called once per frame
    void Update()
    {
        Weapon activeWeapon = WeaponManager.instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnactiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft}";
            totalAmmoUI.text = $"{WeaponManager.instance.CheckAmmoLeft(activeWeapon.model)}";

            Weapon.WeaponModel model = activeWeapon.model;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon)
            {
                unactiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.model);
            }
        } else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;

            activeWeaponUI.sprite = emptySlot;
            unactiveWeaponUI.sprite = emptySlot;
        }

        if (WeaponManager.instance.lethalsCount  <= 0)
        {
            lethalUI.sprite = greySlot;
            lethalAmountUI.text = "";
        }

        if (WeaponManager.instance.tacticalsCount <= 0)
        {
            tacticalUI.sprite = greySlot;
            tacticalAmountUI.text = "";
        }
    }

    public void SetFlashbangUI()
    {
        StartCoroutine(GoBlind());
    }

    public IEnumerator GoBlind()
    {
        yield return new WaitForEndOfFrame();

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        afterEffectImg.sprite = Sprite.Create(texture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f), 100);
        flashbangAnimator.SetTrigger("goBlind");
    }
    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.M4A1:
                return M4A1_Weapon;
            case Weapon.WeaponModel.FN57:
                return FN57_Weapon;
            case Weapon.WeaponModel.Baretta:
                return Baretta_Weapon;
            default:
                return null;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.M4A1:
                return RifleAmmo;
            case Weapon.WeaponModel.FN57:
                return PistolAmmo;
            case Weapon.WeaponModel.Baretta:
                return PistolAmmo;
            default:
                return null;
        }
    }

    private GameObject GetUnactiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.instance.activeWeaponSlot)
            {   
                return weaponSlot;
            }
        }
        return null;
    }

    internal void UpdateThrowables()
    {
        lethalAmountUI.text = $"{WeaponManager.instance.lethalsCount}";
        tacticalAmountUI.text = $"{WeaponManager.instance.tacticalsCount}";
        switch (WeaponManager.instance.equippedLethalType)
        {
            case Grenade.Throwable.Grenade:
                lethalUI.sprite = GrenadeSprite;
                break;
            case Grenade.Throwable.None:
                lethalUI.sprite = greySlot;
                break;
            default:
                break;
        }
        switch (WeaponManager.instance.equippedTacticalType)
        {
            case Grenade.Throwable.Smoke:
                tacticalUI.sprite = SmokeSprite;
                break;
            case Grenade.Throwable.Flashbang:
                tacticalUI.sprite = FlashbangSprite;
                break;
            case Grenade.Throwable.None:
                tacticalUI.sprite = greySlot;
                break;
            default:
                break;
        }
    }

    public void togglePause ()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        crosshair.SetActive(!crosshair.activeSelf);
    }

    internal void OnPlayerDeath()
    {
        GlobalRef.instance.blurEffect.SetActive(true);
        weaponPanel.SetActive(false);
        crosshair.SetActive(false);
        deathScreen.SetActive(true);
        StartCoroutine(RemoveDeathEffects());
    }

    private IEnumerator RemoveDeathEffects()
    {
        yield return new WaitForSeconds(GlobalRef.instance.respawnTime);
        GlobalRef.instance.blurEffect.SetActive(false);
        deathScreen.SetActive(false);
        weaponPanel.SetActive(true);
        crosshair.SetActive(true);
    }

    internal void OnPlayerWin()
    {
        GlobalRef.instance.blurEffect.SetActive(true);
        weaponPanel.SetActive(false);
        crosshair.SetActive(false);
        winScreen.SetActive(true);
        StartCoroutine(RemoveWinEffects());
    }

    private IEnumerator RemoveWinEffects()
    {
        yield return new WaitForSeconds(GlobalRef.instance.respawnTime);
        GlobalRef.instance.blurEffect.SetActive(false);
        winScreen.SetActive(false);
        weaponPanel.SetActive(true);
        crosshair.SetActive(true);
    }
}
