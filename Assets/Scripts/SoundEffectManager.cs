using Unity.VisualScripting;
using UnityEngine;  
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
	public static SoundEffectManager Instance { get; private set; }
	
	private static AudioSource audioSource;  
	private static AudioSource randomPitchAudioSource;
	private static AudioSource voiceAudioSource;
	private static SoundEffectLibrary soundEffectLibrary;
    [SerializeField] private Slider sfxSlider;
	
	private void Awake()  
	{  
		if (Instance == null)  
		{  
			Instance = this;
			AudioSource[] audioSources = GetComponents<AudioSource>();
			audioSource = audioSources[0];
			randomPitchAudioSource = audioSources[1];
			voiceAudioSource = audioSources[2];
			soundEffectLibrary = GetComponent<SoundEffectLibrary>();
			// DontDestroyOnLoad(gameObject); // 跨场景不销毁  
		}  
		else  
		{  
			Destroy(gameObject);  
			return;  
		}

	}

    public static void Play(string soundName, bool randomPitch = false)  
	{
		AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);    
		if (audioClip != null)  
		{  
			if(randomPitch)
			{
				randomPitchAudioSource.pitch = Random.Range(1.5f, 1.5f);
				randomPitchAudioSource.PlayOneShot(audioClip);
			}
			else
			{
				// 使用 PlayOneShot 允许音效重叠播放  
				audioSource.PlayOneShot(audioClip);  
			}
		}  
	}

	public static void PlayVoice(AudioClip audioClip, float pitch = 1f)
	{
		voiceAudioSource.pitch = pitch;
		voiceAudioSource.PlayOneShot(audioClip);
	}
	
	private void Start()  
	{    
		if (sfxSlider != null)  
		{  
			sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
		}  
	}
	
	public static void SetVolume(float volume)  
	{  
		audioSource.volume = volume;  
		randomPitchAudioSource.volume = volume;
		voiceAudioSource.volume = volume;  
	}

	public void OnValueChanged()
	{
		SetVolume(sfxSlider.value);
	}
}