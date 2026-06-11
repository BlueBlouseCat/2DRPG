using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] private Image[] tabImages;
    [SerializeField] private GameObject[] pages;
    void Start()
    {
        ActiveTab(0);
    }

    public void ActiveTab(int tabNo)
    {
        for(int i = 0; i < tabImages.Length; i ++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;
    }
}
