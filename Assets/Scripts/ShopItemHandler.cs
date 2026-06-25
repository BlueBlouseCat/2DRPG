using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemHandler : MonoBehaviour, IPointerClickHandler
{
    private bool isShopItem;
    public Slot originalInventorySlot;

    public void Initialise(bool shopItem) => isShopItem = shopItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(isShopItem) BuyItem();
                else SellItem();
        }
    }

    private void BuyItem()
    {
        Item item = GetComponent<Item>();
        ShopSlot slot = GetComponentInParent<ShopSlot>();
        if(!item || !slot) return;

        if(CurrencyController.Instance.GetGold() < slot.itemPrice)
        {
            // 弹窗：金币不足
            Debug.Log("金币不足！");
            return;
        }

        GameObject itemPrefab = FindObjectOfType<ItemDictionary>().GetItemPrefab(item.ID);
        if(InventoryController.Instance.AddItem(itemPrefab))
        {
            CurrencyController.Instance.SpendGold(slot.itemPrice);
            ShopController.Instance.RefreshPlayerInventoryDisplay();
            ShopController.Instance.RemoveItemFromShop(item.ID, 1);
        }
        else
        {
            Debug.Log("背包已满");
        }
    }

    private void SellItem()
    {
        Item item = GetComponent<Item>();
        ShopSlot slot = GetComponentInParent<ShopSlot>();
        if(!item || !slot || !originalInventorySlot) return;

        Item invItem = originalInventorySlot.currentItem?.GetComponent<Item>();
        if(!invItem) return;

        if(invItem.quantity > 1) invItem.RemoveFromStack(1);
        else
        {
            Destroy(originalInventorySlot.currentItem);
            originalInventorySlot.currentItem = null;
        }

        InventoryController.Instance.RebuildItemCounts();
        CurrencyController.Instance.AddGold(slot.itemPrice);
        ShopController.Instance.RefreshPlayerInventoryDisplay();
        ShopController.Instance.AddItemToShop(item.ID, 1);
    }
}
