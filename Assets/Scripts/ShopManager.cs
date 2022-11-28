using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TMP_Text coinUI;
    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseButtons;
    public GameObject[] weapons;
    public PlayerController player;
    public ShowSlash showSlash;
    public bool purchasedItem = false;
    public int currentWeaponIndex;

    void Start()
    {
        for(int i=0; i<shopItemsSO.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        coinUI.text = "Coins: " + GameManager.Instance.coins.ToString();
        LoadPanels();
        CheckPurchasable();
    }

    public void ShowShop()
    {
        if(gameObject.activeInHierarchy == true)
        {
            gameObject.SetActive(false);
        } else if(gameObject.activeInHierarchy == false) {
            gameObject.SetActive(true);
        }
    }
    public void PurchaseItem(int btnNumber)
    {
        if (btnNumber == currentWeaponIndex)
        {
            return;
        } 
        else if (shopItemsSO[btnNumber].purchased == true) 
        {
            shopPanels[currentWeaponIndex].costText.text = "Equip";
            currentWeaponIndex = btnNumber;
            shopPanels[btnNumber].costText.text = "Equipped";

            GameObject destroyedObject = GameObject.FindGameObjectWithTag("Sword");
            Destroy(destroyedObject);
            GameObject currentSword = Instantiate(weapons[btnNumber]);
            player.swordCollider = currentSword.GetComponent<Collider>();
            showSlash.slash = currentSword.transform.GetChild(0).gameObject;
            CheckPurchasable();
        } 
        else if (GameManager.Instance.coins >= shopItemsSO[btnNumber].baseCost)
        {
            shopPanels[currentWeaponIndex].costText.text = "Equip";
            GameManager.Instance.coins -= shopItemsSO[btnNumber].baseCost;
            currentWeaponIndex = btnNumber;
            coinUI.text = "Coins: " + GameManager.Instance.coins.ToString();
            shopItemsSO[btnNumber].purchased = true;
            shopPanels[btnNumber].costText.text = "Equipped";

            GameObject destroyedObject = GameObject.FindGameObjectWithTag("Sword");
            Destroy(destroyedObject);
            GameObject currentSword = Instantiate(weapons[btnNumber]);
            player.swordCollider = currentSword.GetComponent<Collider>();
            showSlash.slash = currentSword.transform.GetChild(0).gameObject;
            CheckPurchasable();
        }
    }

    public void LoadPanels()
    {
        for(int i=0; i<shopItemsSO.Length; i++)
        {
            shopPanels[i].titleText.text = shopItemsSO[i].title;
            shopPanels[i].image.sprite = shopItemsSO[i].image;
            shopPanels[i].descriptionText.text = shopItemsSO[i].description;
            shopPanels[i].costText.text = shopItemsSO[i].baseCost.ToString() + " coins";
            shopItemsSO[i].purchased = false;
        }
    }

    public void CheckPurchasable()
    {
        for(int i=0; i<shopItemsSO.Length; i++)
        {
            if (shopItemsSO[i].purchased == true)
            {
                myPurchaseButtons[i].interactable = true;
            } 
            else if (GameManager.Instance.coins >= shopItemsSO[i].baseCost)
            {
                myPurchaseButtons[i].interactable = true;
            }
            else
            {
                myPurchaseButtons[i].interactable = false;
            }
        }
    }
}
