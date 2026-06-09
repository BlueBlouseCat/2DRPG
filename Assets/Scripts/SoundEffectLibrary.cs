using System.Collections.Generic;  
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
	[SerializeField] private SoundEffectGroup[] soundEffectGroups;
	
	private Dictionary<string, List<AudioClip>> soundDictionary;
	
	private void Awake()  
	{  
		InitializeDictionary();  
	}
	 
	private void InitializeDictionary()  
	{  
		soundDictionary = new Dictionary<string, List<AudioClip>>();  
		foreach (var soundEffectGroup in soundEffectGroups)  
		{  
			soundDictionary[soundEffectGroup.name] = soundEffectGroup.audioClips;
		}  
	}
	
	public AudioClip GetRandomClip(string name)  
	{  
		if (soundDictionary.ContainsKey(name))  
		{  
			List<AudioClip> audioClips = soundDictionary[name];
            if(audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
		}
		return null;  
	}
}

// 定义一个音效组的结构  
[System.Serializable]  
public struct SoundEffectGroup  
{  
	public string name; // 音效组的名字
	public List<AudioClip> audioClips; // 该组包含的音频文件
}