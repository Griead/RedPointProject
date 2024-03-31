using System;
using UnityEngine;

public class RedPointTool:MonoBehaviour
{
    public string RedPointNode;

    public RedPointBranchType RedPointBranchType;

    public void Awake()
    {
        //注册数据
        RedPointManager.Instance.RegisterRedPointNode(this);
    }

    public void OnDestroy()
    {
        //移除数据
        RedPointManager.Instance.RemoveRedPointNode(this);
    }
}