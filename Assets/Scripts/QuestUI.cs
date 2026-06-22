using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent; // 任务列表
    public GameObject questEntryPrefab; // 任务条目预制体
    public GameObject objectiveTextPrefab; // 目标文字预制体

    void Start()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        // 销毁存在的任务条目
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        // 创建新的任务条目
        foreach(var quest in QuestController.Instance.activateQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestName").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.questName; // ?

            foreach(var objective in quest.objectives)
            {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; // 需要收集5瓶药水 (0/5)
            }
        }
    }
}
