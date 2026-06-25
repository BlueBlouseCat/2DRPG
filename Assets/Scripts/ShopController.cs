using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("UI")]
    public GameObject shopPanel;
    public Transform shopInventoryGrid, playerInventoryGrid;
    public GameObject shopSlotPrefab;
    public TMP_Text playerMoneyText, shopTitleText;

    private ItemDictionary itemDictionary;
    private ShopNPC currentShop;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        shopPanel.SetActive(false);
        if(CurrencyController.Instance != null)
        {
            CurrencyController.Instance.OnGoldChanged += UpdateMoneyDisplay;
        }
    }

    private void UpdateMoneyDisplay(int amount)
    {
        if(playerMoneyText != null)
        {
            playerMoneyText.text = amount.ToString();
        }
    }

    public void OpenShop(ShopNPC shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        if(shopTitleText != null) shopTitleText.text = shop.shopKeeperName + "的店铺";
        RefreshShopDisplay();
        RefreshPlayerInventoryDisplay();
        PauseController.SetPause(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
        PauseController.SetPause(false);
    }

    public void RefreshShopDisplay()
    {
        if(currentShop == null) return;
        foreach(Transform child in shopInventoryGrid) Destroy(child.gameObject);

        foreach(var stockItem in currentShop.GetCurrentStock())
        {
            if(stockItem.quantity <= 0) continue;

            CreateShopSlot(shopInventoryGrid, stockItem.itemID, stockItem.quantity, true);
        }
    }

    public void RefreshPlayerInventoryDisplay()
    {
        if(InventoryController.Instance == null) return;
        foreach(Transform child in playerInventoryGrid) Destroy(child.gameObject);

        foreach(Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot inventorySlot = slotTransform.GetComponent<Slot>();
            if(inventorySlot?.currentItem != null)
            {
                Item originalItem = inventorySlot.currentItem.GetComponent<Item>();
                CreateShopSlot(playerInventoryGrid, originalItem.ID, originalItem.quantity, false, inventorySlot);
            }
        }
    }

    private void CreateShopSlot(Transform grid, int itemID, int quantity, bool isShop, Slot originalSlot = null)
    {
        GameObject slotObj = Instantiate(shopSlotPrefab, grid);
        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);
        if(itemPrefab == null) return;

        GameObject itemInstance = Instantiate(itemPrefab, slotObj.transform);
        itemInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Item item = itemInstance.GetComponent<Item>();
        item.quantity = quantity;
        item.UpdateQuantityDisplay();

        int price = isShop ? item.buyPrice : item.GetSellPrice();

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();
        slot.isShopSlot = isShop;
        slot.SetItem(itemInstance, price);

        // ItemHandler
        ItemDragHandler dragHandler = itemInstance.GetComponent<ItemDragHandler>();
        if(dragHandler) dragHandler.enabled = false;

        ShopItemHandler handler = itemInstance.AddComponent<ShopItemHandler>();
        handler.Initialise(isShop);
        if(!isShop) handler.originalInventorySlot = originalSlot;
    }

    public void AddItemToShop(int itemID, int quantity)
    {
        if(!currentShop) return;
        currentShop.AddToStock(itemID, quantity);
        RefreshShopDisplay();
    }

    public bool RemoveItemFromShop(int itemID, int quantity)
    {
        if(!currentShop) return false;
        bool success = currentShop.RemoveFromShopStock(itemID, quantity);
        if(success) RefreshShopDisplay();
        return success;
    }
}
