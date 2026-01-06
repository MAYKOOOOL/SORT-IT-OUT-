using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentEffects : MonoBehaviour
{
    public bool flashlightEnabled = false;
    public bool trashlocatorEnabled = false;

    public GameObject lights;
    public GameObject bullet;
    public GameObject bagPanel;
    public GameObject trashlocator;


    public Slider flashlightEnergySlider;
    public Slider shrinkgunEnergySlider;
    public Slider trashlocatorEnergySlider;
    
    public float flashlightEnergy;
    public float shrinkgunEnergy;
    public float trashlocatorEnergy;

    private bool shrinkgunFiredThisFrame = false;
    private void Awake()
    {
        //flashlightEnergySlider.maxValue = flashlightEnergy;
        //flashlightEnergySlider.value = flashlightEnergySlider.maxValue;
        //shrinkgunEnergySlider.maxValue = shrinkgunEnergy;
        //shrinkgunEnergySlider.value = shrinkgunEnergySlider.maxValue;
        //trashlocatorEnergySlider.maxValue = trashlocatorEnergy;
        //trashlocatorEnergySlider.value = trashlocatorEnergySlider.maxValue;
    }

    public void Update()
    {
        //if (flashlightEnabled)
        //{
        //    /*flashlightEnergySlider.value =*/ Mathf.Max(flashlightEnergySlider.value - 10f * Time.deltaTime, 0f);
        //}

        //if (trashlocatorEnabled)
        //{
        //    /*trashlocatorEnergySlider.value =*/ Mathf.Max(trashlocatorEnergySlider.value - 5f * Time.deltaTime, 0f);
        //}

        //if (!shrinkgunFiredThisFrame)
        //{
        //    shrinkgunEnergySlider.value = Mathf.Min(shrinkgunEnergySlider.value + 15f * Time.deltaTime, shrinkgunEnergySlider.maxValue);
        //}

        //shrinkgunFiredThisFrame = false;
    }

    public void UseEquipment(EquipmentEffect equipmentEffect)
    {
        switch (equipmentEffect.equipmentType)
        {
            case EquipmentType.Flashlight:
                if (lights != null)
                {
                    flashlightEnabled = !flashlightEnabled; // Toggle flashlight state
                }
                GameObject child = transform.GetChild(0).gameObject;
                child.SetActive(flashlightEnabled);
                
                break;

            case EquipmentType.Shrinkgun:
                if (bullet != null && shrinkgunEnergySlider.value >= 25f)
                {
                    GameObject spawnedBullet = Instantiate(bullet, transform.position + transform.forward, Quaternion.identity);
                    Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.AddForce(transform.forward * 500f);

                    shrinkgunEnergySlider.value = Mathf.Max(shrinkgunEnergySlider.value - 25f, 0f);
                    shrinkgunFiredThisFrame = true;
                }
                break;

            case EquipmentType.Trashbag:
                if (bagPanel != null)
                    bagPanel.SetActive(!bagPanel.activeSelf); // Toggle bag panel
                break;

            case EquipmentType.Trashlocator:
                trashlocatorEnabled = !trashlocatorEnabled; // Toggle trash locator state
                Debug.Log("Trashlocator effect not implemented yet.");
                break;

            default:
                Debug.LogWarning("Unhandled equipment type: " + equipmentEffect.equipmentType);
                break;
        }
    }
}
