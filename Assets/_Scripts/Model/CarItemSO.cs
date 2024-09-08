using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CarItemSO : ItemSO, IDestroyableItem, IItemAction
{
    public string ActionName => "Drive Car";

    public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
    {
        return true;
    }
}

public interface IDestroyableItem
{

}

public interface IItemAction
{
    public string ActionName { get; }
    bool PerformAction(GameObject character, List<ItemParameter> itemState);
}