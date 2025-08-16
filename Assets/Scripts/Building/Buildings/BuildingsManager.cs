using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingsManager : Singleton<BuildingsManager>
{
    public List<BaseBuilding> buildings = new List<BaseBuilding>();
    public Queue<BaseBuilding> buildingsNeedConstruct = new Queue<BaseBuilding>();

    public BaseBuilding GetNeedConstructBuilding()
    {
        if (buildingsNeedConstruct.Count > 0)
        {
            return buildingsNeedConstruct.Dequeue();
        }

        return null;
    }

    public BaseBuilding GetNeedRepairBuilding()
    {
        return buildings.FirstOrDefault(b => b.NeedsRepair);
    }
}
