using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;

    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines; // 标记在哪对话结束
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;

    public AudioClip voiceSound;
    public float voicePitch = 1f;

    public DialogueChoice[] choices;

    public int questInProgressIndex; // 任务进行中说的话
    public int questCompletedIndex; // 任务完成后说的话
    public Quest quest; // NPC给的任务
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndexes;
    public bool[] givesQuest; // 标记哪些选项可以触发任务
}