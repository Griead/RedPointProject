using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 字典管理器 通过链表的形式管理红点节点数据 通过枚举的方式判断是哪条红点链
/// </summary>
public class RedPointManager
{
    private static RedPointManager instance;

    public static RedPointManager Instance
    {
        get
        {
            if (instance is null)
                instance = new RedPointManager();
            return instance;
        }
    }


    #region 注册节点管理
    
    /// <summary> 链父节点字典 </summary>
    private Dictionary<RedPointBranchType, RedPointLinkData> ParentLinkDataDict = new Dictionary<RedPointBranchType, RedPointLinkData>();
    
    /// <summary>
    /// 注册红点节点
    /// </summary>
    /// <param name="_redPointTool"></param>
    public void RegisterRedPointNode(RedPointTool _redPointTool)
    {
        var result = SplitRedPointNodeString(_redPointTool.RedPointNode);
        
        //将红点节点注册到树内
        if (!ParentLinkDataDict.TryGetValue(_redPointTool.RedPointBranchType, out var _redPointLinkData))
        {
            _redPointLinkData = new RedPointLinkData(result[0]);
            ParentLinkDataDict.Add(_redPointTool.RedPointBranchType, _redPointLinkData);
        }
        //递归注册红点
        RecursionRegisterRedPoint(_redPointLinkData, result, 0, _redPointTool);
    }
    
    /// <summary>
    /// 注册红点
    /// </summary>
    private void RecursionRegisterRedPoint(RedPointLinkData _redPointLinkData, string[] _redPointNodeTotalName, int _layer, RedPointTool _redPointTool)
    {
        //获取当前层的总名称
        StringBuilder _curLayerNameBuilder = new StringBuilder();
        for (int i = 0; i <= _layer; i++)
        {
            if(i != 0)
                _curLayerNameBuilder.Append("_");
            
            _curLayerNameBuilder.Append(_redPointNodeTotalName[i]);
        }
        string _curLayerName = _curLayerNameBuilder.ToString();
        
        //是否存在该节点
        if (_redPointLinkData.TotalNodeName == _curLayerName)
        {
            //是否还存在枝干
            if (_layer < _redPointNodeTotalName.Length - 1)
            {
                StringBuilder _redPointBranchNodeBuilder = _curLayerNameBuilder.Append('_').Append(_redPointNodeTotalName[_layer + 1]);
                string _redPointBranchNode = _redPointBranchNodeBuilder.ToString();
            
                //如果不存在该叶子节点 则创建一个出来
                if (!_redPointLinkData.BranchLinkDict.TryGetValue(_redPointBranchNode, out var _branchRadPointData))
                {
                    _branchRadPointData = new RedPointLinkData(_redPointBranchNode);
                    _redPointLinkData.BranchLinkDict.Add(_redPointBranchNode, _branchRadPointData);
                }
            
                //继续向下遍历
                RecursionRegisterRedPoint(_branchRadPointData, _redPointNodeTotalName, _layer + 1, _redPointTool);
            }
            else
            {
                //找到最终的枝干
                _redPointLinkData.RedPointToolList.Add(_redPointTool);
            }
            
            //激活所有链接节点
            bool _shouldShow = CheckNodeState(_curLayerName);
            int _redPointToolCount = _redPointLinkData.RedPointToolList?.Count ?? 0;
            for (int i = 0; i < _redPointToolCount; i++)
            {
                _redPointLinkData.RedPointToolList[i].gameObject.SetActive(_shouldShow);
            }
        }
        else
        {
            Debug.LogError($"节点异常: 节点名称对应不上 树{_redPointLinkData.TotalNodeName}!=注册名称{_curLayerNameBuilder}");
        }
    }
    
    /// <summary>
    /// 移除红点工具
    /// </summary>
    public void RemoveRedPointNode(RedPointTool _redPointTool)
    {       
        var result = SplitRedPointNodeString(_redPointTool.RedPointNode);
        //将红点节点注册到树内
        if (ParentLinkDataDict.TryGetValue(_redPointTool.RedPointBranchType, out var _redPointLinkData))
        {
            //递归注册红点
            RecursionRemoveRedPoint(_redPointLinkData, result, 0, _redPointTool);
        }
    }
    
    /// <summary>
    /// 移除红点工具
    /// 通过回溯枝干节点 然后再判断当前枝干节点是否需要移除
    /// </summary>
    private void RecursionRemoveRedPoint(RedPointLinkData _redPointLinkData, string[] _redPointNodeTotalaName, int _layer, RedPointTool _redPointTool)
    {
        //获取当前层的总名称
        StringBuilder _curLayerNameBuilder = new StringBuilder();
        for (int i = 0; i <= _layer; i++)
        {
            if(i != 0)
                _curLayerNameBuilder.Append("_");
            
            _curLayerNameBuilder.Append(_redPointNodeTotalaName[i]);
        }
        string _curLayerName = _curLayerNameBuilder.ToString();

        if (_redPointLinkData.TotalNodeName == _curLayerName)
        {
            //首先 判断当前节点是否是最后的节点
            if (_layer < _redPointNodeTotalaName.Length - 1)
            {
                StringBuilder _redPointBranchNodeBuilder = _curLayerNameBuilder.Append('_').Append(_redPointNodeTotalaName[_layer + 1]);
                string _redPointBranchNode = _redPointBranchNodeBuilder.ToString();

                if (_redPointLinkData.BranchLinkDict.TryGetValue(_redPointBranchNode, out var _redPointBranchData))
                {
                    RecursionRemoveRedPoint(_redPointBranchData, _redPointNodeTotalaName, _layer + 1, _redPointTool);

                    if (_redPointBranchData.BranchLinkDict.Count <= 0 &&_redPointBranchData.RedPointToolList.Count <= 0)
                        _redPointLinkData.BranchLinkDict.Remove(_redPointBranchNode);
                }
            }
            else
            {
                //最后的节点移除
                _redPointLinkData.RedPointToolList.Remove(_redPointTool);
            }
        }
        else
        {
            Debug.LogError($"节点异常: 节点名称对应不上 树{_redPointLinkData.TotalNodeName}!=注册名称{_curLayerNameBuilder}");
        }
    }

