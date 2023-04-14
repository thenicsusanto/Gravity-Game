using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseButtons;
    public GameObject[] weapons;
    public PlayerController player;
    public ShowSlash showSlash;
    public bool purchasedItem = false;
    public int currentWeaponIndex;
    public SwordCollider swordCollider;
    public GameObject abilityButton;
    public GameObject swordsMenu, swordsButton;
    public GameObject itemsMenu, itemsButton;
    public GameObject swordIcon, itemsIcon;
    public TextMeshProUGUI coinText;

    void Start()
    {
        for(int i=0; i<shopItemsSO.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        //first load panels resets everything back to default
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanels[i].titleText.text = shopItemsSO[i].title;
            shopPanels[i].image.sprite = shopItemsSO[i].image;
            shopPanels[i].descriptionText.text = shopItemsSO[i].description;
            shopPanels[i].costText.text = shopItemsSO[i].baseCost.ToString() + " coins";
            shopItemsSO[i].purchased = false;
        }
        CheckPurchasable();
        shopPanels[0].costText.text = "Equipped";
        shopItemsSO[0].purchased = true;
    }

    void OnEnable()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        //swordCollider.damageToTake = shopItemsSO[currentWeaponIndex].damage;
        LoadPanelSword();
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
    public void PurchaseItemSword(int btnNumber)
    {
        if (btnNumber == currentWeaponIndex)
        {
            //swordCollider.damageToTake = shopItemsSO[currentWeaponIndex].damage;
            return;
        }
        else if (shopItemsSO[btnNumber].purchased == true) 
        {
            shopPanels[currentWeaponIndex].costText.text = "Equip";
            currentWeaponIndex = btnNumber;
            player.playerWeaponIndex = btnNumber;
            shopPanels[btnNumber].costText.text = "Equipped";

            GameObject destroyedObject = GameObject.FindGameObjectWithTag("Sword");
            Destroy(destroyedObject);
            GameObject currentSword = Instantiate(weapons[btnNumber]);
            swordCollider = currentSword.GetComponentInChildren<SwordCollider>();
            player.swordCollider = currentSword.GetComponent<Collider>();
            showSlash.slash = currentSword.transform.GetChild(0).gameObject;
            showSlash.swordCollider = currentSword.GetComponent<SwordCollider>();
            showSlash.sword = currentSword.transform;

            //checks if sword has ability
            if(btnNumber == 0 || btnNumber == 1 || btnNumber == 2 || btnNumber == 3)
            {
                abilityButton.SetActive(false);
            }
            else
            {
                abilityButton.SetActive(true);
            }

            CheckPurchasable();
        } 
        else if (GameManager.Instance.coins >= shopItemsSO[btnNumber].baseCost)
        {
            FindObjectOfType<AudioManager>().Play("UnlockWeapon");
            shopPanels[currentWeaponIndex].costText.text = "Equip";
            GameManager.Instance.coins -= shopItemsSO[btnNumber].baseCost;
            currentWeaponIndex = btnNumber;
            player.playerWeaponIndex = btnNumber;
            shopItemsSO[btnNumber].purchased = true;
            shopPanels[btnNumber].costText.text = "Equipped";

            GameObject destroyedObject = GameObject.FindGameObjectWithTag("Sword");
            Destroy(destroyedObject);
            GameObject currentSword = Instantiate(weapons[btnNumber]);
            swordCollider = currentSword.GetComponentInChildren<SwordCollider>();
            player.swordCollider = currentSword.GetComponent<Collider>();
            showSlash.slash = currentSword.transform.GetChild(0).gameObject;
            showSlash.swordCollider = currentSword.GetComponent<SwordCollider>();
            showSlash.sword = currentSword.transform;
            coinText.text = GameManager.Instance.coins.ToString();

            if (btnNumber == 0 || btnNumber == 1 || btnNumber == 2 || btnNumber == 3)
            {
                abilityButton.SetActive(false);
            }
            else
            {
                abilityButton.SetActive(true);
            }

            CheckPurchasable();
        }
    }

    public void PurchaseUtilityItem(int btnNumber)
    {
        Vector3 posOffset = new Vector3(0, 3, 1);
        Instantiate(weapons[btnNumber], player.gameObject.transform.localPosition + posOffset, player.gameObject.transform.rotation);
        GameManager.Instance.coins -= shopItemsSO[btnNumber].baseCost;
        FindObjectOfType<AudioManager>().Play("UnlockWeapon");
        coinText.text = GameManager.Instance.coins.ToString();
        CheckPurchasable();
    }

    public void LoadPanelSword()
    {
        for(int i=0; i<shopItemsSO.Length; i++)
        {
            shopPanels[i].titleText.text = shopItemsSO[i].title;
            shopPanels[i].image.sprite = shopItemsSO[i].image;
            shopPanels[i].descriptionText.text = shopItemsSO[i].description;
            if (shopItemsSO[i].purchased == true)
            {
                if(i == currentWeaponIndex)
                {
                    shopPanels[i].costText.text = "Equipped";
                } else
                {
                    shopPanels[i].costText.text = "Equip";
                }
                
            } else
            {
                shopPanels[i].costText.text = shopItemsSO[i].baseCost.ToString() + " coins";
                shopItemsSO[i].purchased = false;
            }
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

    public void OpenSwordsShop()
    {
        swordsMenu.SetActive(true);

        swordsButton.SetActive(false);
        itemsButton.SetActive(false);

        swordIcon.SetActive(false);
        itemsIcon.SetActive(false);
        OnEnable();
    }

    public void OpenItemsShop()
    {
        itemsMenu.SetActive(true);

        itemsButton.SetActive(false);
        swordsButton.SetActive(false);

        swordIcon.SetActive(false);
        itemsIcon.SetActive(false);
        OnEnable();
    }

    public void BackButton()
    {
        if(swordsButton.activeInHierarchy == true)
        {
            ShowShop();
        }
        else
        {
            swordsMenu.SetActive(false);
            itemsMenu.SetActive(false);

            swordsButton.SetActive(true);
            itemsButton.SetActive(true);

            swordIcon.SetActive(true);
            itemsIcon.SetActive(true);
        }
    }
}
