using UnityEngine;

public class Flashlight : MonoBehaviour, IUsable
{
    public GameObject lightObject;
    bool isOn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Use()
    {
       isOn = !isOn;
       lightObject.SetActive(isOn);
    }
}
