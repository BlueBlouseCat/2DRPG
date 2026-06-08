using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 10; // 每个槽位对应键盘1-0的键位

    private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;
    
    private void Awake()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for(int i = 0; i < slotCount; i ++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }

    void Update()
    {
        for(int i = 0; i < slotCount; i ++)
        {
            if(Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                // 使用物品
                UseItemInSlot(i);
            }
        }
    }

    void UseItemInSlot(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if(slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            item.UseItem();
        }
    }

    // 从存档中得到保存数据
    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach(Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData{itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex()});
            }
        }
        return hotbarData;
    }

    // 根据保存数据设置存档
    public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
    {
        // 清空原来的格子，避免覆盖
        foreach(Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 重新生成格子
        for(int i = 0; i < slotCount; i ++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        // 根据存档填物品
        foreach(InventorySaveData data in inventorySaveData)
        {
            if(data.slotIndex < slotCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
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
