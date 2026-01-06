using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public ShrinkGun shrinkGun;
    [Header("UI")]
    public TextMeshProUGUI itemNameText;
    private float nameDisplayTimer = 0f;
    private float nameDisplayDuration = 3f;

    [Header("Inventory Settings")]
    public InventorySO inventory;
    public int slotIndex = 0;
    public int currentSlotSize; // set this to your hotbar size

    [Header("Player References")]
    public GameObject player;

    private float scrollCooldown = 0.1f;
    private float lastScrollTime;

    [Header("Input Flags")]
    public Vector2 move;
    public bool onMove, onRun, onCrouch, onAim, onJump, onCollect, onUse, onThrow, onDrop, onInteract, onSwim, onSwimDown;
    public bool hasItemInSlot;
    public bool hasEquipmentInSlot;
    private void Awake()
    {
        if (shrinkGun == null)
            shrinkGun = FindAnyObjectByType<ShrinkGun>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        currentSlotSize = inventory.minSize;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void OnHotbar(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // The Input System sends us the name of the control that triggered the action.
        // For keyboard keys, this is usually "1", "2", etc.
        string keyName = context.control.name;

        // Parse the key name to an integer
        if (int.TryParse(keyName, out int numberKey))
        {
            // Convert key (1-9) to array index (0-8)
            int newIndex = numberKey - 1;

            // Safety check: Ensure index is within your current slot size
            if (newIndex >= 0 && newIndex < currentSlotSize)
            {
                slotIndex = newIndex;
                Debug.Log($"Hotbar Key {numberKey} Pressed -> Slot {slotIndex}");
                ShowItemName();
            }
        }
    }

    //SCROLL INPUT
    public void OnScroll(InputAction.CallbackContext context)
    {
        // Get the scroll delta from the mouse wheel
        Vector2 scrollValue = context.ReadValue<Vector2>();

        // Prevent rapid scroll switching
        if (Time.time - lastScrollTime < scrollCooldown)
            return;

        if (scrollValue.y > 0.1f)
        {
            slotIndex++;
        }
        else if (scrollValue.y < -0.1f)
        {
            slotIndex--;
        }

        // Wrap or clamp slot index
        if (slotIndex < 0)
            slotIndex = currentSlotSize - 1;
        else if (slotIndex >= currentSlotSize)
            slotIndex = 0;

        lastScrollTime = Time.time;

        Debug.Log($"Current Hotbar Slot: {slotIndex}");

        ShowItemName();
    }

    public void Update()
    {
        shrinkGun.gameObject.SetActive(onAim);

        if(player == null)
        {
            player = FindAnyObjectByType<PlayerStats>().gameObject;
        }

        int id = inventory.itemIDs[slotIndex];

        // 2. Determine the state based on the ID.
        if (id == 1) // Specific Equipment (ID 1)
        {
            hasEquipmentInSlot = true;
            hasItemInSlot = false;
        }
        else if (id != -1) // Any other item (ID 0, 2, 3, etc.)
        {
            hasItemInSlot = true;
            hasEquipmentInSlot = false;
        }
        else // Empty slot (ID -1)
        {
            hasItemInSlot = false;
            hasEquipmentInSlot = false;
        }

        if (nameDisplayTimer > 0)
        {
            nameDisplayTimer -= Time.deltaTime;

            if (nameDisplayTimer <= 0)
                itemNameText.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (onCollect) onCollect = false;
        if (onUse) onUse = false;
        if (onDrop) onDrop = false;
    }

    public void ShowItemName()
    {
        int id = inventory.itemIDs[slotIndex];

        if (id == -1)
        {
            itemNameText.gameObject.SetActive(false);
            return;
        }

        string itemName = inventory.itemDatabase.items[id].itemName;

        itemNameText.text = itemName;
        itemNameText.gameObject.SetActive(true);

        nameDisplayTimer = nameDisplayDuration; // reset timer
    }

    // Movement Input
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        onMove = context.performed || context.started;
        if (context.canceled) onMove = false;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        onAim = context.performed;
        if (context.canceled) onAim = false;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        onRun = context.performed;
        if (context.canceled) onRun = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        onCrouch = context.performed;
        if (context.canceled) onCrouch = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        onJump = context.performed;
        if (context.canceled) onJump = false;
    }

    public void OnCollect(InputAction.CallbackContext context)
    {
        if (context.performed)
            onCollect = true;
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
            onUse = true;
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
            onDrop = true;
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
            onThrow = true;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        onInteract = context.performed;
        if (context.canceled)
            onInteract = false;
    }

    public void OnSwimDown(InputAction.CallbackContext context)
    {
        if (context.performed && onSwim)
            onSwimDown = true;

        if (context.canceled)
            onSwimDown = false;
    }
}
