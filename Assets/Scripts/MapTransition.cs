using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundry;
    CinemachineConfiner confiner;
    [SerializeField] Direction direction; 
    [SerializeField] float addictivePos = 2f;
    enum Direction {Up, Down, Left, Right}

    void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            confiner.m_BoundingShape2D = mapBoundry;
            UpdatePlayerPosition(collision.gameObject);

            MapController_Manual.Instance?.HighlightArea(mapBoundry.name); // mapBoundry是实际的地图边界，但由于每个地图边界的名字都和小地图对应上了，所以能正确高亮小地图
            MapController_Dynamic.Instance?.UpdateCurrentArea(mapBoundry.name);
        }
    }

    // 防止经过传送点又被传送回去
    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position; // 玩家传送后的位置

        switch(direction)
        {
            case Direction.Up:
                newPos.y += addictivePos;
                break;
            case Direction.Down:
                newPos.y -= addictivePos;
                break;
            case Direction.Left:
                newPos.x -= addictivePos;
                break;
            case Direction.Right:
                newPos.x += addictivePos;
                break;
        }

        player.transform.position = newPos;
    }
}
