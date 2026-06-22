using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount;
    [SerializeField] private GameObject[] itemPrefab;

    public static InventoryController Instance {get; private set;}
    Dictionary<int, int> itemsCountCache = new(); // 任务条目缓存<物品id, 数量>
    public event Action OnInventoryChanged; // 当任务变动时通知此事件

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        RebuildItemCounts();
    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if(item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public Dictionary<int, int> GetItemCounts() => itemsCountCache;

    // 向背包中添加物品
    public bool AddItem(GameObject itemPrefab)
    {
        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if(itemToAdd == null) return false;
        // 找一下是否有同样类型的物品
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if(slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    // 同样的物品堆叠摆放
                    slotItem.AddToStack();
                    RebuildItemCounts();
                    return true;
                }
            }
        }

        // 寻找空槽
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                RebuildItemCounts();
                return true;
            }
        }

        Debug.Log("背包满了");
        return false;
    }


    // 从存档中得到保存数据
    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID, 
                    slotIndex = slotTransform.GetSiblingIndex(), 
                    quantity = item.quantity
                });
            }
        }
        return invData;
    }

    // 根据保存数据设置存档
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        // 清空原来的格子，避免覆盖
        foreach(Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 重新生成格子
        for(int i = 0; i < slotCount; i ++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        // 根据存档填物品
        foreach(InventorySaveData data in inventorySaveData)
        {
            if(data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if(itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    Item itemComponent = item.GetComponent<Item>();
                    if(itemComponent != null && data.quantity > 1)
                    {
                        itemComponent.quantity = data.quantity;
                        itemComponent.UpdateQuantityDisplay();
                    }

                    slot.currentItem = item;
                }
            }
        }

        RebuildItemCounts();
    }

    public void RemoveItemFromInventory(int itemID, int amountToRemove)
    {
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            if(amountToRemove <= 0) break;

            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID)
            {
                int removed = Mathf.Min(amountToRemove, item.quantity);
                item.RemoveFromStack(removed);
                amountToRemove -= removed;

                if(item.quantity == 0)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
            }
        }
        RebuildItemCounts();
    }
}
