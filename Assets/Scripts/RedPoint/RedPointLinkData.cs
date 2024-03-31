 using System.Collections.Generic;

 public class RedPointLinkData
 {
     public RedPointLinkData(string _totalNodeName)
     {
         TotalNodeName = _totalNodeName;
         RedPointToolList = new List<RedPointTool>();
         BranchLinkDict = new Dictionary<string, RedPointLinkData>();
     }
     
     /// <summary> 节点总名称 </summary>
     public string TotalNodeName;

     /// <summary> 节点注册的工具列表 </summary>
     public List<RedPointTool> RedPointToolList;
    
     /// <summary> 枝干列表 </summary>
     public Dictionary<string, RedPointLinkData> BranchLinkDict;
 }