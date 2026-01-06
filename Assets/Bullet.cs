using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject breakEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Trash trash = collision.gameObject.GetComponent<Trash>();
        if(trash != null && trash.isBig)
        {
            trash.Shrink();
        }
        Instantiate(breakEffect, transform.position, Quaternion.identity);
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
