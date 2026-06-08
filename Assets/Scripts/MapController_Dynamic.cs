using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController_Dynamic : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform mapParent;
    [SerializeField] private GameObject areaPrefab;
    [SerializeField] private RectTransform playerIcon;

    [Header("Colours")]
    [SerializeField] private Color defaultColor = Color.gray;
    [SerializeField] private Color currentAreaColor = Color.green;

    [Header("Map Settings")]
    [SerializeField] private GameObject mapBounds; // 区域碰撞器的父对象
    [SerializeField] private PolygonCollider2D initialArea; // 初始区域（玩家没有存档时）
    [SerializeField] private float mapScale = 10f; // 在UI界面上调整地图大小

    private PolygonCollider2D[] mapAreas;
    private Dictionary<string, RectTransform> uiAreas = new Dictionary<string, RectTransform>();

    public static MapController_Dynamic Instance {get; set;}

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mapAreas = mapBounds.GetComponentsInChildren<PolygonCollider2D>();
    }

    // 生成地图
    public void GenerateMap(PolygonCollider2D newCurrentArea = null)
    {
        PolygonCollider2D currentArea = newCurrentArea != null ? newCurrentArea : initialArea;

        ClearMap();

        foreach(PolygonCollider2D area in mapAreas)
        {
            CreateAreaUI(area, area == currentArea);
        }

        MovePlayerIcon(currentArea.name);
    }

    // 清除地图
    private void ClearMap()
    {
        foreach(Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        uiAreas.Clear();
    }

    private void CreateAreaUI(PolygonCollider2D area, bool isCurrent)
    {
        // 创建预制体
        GameObject areaImage = Instantiate(areaPrefab, mapParent);
        RectTransform rectTransform = areaImage.GetComponent<RectTransform>();

        // 获得边界
        Bounds bounds = area.bounds;

        // 缩放
        rectTransform.sizeDelta = new Vector2(bounds.size.x * mapScale, bounds.size.y * mapScale);
        rectTransform.anchoredPosition = bounds.center * mapScale;

        // 设置颜色
        areaImage.GetComponent<Image>().color = isCurrent ? currentAreaColor : defaultColor;

        // 添加到字典中
        uiAreas[area.name] = rectTransform;
    }

    // 更新当前区域
    public void UpdateCurrentArea(string newCurrentArea)
    {
        // 更新颜色
        foreach(KeyValuePair<string, RectTransform> area in uiAreas)
        {
            area.Value.GetComponent<Image>().color = area.Key == newCurrentArea ? currentAreaColor : defaultColor;
        }
        // 更新玩家图标
        MovePlayerIcon(newCurrentArea);
    }

    // 移动玩家图标
    private void MovePlayerIcon(string newCurrentArea)
    {
        if(uiAreas.TryGetValue(newCurrentArea, out RectTransform areaUI))
        {
            playerIcon.anchoredPosition = areaUI.anchoredPosition;
        }
    }
}
