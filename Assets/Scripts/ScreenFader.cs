using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f; 
    [SerializeField] CinemachineVirtualCamera Vcam;

    CinemachineFramingTransposer transposer;
    Vector3 originalDamping;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        transposer = Vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        originalDamping = new Vector3(transposer.m_XDamping, transposer.m_YDamping, transposer.m_ZDamping);
    }

    async UniTask Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha, t = 0;
        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / fadeDuration);
            await UniTask.Yield();
        }
        canvasGroup.alpha = targetTransparency;
    }

    public async UniTask FadeOut()
    {
        await Fade(1);
        SetDamping(Vector3.zero);
    }

    public async UniTask FadeIn()
    {
        await Fade(0);
        SetDamping(originalDamping);
    }

    void SetDamping(Vector3 d)
    {
        if(!transposer) return;
        transposer.m_XDamping = d.x;
        transposer.m_YDamping = d.y;
        transposer.m_ZDamping = d.z;
    }
}
