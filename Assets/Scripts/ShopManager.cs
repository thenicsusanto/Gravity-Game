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
        if(GameManager.Instance.coins >= shopItemsSO[btnNumber].baseCost)
        {
            GameManager.Instance.coins -= shopItemsSO[btnNumber].baseCost;
            coinUI.text = "Coins: " + GameManager.Instance.coins.ToString();
            shopPanels[btnNumber].costText.text = "Equipped";
            Instantiate(weapons[btnNumber]);
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
        }
    }

    public void CheckPurchasable()
    {
        for(int i=0; i<shopItemsSO.Length; i++)
        {
            if(GameManager.Instance.coins >= shopItemsSO[i].baseCost)
            {
                myPurchaseButtons[i].interactable = true;
            } else
            {
                myPurchaseButtons[i].interactable = false;
            }
        }
    }
}
