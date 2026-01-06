using UnityEngine;

public class ExitCheck : MonoBehaviour
{
    public Gate gate;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gate != null)
            {
                gate.waterSlider.gameObject.SetActive(false);
            }
        }
            
    }
}
