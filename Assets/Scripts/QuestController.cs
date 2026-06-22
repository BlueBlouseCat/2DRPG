using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance {get; private set;}
    public List<QuestProgress> activateQuests = new();
    private QuestUI questUI;

    public List<string> handinQuestIDs = new(); // 提交的任务id列表

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindObjectOfType<QuestUI>();
        InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }

    public void AcceptQuest(Quest quest)
    {
        if(IsQuestActive(quest.questID)) return;

        activateQuests.Add(new QuestProgress(quest));
        
        CheckInventoryForQuests();
        questUI.UpdateQuestUI();
    }

    public bool IsQuestActive(string questID) => activateQuests.Exists(q => q.QuestID == questID);

    public void CheckInventoryForQuests()
    {
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach(QuestProgress quest in activateQuests)
        {
            foreach(QuestObjective questObjective in quest.objectives)
            {
                if(questObjective.type != ObjectiveType.CollectItem) continue;
                if(!int.TryParse(questObjective.objectiveID, out int itemID)) continue;

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0; // (10/5)
            
                if(questObjective.currentAmount != newAmount)
                {
                    questObjective.currentAmount = newAmount;
                }
            }
        }

        questUI.UpdateQuestUI();
    }

    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInQuest(string questID)
    {
        // 移除所需物品
        if(!RemoveRequiredItemsFromInventory(questID))
        {
            return;
        }
        // 从日志中删除任务
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if(quest != null)
        {
            handinQuestIDs.Add(questID);
            activateQuests.Remove(quest);
            questUI.UpdateQuestUI();
        }
    }

    public bool IsQuestHandedIn(string questID)
    {
        return handinQuestIDs.Contains(questID);
    }

    public bool RemoveRequiredItemsFromInventory(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if(quest == null) return false;
        Dictionary<int, int> requiredItems = new();

        // 验证库存中是否有足够物品
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        
        // 获取所有要扣除的道具的数量
        foreach(QuestObjective objective in quest.objectives)
        {
            if(objective.type == ObjectiveType.CollectItem && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItems[itemID] = requiredItems.GetValueOrDefault(itemID, 0) + objective.requiredAmount;
            }
        }

        // 校验道具是否充足
        foreach(var item in requiredItems)
        {
            int have = itemCounts.GetValueOrDefault(item.Key, 0);
            if(have < item.Value)
            {
                return false;
            }
        }

        // 从背包中移除需要物品
        foreach(var itemRequirement in requiredItems)
        {
            InventoryController.Instance.RemoveItemFromInventory(itemRequirement.Key, itemRequirement.Value);
        }

        return true;
    }

    // 根据存档加载保存的任务进度
    public void LoadQuestProgress(List<QuestProgress> savedQuests)
    {
        activateQuests = savedQuests ?? new();

        CheckInventoryForQuests();
        questUI.UpdateQuestUI();
    }
}
