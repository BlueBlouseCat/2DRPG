using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount;
    // [SerializeField] private GameObject[] itemPrefab;
    
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
    }

    // 向背包中添加物品
    public bool AddItem(GameObject itemPrefab)
    {
        // 寻找空槽位
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
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
                invData.Add(new InventorySaveData{itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex()});
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
                    slot.currentItem = item;
                }
            }
        }
    }
}
