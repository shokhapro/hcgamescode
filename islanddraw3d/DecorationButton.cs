using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecorationButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private int price;
    [SerializeField] private Text priceText;
    [SerializeField] private GameObject[] unpaidStyleElements;
    [Space]
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private float randomRotation = 360;
    [SerializeField] private int maxCount = 10;
    
    private string _savename;
    private List<GameObject> _objectsList = new List<GameObject>();
    
    public void Set(GameObject prefabValue, Sprite iconValue, int priceValue, float randRotValue, int maxCountValue)
    {
        icon.sprite = iconValue;
        price = priceValue;
        priceText.text = "$" + price;
        objectPrefab = prefabValue;
        randomRotation = randRotValue;
        maxCount = maxCountValue;

        _savename = "deco_" + objectPrefab.name + "_open";

        var isUnpaid = price > 0 && !PlayerPrefs.HasKey(_savename);
        foreach (var e in unpaidStyleElements)
            e.gameObject.SetActive(isUnpaid);
    }
    
    public void Click()
    {
        if (price > 0 && !PlayerPrefs.HasKey(_savename))
        {
            if (GameManager.Instance.SpendMoney(price))
            {
                PlayerPrefs.SetInt(_savename, 1);
                
                foreach (var e in unpaidStyleElements)
                    e.gameObject.SetActive(false);
            }
            
            return;
        }

        var count = 0;
        
        for (var i = 0; i < _objectsList.Count; i++)
            if (_objectsList[i])
                count++;
            else
            {
                _objectsList.RemoveAt(i);
                i--;
            }

        if (count >= maxCount) return;

        var obj = Decorator.Instance.AddObject(objectPrefab, randomRotation);
        
        _objectsList.Add(obj);
    }
}
