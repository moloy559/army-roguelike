using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaneResourceDisplay : MonoBehaviour
{
    public Image resourceImage;
    public TextMeshProUGUI resourceText;

    public void Fill(Sprite sprite, int amount)
    {
        resourceImage.sprite = sprite;
        resourceText.text = amount.ToString();
    }

    public void Fill(ResourceSet resourceSet)
    {
        Fill(GameManager.Instance.resourceData[resourceSet.resourceName].sprite, resourceSet.amount);
    }
}
