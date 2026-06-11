using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapController_Manual : MonoBehaviour
{
    public static MapController_Manual Instance {get; set;}

    [SerializeField] private GameObject mapParent;
    private List<Image> mapImages;

    public Color highlightColor = Color.white;
    public Color dimmedColor = new Color(1f, 1f, 1f, 0.5f);

    public RectTransform playerIconTransform;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        mapImages = mapParent.GetComponentsInChildren<Image>().ToList();
    }

    public void HighlightArea(string areaName)
    {
        for(int i = 0; i < mapImages.Count; i ++)
        {
            if(mapImages[i].name == areaName)
            {
                mapImages[i].color = highlightColor;

                playerIconTransform.position = mapImages[i].GetComponent<RectTransform>().position;
            }
            else
            {
                mapImages[i].color = dimmedColor;
            }
        }
    }
}
