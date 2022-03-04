using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject FireArm;
    public string tag;
    private Gun weapon;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject weaParent = GameObject.Find("WeaponParent");
            GameObject weap = FindDupe(weaParent, tag);

            if (weap != null)
            {
                Destroy(gameObject);
                Debug.Log("GUN_Restocked");
                weapon = weap.GetComponent<Gun>();
                weapon.AmmoFill();
            }
            else {
                Debug.Log("GUN_Acquired");
                Destroy(gameObject);
                GameObject thisFireArm = Instantiate(FireArm) as GameObject;
                thisFireArm.transform.parent = GameObject.Find("WeaponParent").transform;
                thisFireArm.transform.localPosition = new Vector3(0.574f, 0f, 0f);
                thisFireArm.transform.localRotation = Quaternion.identity;
                thisFireArm.SetActive(false);
            }
        }
    }

    public static GameObject FindDupe(GameObject parent, string tag)
    {
        Transform Pt = parent.transform;
        GameObject result;
        for (int i = 0; i < Pt.childCount; i++)
        {
            result = Pt.GetChild(i).gameObject;
            if (result?.tag == tag)
            {
                return result;
            }

        }
        return null;
    }
}
