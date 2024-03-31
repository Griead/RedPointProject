using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using UnityEngine;

public class RedPointTest : MonoBehaviour
{
    public void WeaponRedPoint()
    {
        string _redPointNode = "Main_Bag_Weapon";
        
        if(!RedPointManager.Instance.CheckRedPointSaveData(_redPointNode))
            RedPointManager.Instance.AddRedPointSaveData(_redPointNode);
        else
            RedPointManager.Instance.RemoveRedPointSaveData(_redPointNode);
    }

    public void PotionRedPoint()
    {
        string _redPointNode = "Main_Bag_Potion";
        
        if(!RedPointManager.Instance.CheckRedPointSaveData(_redPointNode))
            RedPointManager.Instance.AddRedPointSaveData(_redPointNode);
        else
            RedPointManager.Instance.RemoveRedPointSaveData(_redPointNode);
    }
}
