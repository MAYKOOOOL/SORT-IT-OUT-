using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlot : MonoBehaviour
{
    [Header("UI Prefab")]
    public GameObject buffIconSlotPrefab; // The prefab with 2 children: Image and Text
    public Transform buffParent; // Parent to hold instantiated icons
    public Sprite[] buffIcons; // Array of sprites for each buff type (index matches PotionType order)

    // Track how many times each buff is applied
    private Dictionary<int, BuffIconData> activeBuffs = new Dictionary<int, BuffIconData>();

    private class BuffIconData
    {
        public GameObject iconObject;
        public TextMeshProUGUI countText;
        public int count;
    }

    /// <summary>
    /// Add a buff icon by its index (matches PotionType order)
    /// </summary>
    public void AddBuff(int buffIndex)
    {
        if (buffIndex < 0 || buffIndex >= buffIcons.Length)
        {
            Debug.LogWarning("Invalid buff index: " + buffIndex);
            return;
        }

        // Already active? Increment count
        if (activeBuffs.ContainsKey(buffIndex))
        {
            activeBuffs[buffIndex].count++;
            activeBuffs[buffIndex].countText.text = "x" + activeBuffs[buffIndex].count;
            return;
        }

        // Instantiate a new buff icon
        GameObject iconObj = Instantiate(buffIconSlotPrefab, buffParent);
        Image iconImage = iconObj.transform.GetChild(0).GetComponent<Image>(); // Child 0 = icon
        TextMeshProUGUI countText = iconObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); // Child 1 = text

        iconImage.sprite = buffIcons[buffIndex];
        countText.text = "x1";

        // Store in dictionary
        BuffIconData data = new BuffIconData
        {
            iconObject = iconObj,
            countText = countText,
            count = 1
        };

        activeBuffs[buffIndex] = data;
    }
}