    #endregion

    #region 红点数据管理
    
    private Dictionary<string, int> RedPointSaveDataDict = new Dictionary<string, int>();

    public bool CheckRedPointSaveData(string _redPointNode)
    {
        return RedPointSaveDataDict.ContainsKey(_redPointNode);
    }

    public void AddRedPointSaveData(string _redPointNode)
    {
        if (RedPointSaveDataDict.ContainsKey(_redPointNode))
        {
            Debug.Log("请勿重复注册节点");
            return;
        }

        string[] _splitStringArray = SplitRedPointNodeString(_redPointNode);

        StringBuilder _sb = new StringBuilder();
        int _arrayLength = _splitStringArray?.Length ?? 0;
        for (int i = 0; i < _arrayLength; i++)
        {
            if (i != 0)
                _sb.Append("_");

            _sb.Append(_splitStringArray[i]);
            string _curNodeName = _sb.ToString();
            if (RedPointSaveDataDict.ContainsKey(_curNodeName))
                RedPointSaveDataDict[_curNodeName]++;
            else
                RedPointSaveDataDict.Add(_curNodeName, 1);
        }
        
        SetRedPointNode(_redPointNode);
    }

    public void RemoveRedPointSaveData(string _redPointNode)
    {
        if (!RedPointSaveDataDict.ContainsKey(_redPointNode))
        {
            Debug.Log("存储数据中不存在该节点");
            return;
        } 
        
        string[] _splitStringArray = SplitRedPointNodeString(_redPointNode);
        
        StringBuilder _sb = new StringBuilder();
        int _arrayLength = _splitStringArray?.Length ?? 0;
        for (int i = 0; i < _arrayLength; i++)
        {
            if (i != 0)
                _sb.Append("_");

            _sb.Append(_splitStringArray[i]);
            string _curNodeName = _sb.ToString();
            if (RedPointSaveDataDict.ContainsKey(_curNodeName))
            {
                RedPointSaveDataDict[_curNodeName]--;
                if (RedPointSaveDataDict[_curNodeName] <= 0)
                    RedPointSaveDataDict.Remove(_curNodeName);
            }
            else
                Debug.LogError("错误 不存在该节点");
        }
        
        SetRedPointNode(_redPointNode);
    }

    #endregion
    
    
    public void SetRedPointNode(string _redPointNodeName)
    {
        string[] _split = SplitRedPointNodeString(_redPointNodeName);
        string _baseNodeName = _split[0]; 
        
        foreach (var _redPointBranchType in ParentLinkDataDict.Keys)
        {
            if (ParentLinkDataDict[_redPointBranchType].TotalNodeName == _baseNodeName)
            {
                //重置节点的状态
                RecursionSetRedPointNode(ParentLinkDataDict[_redPointBranchType], _split, 0);
            }
        }
    }
    
    /// <summary>
    /// 递归设置红点状态
    /// </summary>
    /// <param name="_redPointLinkData"></param>
    /// <param name="_redPointNodeName"></param>
    public void RecursionSetRedPointNode(RedPointLinkData _redPointLinkData, string[] _redPointNodeName, int _layer)
    {
        if (_layer < _redPointNodeName.Length - 1)
        {
            StringBuilder _sb = new StringBuilder();
            for (int i = 0; i <= _layer; i++)
            {
                if (i != 0)
                    _sb.Append("_");
            
                _sb.Append(_redPointNodeName[i]);
            }

            string _curBranchNodeName = _sb.Append("_").Append(_redPointNodeName[_layer + 1]).ToString();
            foreach (var _tempBranchNodeName in _redPointLinkData.BranchLinkDict.Keys)
            {
                if (_tempBranchNodeName == _curBranchNodeName)
                    RecursionSetRedPointNode(_redPointLinkData.BranchLinkDict[_tempBranchNodeName], _redPointNodeName, _layer + 1);
            }
        }
        
        bool _nodeState = CheckNodeState(_redPointLinkData.TotalNodeName);
        int _redPointToolListCount = _redPointLinkData.RedPointToolList?.Count ?? 0;
        for (int i = 0; i < _redPointToolListCount; i++)
            _redPointLinkData.RedPointToolList[i].gameObject.SetActive(_nodeState);
    }
    
    /// <summary>
    /// 检测节点状态
    /// </summary>
    private bool CheckNodeState(string _redPointNode)
    {
        return RedPointSaveDataDict.ContainsKey(_redPointNode);
    }
    
    /// <summary>
    /// 拆解红点节点
    /// </summary>
    /// <param name="_redPointName"></param>
    /// <returns></returns>
    private string[] SplitRedPointNodeString(string _redPointName)
    {
        string[] result = _redPointName.Split('_');
        
        return result;
    }
}
