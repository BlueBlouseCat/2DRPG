using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs; // 所有物品预制体的列表
    private Dictionary<int, GameObject> itemDictionary; // 存储所有预制体的字典

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        for(int i = 0; i < itemPrefabs.Count; i ++)
        {
            if(itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1;
            }
        }

        foreach(Item item in itemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if(prefab == null)
        {
            Debug.LogWarning($"没找到编号为{itemID}的物品!");
        }
        return prefab;
    }
}
