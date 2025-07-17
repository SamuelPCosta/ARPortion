using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System.Globalization;

public class FullScreenController : MonoBehaviour
{
    public Transform table;
    public TextMeshProUGUI ingredients;
    public TextMeshProUGUI allergens;
    public TextMeshProUGUI portionText;

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

        float nearest = Mathf.Clamp01(targetIndex * step);
        scrollRect.horizontalNormalizedPosition = Mathf.MoveTowards(scrollPos, nearest, 0.05f);

        lastScrollPos = scrollPos;
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
}
