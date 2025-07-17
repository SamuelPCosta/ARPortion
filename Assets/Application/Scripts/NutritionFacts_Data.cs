using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using TMPro;
using System.IO;
using Newtonsoft.Json.Linq;

[Serializable]
public enum productUnity { ml, g, Un} ;
public enum products { MILK_NINHO_NoLAC, COOKIE_SEM_MARCA };

[Serializable]
public class Fact {
    public string name;
    public float value;
    public float dailyValue;
}

[Serializable]
public class NutritionFacts_Data : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Transform table;
    [SerializeField] private TextMeshProUGUI portionText;
    [SerializeField] private GameObject linePrefab;

    [Header("Data")]
    [SerializeField] private products product;
    [SerializeField] private productUnity unity;
    [SerializeField] private int servingSize;
    [SerializeField] private int total;
    [SerializeField] private float servingsPerContainer;
    [SerializeField] private Fact[] facts;

    private int IGNORE_LINES = 3;
    public int linesCount = -1;
    private string[] lines = null;
    private JToken item = null;
    private List<GameObject> data = new List<GameObject>();
    private float currentPortion = -1;

    private void Start()
    {
        LoadData("data", 0);
    }

    public void LoadData(string csvName, int productIndex) {
        TextAsset jsonText = Resources.Load<TextAsset>(csvName);
        var jsonArray = JArray.Parse(jsonText.text);
        item = jsonArray[productIndex];


        unity = Enum.Parse <productUnity>(jsonArray[0]["unity"]?.ToString());
        total = item["total"]?.ToObject<int>() ?? 0;

        servingSize = item["servingSize"]?.ToObject<int>() ?? 0;
        currentPortion = servingSize;
        servingsPerContainer = total / servingSize;


        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();
        //painel 1
        string[] ingredientsParts = item["ingredients"]?.ToString().Split(',') ?? new string[0];
        string ingredientsText = "";

        for (int i = 0; i < ingredientsParts.Length; i++)
        {
            string part = ingredientsParts[i].Trim();
            if (part.Length > 0)
                part = char.ToUpper(part[0]) + part.Substring(1);
            ingredientsText += "• " + part + "\n";
        }
        fullCanvas.ingredients.text = ingredientsText;


        //painel 3
        string[] allergensParts = item["allergens"]?.ToString().Split(',') ?? new string[0];
        string allergensText = "";

        for (int i = 0; i < allergensParts.Length; i++)
            allergensText += "• " + allergensParts[i].Trim() + "\n";
        fullCanvas.allergens.text = allergensText;


        //painel 2
        foreach (Transform child in table.transform)
            Destroy(child.gameObject);

        foreach (Transform child in fullCanvas.table.transform)
            Destroy(child.gameObject);

        portionText.text = "" + currentPortion + " " + unity.ToString();
        FullScreenController fullScreen = FindObjectOfType<FullScreenController>();
        fullScreen.portionText.text = "" + currentPortion + " " + unity.ToString();
        fullScreen.inputField.text = "" + currentPortion;

        foreach (var factJson in item["facts"].Children()){
            Fact fact = new Fact
            {
                name = factJson["name"].ToString(),
                value = factJson["value"].ToObject<float>(),
                dailyValue = factJson["dv"].ToObject<float>()
            };

            GameObject line = Instantiate(linePrefab, table.transform);
            line.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = fact.name;
            line.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ""+fact.value;
            line.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ""+fact.dailyValue;
            data.Add(line);

            GameObject lineFullScreen = Instantiate(line, fullCanvas.table.transform);
        }

        portionText.text = "" + servingSize + " " + unity.ToString();
        fullCanvas.portionText.text = "" + servingSize + " " + unity.ToString();
    }


    public void calculatePortion(float portion)
    {
        currentPortion = portion;
        portionText.text = "" + portion + " " + unity.ToString();

        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();

        int i = 0;
        foreach (var factJson in item["facts"].Children())
        {
            float mult = portion / servingSize;

            string valueText = (item["facts"][i]["value"].ToObject<float>() * mult).ToString(CultureInfo.InvariantCulture);
            string dvText = (item["facts"][i]["dv"].ToObject<float>() * mult).ToString(CultureInfo.InvariantCulture);

            //fullscreen
            Transform row = fullCanvas.table.transform.GetChild(i);
            row.GetChild(1).GetComponent<TextMeshProUGUI>().text = valueText;
            row.GetChild(2).GetComponent<TextMeshProUGUI>().text = dvText;

            //AR
            table.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = valueText;
            table.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = dvText;
            i++;
        }
    }

    public void setData()
    {
        GameObject.FindObjectOfType<FullScreenController>().data = this;

        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();
        foreach (Transform child in fullCanvas.table.transform)
            Destroy(child.gameObject);
        foreach (var obj in data)
        {
            Instantiate(obj, fullCanvas.table.transform);
        }
        if(currentPortion > 0) { 
            portionText.text = "" + currentPortion + " " + unity.ToString();
            FullScreenController fullScreen = FindObjectOfType<FullScreenController>();
            fullScreen.portionText.text = "" + currentPortion + " " + unity.ToString();
            fullScreen.inputField.text = "" + currentPortion;
        }
    }

    public string getUnity()
    {
        return unity.ToString();
    }
}
