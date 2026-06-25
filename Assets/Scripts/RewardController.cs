using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardController : MonoBehaviour
{
    public static RewardController Instance {get; private set;}

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void GiveQuestReward(Quest quest)
    {
        if(quest?.questRewards == null) return;
        
        foreach(var reward in quest.questRewards)
        {
            switch(reward.type)
            {
                case RewardType.Item:
                    GiveItemReward(reward.rewardID, reward.amount);
                    break;
                case RewardType.Gold:
                    break;
                case RewardType.Experience:
                    break;
                case RewardType.Custom:
                    break;

            }
        }
    }

    public void GiveItemReward(int itemID, int amount)
    {
        var itemPrefab = FindAnyObjectByType<ItemDictionary>()?.GetItemPrefab(itemID);

        if(itemPrefab == null) return;

        for(int i = 0; i < amount; i ++)
        {
            if(!InventoryController.Instance.AddItem(itemPrefab))
            {
                GameObject dropItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
                dropItem.GetComponent<BounceEffect>().StartBounce();
            }
            else
            {
                itemPrefab.GetComponent<Item>().ShowPopUp();
            }
        }
    }
}
