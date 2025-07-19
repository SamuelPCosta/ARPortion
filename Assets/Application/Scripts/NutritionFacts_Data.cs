using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using TMPro;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

[Serializable]
public enum productUnity { ml, g, un} ;

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
    [SerializeField] private TextMeshProUGUI productName;
    [SerializeField] private GameObject linePrefab;

    [Header("Data")]
    [SerializeField] private productUnity unity;
    [SerializeField] private int servingSize;
    [SerializeField] private int total;
    [SerializeField] private float servingsPerContainer;
    [SerializeField] private Fact[] facts;

    private string[] lines = null;
    private JToken item = null;
    private List<GameObject> data = new List<GameObject>();
    private string ingredients = "";
    private JToken allergens;
    private float currentPortion = -1;
    private string itemName;

    public void LoadData(string product) {
        TextAsset jsonText = Resources.Load<TextAsset>("data");
        var jsonArray = JArray.Parse(jsonText.text);

        if (product.Equals("Null"))
            item = jsonArray.Children<JObject>().FirstOrDefault(x => (string)x["product"] == "Error");
        else
            item = jsonArray.Children<JObject>().FirstOrDefault(x => (string)x["product"] == product);


        itemName = product;
        productName.text = itemName;
        Enum.TryParse<productUnity>(item["unity"]?.ToString(), true, out var unity);
        total = item["total"]?.ToObject<int>() ?? 0;
        servingSize = item["servingSize"]?.ToObject<int>() ?? 0;
        currentPortion = servingSize;
        servingsPerContainer = total / servingSize;


        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();
        //painel 1 & 3
        ingredients = fullCanvas.ingredients.text = FormatBulletList(item["ingredients"]);
        allergens = item["allergens"];
        setAllergnsItems(item["allergens"]);



        //painel 2
        foreach (Transform child in table.transform)
            Destroy(child.gameObject);

        foreach (Transform child in fullCanvas.table.transform)
            Destroy(child.gameObject);

        FullScreenController fullScreen = FindObjectOfType<FullScreenController>();
        fullScreen.portionText.text = portionText.text = "" + currentPortion + unity;

        fullScreen.inputField.text = "" + currentPortion;

        foreach (var factJson in item["facts"].Children()){
            Fact fact = new Fact{
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

        setName();

        portionText.text = "" + servingSize + unity;
        fullCanvas.portionText.text = "" + servingSize + unity;
    }

    private string FormatBulletList(JToken token)
    {
        var parts = token?.ToString().Split(',') ?? new string[0];
        string result = "";

        foreach (var partRaw in parts)
        {
            var part = partRaw.Trim();
            if (part.Length > 0)
                part = char.ToUpper(part[0]) + part.Substring(1);
            result += "• " + part + "\n";
        }

        return result;
    }

    private void setAllergnsItems(JToken token)
    {
        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();

        var allergensOfProduct = token?.ToString().ToLower().Split(',').
                                Select(str => str.Trim()).ToArray() ?? new string[0];

        List<GameObject> items = fullCanvas.allergenItems.transform.Cast<Transform>().
                                Select(transform => transform.gameObject).ToList();

        foreach (var item in items){
            if (allergensOfProduct.Contains(item.name.ToLower()))
                item.SetActive(true);
            else
                item.SetActive(false);
        }
    }

    public void calculatePortion(float portion)
    {
        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();

        currentPortion = portion;
        fullCanvas.portionText.text = portionText.text = "" + portion + unity.ToString().Trim();

        int i = 0;
        foreach (var factJson in item["facts"].Children())
        {
            float mult = portion / servingSize;

            string valueText = (item["facts"][i]["value"].ToObject<float>() * mult).ToString("0.#", CultureInfo.InvariantCulture);
            string dvText = (item["facts"][i]["dv"].ToObject<float>() * mult).ToString("0.#", CultureInfo.InvariantCulture);

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

        //Panel 2
        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();
        foreach (Transform child in fullCanvas.table.transform)
            Destroy(child.gameObject);
        foreach (var obj in data)
        {
            Instantiate(obj, fullCanvas.table.transform);
        }

        portionText.text = "" + currentPortion + unity;
        FullScreenController fullScreen = FindObjectOfType<FullScreenController>();
        fullScreen.portionText.text = "" + currentPortion + unity;
        fullScreen.inputField.text = "" + currentPortion;
        

        //Panel 1 & 3
        fullCanvas.ingredients.text = FormatBulletList(item["ingredients"]);
        setAllergnsItems(item["allergens"]);

        setName();
    }

    private void setName()
    {
        FullScreenController fullScreen = FindObjectOfType<FullScreenController>();
        foreach (var panel in fullScreen.panels)
        {
            panel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName;
        }
    }

    public string getUnity()
    {
        return unity.ToString();
    }
}
