using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlotElement : MonoBehaviour, IPointerDownHandler, IDropHandler
{
    [SerializeField]
    private Sprite defaultBG;
    [SerializeField]
    private Sprite hasItemBG;
    [SerializeField]
    private Sprite hasAugBG;
    [SerializeField]
    private Sprite hasWepBG;
    private Sprite current;
    public Image background;
    public Image itemDisplay;
    public GameObject amountDisplay;
    private TMP_Text amountText;
    public ItemInventory inventory;
    public SlotType slotType;
    [SerializeField]
    public int slotIndex;
    public Slot slot;
    
    public void Awake()
    {
        current = null;
        amountText = amountDisplay.GetComponent<TMP_Text>();
    }
    void OnValidate()
    {
        if (slotType == SlotType.Inventory)
        {
            slot = inventory.Container[slotIndex];
        }
        else if (slotType == SlotType.Augmentation)
        {
            slot = inventory.AContainer[slotIndex];
        }
        else if (slotType == SlotType.Weapon)
        {
            slot = inventory.WContainer[slotIndex];
        }

        background.sprite = defaultBG;
        itemDisplay.sprite = current;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 
        //Debug.Log("OnPointerDown called on slot " + slotIndex);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop called on slot " + slotIndex);
        //swaps this slot's contents with MContainer
        if (eventData.pointerDrag.GetComponent<DragDrop>() != null)
        {
            inventory.MoveSwapCombine(eventData.pointerDrag.GetComponent<DragDrop>().parent.GetComponent<InventorySlotElement>().slot, slot);
            inventory.Print();
        }
        
    }

    public void Update()
    {
        // if slot.item changes, update current to slot.item.icon
        if (slot.item != null && itemDisplay.sprite != slot.item.icon)
        {
            current = slot.item.icon;
            itemDisplay.sprite = current;
            itemDisplay.color = new Color (255, 255, 255, 1);

            if (slot.item.GetType() == typeof(AugmentationItemSO))
            {
                background.sprite = hasAugBG;
            }
            else if (slot.item.GetType() == typeof(WeaponItemSO))
            {
                background.sprite = hasWepBG;
            }
            else 
            {
                background.sprite = hasItemBG;
            }
            
        }
        else if (slot.item == null && itemDisplay.sprite != null)
        {
            current = null;
            itemDisplay.sprite = current;
            itemDisplay.color = new Color (255, 255, 255, 0);

            background.sprite = defaultBG;
        }
        
        if (slot.amount < 1 && amountDisplay.activeSelf)
        {
            amountDisplay.SetActive(false);
        }
        else if (slot.amount > 1 && !amountDisplay.activeSelf)
        {
            amountDisplay.SetActive(true);
        }

        if (slot.amount != int.Parse(amountText.text))
        {
            amountText.text = slot.amount.ToString();
        }
    }
    
}

public class WeaponSlotElement : MonoBehaviour
{
    // 
}

public class AugSlotElement : MonoBehaviour
{
    // 
}