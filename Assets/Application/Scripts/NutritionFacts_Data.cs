using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using TMPro;

[Serializable]
public enum productUnity { ML, G, Un} ;
public enum products { MILK_NINHO_NoLAC, COOKIE_SEM_MARCA };

[Serializable]
public class Fact {
    public string fact;
    public float perServing;
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

    private void Start()
    {
        LoadFromCSV(product.ToString());
    }

    public void LoadFromCSV(string csvName) 
    {
        TextAsset csv = Resources.Load<TextAsset>(csvName);
        lines = csv.text.Split('\n');

        string[] firstLineParts = lines[0].Split(',');
        unity = (productUnity)System.Enum.Parse(typeof(productUnity), firstLineParts[0]);
        servingSize = int.Parse(firstLineParts[1]);
        total = int.Parse(firstLineParts[2]);
        servingsPerContainer = total / servingSize;

        linesCount = lines.Length - 1;
        facts = new Fact[linesCount];

        //
        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();

        //painel 1
        string[] ingredientsParts = lines[1].Split(',');
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
        string[] allergensParts = lines[2].Split(',');
        string allergensText = "";

        for (int i = 0; i < allergensParts.Length; i++)
            allergensText += "• " + allergensParts[i].Trim() + "\n";
        fullCanvas.allergens.text = allergensText;


        //painel 2
        foreach (Transform child in table.transform)
            Destroy(child.gameObject);

        foreach (Transform child in fullCanvas.table.transform)
            Destroy(child.gameObject);
        //

        for (int i = IGNORE_LINES; i < linesCount; i++)
        {
            string[] parts = lines[i].Split(',');
            facts[i] = new Fact
            {
                fact = parts[0],
                perServing = float.Parse(parts[1], CultureInfo.InvariantCulture),
                dailyValue = float.Parse(parts[2], CultureInfo.InvariantCulture)
            };

            GameObject line = Instantiate(linePrefab, table.transform);
            line.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + facts[i].fact;
            line.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + facts[i].perServing;
            line.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "" + facts[i].dailyValue;
                
            GameObject lineFullScreen = Instantiate(line, fullCanvas.table.transform);
        }

        portionText.text = "" + servingSize + " " + unity.ToString();
        fullCanvas.portionText.text = "" + servingSize + " " + unity.ToString();
    }


    public void calculatePortion(float portion)
    {
        portionText.text = "" + portion + " " + unity.ToString();

        FullScreenController fullCanvas = GameObject.FindObjectOfType<FullScreenController>();
        for (int i = 0; i < linesCount - IGNORE_LINES; i++)
        {

            string[] parts = lines[i + IGNORE_LINES].Split(',');
            float mult = portion / servingSize;

            //fullscreen
            Transform row = fullCanvas.table.transform.GetChild(i);

            row.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                (float.Parse(parts[1], CultureInfo.InvariantCulture) * mult).ToString();

            row.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                (float.Parse(parts[2], CultureInfo.InvariantCulture) * mult).ToString();

            //AR
            table.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text =
                (float.Parse(parts[1], CultureInfo.InvariantCulture) * mult).ToString();

            table.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                (float.Parse(parts[2], CultureInfo.InvariantCulture) * mult).ToString();
        }
    }

    public void setData()
    {
        GameObject.FindObjectOfType<FullScreenController>().data = this;
    }

    public string getUnity()
    {
        return unity.ToString();
    }
}
