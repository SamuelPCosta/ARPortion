using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System.Globalization;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class FullScreenController : MonoBehaviour
{
    public Transform table;
    public TextMeshProUGUI ingredients;
    public TextMeshProUGUI allergens;
    public TextMeshProUGUI portionText;
    public TextMeshProUGUI unityText;
    public TMP_InputField inputField;
    
    public GameObject Overlay;
    public GameObject[] panels;
    public GameObject allergenItems;
    public GameObject tutorial;
    public Transform pagination;

    public ScrollRect scrollRect;
    public int panelsCount = 3;

    float lastScrollPos;

    public NutritionFacts_Data data = null;
    [SerializeField] private TextMeshProUGUI portion;

    public void SetPanelPosition(int panelIndex)
    {
        float step = 1f / (panelsCount - 1);
        panelIndex = Mathf.Clamp(panelIndex, 0, panelsCount - 1);
        scrollRect.horizontalNormalizedPosition = panelIndex * step;
        lastScrollPos = scrollRect.horizontalNormalizedPosition;
    }

    void Update()
    {
        float scrollPos = scrollRect.horizontalNormalizedPosition;
        float step = 1f / (panelsCount - 1);
        float raw = scrollPos / step;
        float dir = scrollPos - lastScrollPos;

        float targetIndex = Mathf.Round(raw);
        if (Mathf.Abs(dir) > 0.003f)
            targetIndex = dir > 0 ? Mathf.Ceil(raw) : Mathf.Floor(raw);

        if (targetIndex != 1)
            MarkTutorialAsSeen();

        SetActiveDot((int)targetIndex);

        float nearest = Mathf.Clamp01(targetIndex * step);
        scrollRect.horizontalNormalizedPosition = Mathf.MoveTowards(scrollPos, nearest, 0.05f);

        lastScrollPos = scrollPos;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Overlay.activeSelf)
                Overlay.SetActive(false);
            else
                Application.Quit();
        }
    }

    public void calculate()
    {
        string input = portion.text.Trim().Replace("\u200B", "").Replace(",", ".");
        if (input.Equals(""))
            return;

        portionText.text = input+" "+data.getUnity();

        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float portionValue))
        {
            data.calculatePortion(portionValue);
        }
    }
    public void SetActiveDot(int index)
    {
        int i = 0;
        foreach (Transform dot in pagination){
            Image img = dot.GetComponent<Image>();
            img.color = (i == index) ? new Color32(0xFD, 0x7A, 0x12, 0xFF) : new Color32(0xAA, 0xAA, 0xAA, 0xFF);
            i++;
        }
    }

    public void MarkTutorialAsSeen()
    {
        tutorial.SetActive(false);
        PlayerPrefs.SetInt("tutorial", 1);
        PlayerPrefs.Save();
    }

    public void setTutorial()
    {
        if (PlayerPrefs.GetInt("tutorial", 0) == 0)
            tutorial.SetActive(true);
        else
            tutorial.SetActive(false);
    }
}
