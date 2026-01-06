using UnityEngine;
using System.Collections.Generic;
using TMPro;

public struct TrashResult
{
    public Sprite sprite;
    public bool correct;
    public int value;
    public TrashType type;
}
public class Extract : MonoBehaviour
{
    public List<Trashcan> trashcans = new List<Trashcan>();

    public GameObject player;
    public TextMeshProUGUI interactText;

    public GameObject extractor;

    private Vector3 startPos;
    private Vector3 targetPos;

    private bool isExtracting = false;
    private bool isReturning = false;
    private bool hasCollected = false;

    private float moveSpeed = 5f;

    public GameObject coinPrefab;
    public GameObject recieptPrefab;
    public GameObject coinSpawn;
    public GameObject recieptSpawn;

    void Start()
    {
        if (extractor != null)
        {
            startPos = extractor.transform.localPosition;
            targetPos = new Vector3(startPos.x, 8.5f, startPos.z);
        }
    }

    void Update()
    {
        if (player != null)
        {
            if (PlayerController.Instance.onInteract)
            {
                Interact();
                player = null;
            }
        }

        if (isExtracting)
        {
            extractor.transform.localPosition = Vector3.MoveTowards(
                extractor.transform.localPosition,
                targetPos,
                moveSpeed * Time.deltaTime * 2
            );

            if (extractor.transform.localPosition.y <= 8.5f)
            {
                if (!hasCollected)
                {
                    Collect();
                    hasCollected = true;
                }

                isExtracting = false;
                isReturning = true;
            }
        }
        else if (isReturning)
        {
            extractor.transform.localPosition = Vector3.MoveTowards(
                extractor.transform.localPosition,
                startPos,
                moveSpeed * Time.deltaTime * 2
            );

            if (extractor.transform.localPosition.y >= startPos.y)
            {
                extractor.transform.localPosition = startPos;
                isReturning = false;
                hasCollected = false;
            }
        }
    }

    public void Interact()
    {
        if (!isExtracting && !isReturning)
        {
            isExtracting = true;
        }
    }

    public void Collect()
    {
        List<TrashResult> results = new List<TrashResult>();

        foreach (var trashcan in trashcans)
        {
            foreach (var trashGO in trashcan.trashCollected)
            {
                int trashLayer = trashGO.layer;
                int acceptedLayer = LayerMask.NameToLayer(trashcan.acceptedTrash.ToString());
                int price = 0;

                // get database info
                ItemPickup pickup = trashGO.GetComponent<ItemPickup>();
                Sprite icon = null;

                if (pickup != null)
                {
                    ItemData data = pickup.itemDatabase.GetItemByID(pickup.itemID);
                    price = data.price;
                    icon = data.icon;
                }

                bool correct = trashLayer == acceptedLayer;
                int finalValue = correct ? price : -price;

                results.Add(new TrashResult
                {
                    sprite = icon,
                    correct = correct,
                    value = finalValue,
                    type = trashcan.acceptedTrash
                });

                Destroy(trashGO);
            }

            trashcan.trashCollected.Clear();
        }

        if (results.Count == 0)
        {
            return;
        }

        SpawnReceipt(results);
    }
    private void SpawnReceipt(List<TrashResult> results)
    {
        if (recieptPrefab == null || recieptSpawn == null)
        {
            Debug.LogWarning("Receipt prefab or spawn not set!");
            return;
        }

        GameObject obj = Instantiate(recieptPrefab, recieptSpawn.transform.position, Quaternion.identity);

        Reciept receipt = obj.GetComponent<Reciept>();
        if (receipt != null)
        {
            receipt.results = results;
        }
    }
    private void SpawnCoin(int value)
    {
        if (coinPrefab == null || coinSpawn == null)
        {
            Debug.LogWarning("Coin prefab or spawn point not assigned!");
            return;
        }

        GameObject coinObj = Instantiate(coinPrefab, coinSpawn.transform.position, Quaternion.identity);

        Coin coin = coinObj.GetComponent<Coin>();
        if (coin != null)
        {
            coin.value = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            interactText.text = "Press E to Extract";
            interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            interactText.gameObject.SetActive(false);
        }
    }
}
