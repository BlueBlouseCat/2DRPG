using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent; // 原始槽位
    CanvasGroup canvasGroup;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // 让物品显示在最上层
        canvasGroup.blocksRaycasts = false; // 关闭射线检测，方便检测拖拽到的新的位置
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // 使物品跟随鼠标移动
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        // 拖动交换物品
        if(dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter; // 鼠标碰到的物体
            if(dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        // 最后鼠标落在槽位上
        if(dropSlot != null)
        {
            // 新槽有物品
            if(dropSlot.currentItem != null)
            {
                dropSlot.currentItem.transform.SetParent(originalParent);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // 被拖动的物品都要进新槽
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        // 最后鼠标没落在槽位上，物品回去
        else
        {
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
