using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType {
    Inventory,
    Augmentation,
    Weapon
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class ItemInventory : ScriptableObject
{
    public  List<InventorySlot> Container = new List<InventorySlot>();
    public  List<WeaponSlot> WContainer = new List<WeaponSlot>();
    public  List<AugSlot> AContainer = new List<AugSlot>();
    public  Slot tempSlot;
    

    public void MoveSwapCombine(Slot _fromslot, Slot _toslot)
    {
        // facilitates the movement of items between slots
        if (_fromslot != _toslot && _toslot.VerifyItem(_fromslot.item))
        {
            // if items are the same or _toslot is empty, just add item and amount to _toslot
            if (_fromslot.item == _toslot.item || _toslot.item == null)
            {
                Debug.Log("MoveSwapCombine Case 1 - fromSlot: " + _fromslot.item + ", toSlot: " + _toslot.item);
                //_toslot.item = _fromslot.item;
                //_toslot.AddAmount(_fromslot.amount);
                _toslot.AddItemToSlot(_fromslot.item, _fromslot.amount);
                _fromslot.Clear();
                
                
            }
            // if not, swap item and amount data
            else
            {
                Debug.Log("MoveSwapCombine Case 2 - fromSlot: " + _fromslot.item + ", toSlot: " + _toslot.item);
                ItemObject toitem = _toslot.item;
                int toamount = _toslot.amount;
                ItemObject fromitem = _fromslot.item;
                int fromamount = _fromslot.amount;
                
                _toslot.Clear();
                _fromslot.Clear();

                _fromslot.AddItemToSlot(toitem, toamount);
                _toslot.AddItemToSlot(fromitem, fromamount);
                
            }
        }
        else
        {
            Debug.Log("MoveSwapCombine failed");
        }
        
    }
    public void AddItemToInventory(ItemObject _item, int _amount)
    {
        // check if item is already in inventory
        // assume we don't
        bool hasItem = false;
        foreach (InventorySlot slot in Container)
        {
            if(slot.item == _item)
            {
                if (slot.item.stackable)
                {
                    slot.AddAmount(_amount);
                    hasItem = true;
                    //Debug.Log("stacking items");
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        // if after all that business still no item, put it in a new slot
        if (!hasItem)
        {
            foreach (InventorySlot slot in Container)
            {
                if(slot.item == null)
                {
                    slot.AddItemToSlot(_item, _amount);
                    hasItem = true;
                    Debug.Log("item is now in inventory");
                    Print();
                    break;
                }
            }
        }
    }

    public void ClearInventory()
    {
        foreach (InventorySlot slot in Container)
        {
            slot.Clear();
        }
        foreach (AugSlot slot in AContainer)
        {
            slot.Clear();
        }
        foreach (WeaponSlot slot in WContainer)
        {
            slot.Clear();
        }
        Debug.Log("Inventory Cleared");
    }

    public void Print()
    {
        for (int i = 0; i < Container.Count; i++)
        {
            //Debug.Log(i + ": " + Container[i].item + ": " + Container[i].amount);
        }
    }
}

// HERE BE SLOTS, LOTS AND LOTS

[System.Serializable]
public class InventorySlot : Slot
{
    //public ItemObject item;
    public InventorySlot(ItemObject _item, int _amount)
    {
        item = _item;
        amount = _amount;
        //allows = null;
    }
}
[System.Serializable]
public class WeaponSlot : Slot
{
    //public WeaponItemSO item;
    public WeaponType slotType;
    public ItemObject prevItem;
    public WeaponSlot(ItemObject _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public override void AddItemToSlot(ItemObject _item, int _amount)
    {
        if (VerifyItem(_item))
        {
            // does the thing
            if (item == null)
            {
                item = _item;
                AddAmount(_amount);
                Debug.Log("In Add Item To Slot");
                Debug.Log("Slot Type = " + slotType);
                if(slotType == WeaponType.Primary)
                {
                    Debug.Log("In Primary");
                    ReplacePrimary(item.prefabClone);
                }
                else if(slotType == WeaponType.Secondary)
                {
                    Debug.Log("In Secondary");
                    ReplaceSecondary(item.prefabClone);
                }
            }
            /*else if (item == _item && item.stackable)
            {
                AddAmount(_amount);
            }
            else 
            {
                Debug.Log("There's already an item in this slot");
            }*/
        }
        
    }

    public override bool VerifyItem(ItemObject _item)
    {
        // checks item for type, then checks for augType or weaponType
        if (_item.GetType() == typeof(WeaponItemSO))
        {
            if (_item.itemType == Convert.ToInt32(slotType) || slotType == WeaponType.Primary)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public override void Clear()
    {
        // Moves current item to PlayerInventory
        prevItem = item;
        GameObject prev = prevItem.prefabClone;
        if(prev != null)
        {
            prev.transform.parent = PlayerInventory.instance.transform;
            prev.SetActive(false);
        }

        // Clears the slot and resets PlayerWeapon variable
        amount = 0;
        item = null;
        if(slotType == WeaponType.Primary)
        {
            PlayerWeapon.instance.Primary.SetActive(false);
            PlayerWeapon.instance.Primary = null;
        }
        if(slotType == WeaponType.Secondary)
        {
            PlayerWeapon.instance.Secondary.SetActive(false);
            PlayerWeapon.instance.Secondary = null;
        }
    }

    public void ReplacePrimary(GameObject clone)
    {
        Debug.Log("Prefab Clone: " + item.prefabClone);
        Debug.Log("Player weapon instance: " + PlayerWeapon.instance);
        clone.transform.parent = PlayerWeapon.instance.transform;
        clone.transform.position = Vector3.zero;
        clone.transform.localPosition = Vector3.zero;
        //item.prefabClone.transform.rotation = Quaternion.identity;
        clone.transform.rotation = new Quaternion(0, 0, 0, 0);
        PlayerWeapon.instance.Primary = clone;
        PlayerWeapon.instance.TogglePrimary();
    }

    public void ReplaceSecondary(GameObject clone)
    {
        Debug.Log("Prefab Clone: " + item.prefabClone);
        Debug.Log("Player weapon instance: " + PlayerWeapon.instance);
        clone.transform.parent = PlayerWeapon.instance.transform;
        clone.transform.position = Vector3.zero;
        clone.transform.localPosition = Vector3.zero;
        //item.prefabClone.transform.rotation = Quaternion.identity;
        clone.transform.rotation = new Quaternion(0, 0, 0, 0);
        if(PlayerWeapon.instance.Secondary != null)
        {
            PlayerWeapon.instance.Secondary.SetActive(false);
            PlayerWeapon.instance.Secondary.transform.parent = PlayerInventory.instance.transform;
        }
        PlayerWeapon.instance.Secondary = clone;
        PlayerWeapon.instance.ToggleSecondary();
    }
}
[System.Serializable]
public class AugSlot : Slot
{
    //public AugmentationItemSO item;
    public AugType slotType;
    public AugSlot(ItemObject _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    
    public override bool VerifyItem(ItemObject _item)
    {
        // checks item for type, then checks for augType or weaponType
        if (_item.GetType() == typeof(AugmentationItemSO))
        {
            if (slotType == AugType.Aux)
            {
                return true;
            }
            else
            {
                if (_item.itemType == Convert.ToInt32(slotType))
                {
                    return true;
                }
                else
                {
                    Debug.Log("Wrong Item Subtype");
                    return false;
                }
            }
            
        }
        else
        {
            Debug.Log("Wrong Item Type");
            return false;
        }
    }
    public override void AddItemToSlot(ItemObject _Item, int _amount){
        if (VerifyItem(_Item))
        {
            // does the thing
            if (item == null)
            {
                item = _Item;
                AddAmount(_amount);
            }
            else if (item == _Item && item.stackable)
            {
                AddAmount(_amount);
            }
            else 
            {
                Debug.Log("There's already an item in this slot");
            }
        }
        PlayerAugmentations.AugmentationList[_Item.Name] = true;
    }

    public override void Clear(){
        amount = 0;
        PlayerAugmentations.AugmentationList[item.Name] = false;
        item = null;
    }
}

public abstract class Slot
{
    public ItemObject item;
    public int amount;
    public void AddAmount(int value)
    {
        amount += value;
    }

    public virtual void AddItemToSlot(ItemObject _item, int _amount)
    {
        if (VerifyItem(_item))
        {
            // does the thing
            if (item == null)
            {
                item = _item;
                AddAmount(_amount);
            }
            else if (item == _item && item.stackable)
            {
                AddAmount(_amount);
            }
            else 
            {
                Debug.Log("There's already an item in this slot");
            }
        }
        
    }

    public virtual bool VerifyItem(ItemObject _item)
    {
        return true;
    }

    public virtual void Clear()
    {
        // clears the slot
        amount = 0;
        item = null;
    }
    
}

