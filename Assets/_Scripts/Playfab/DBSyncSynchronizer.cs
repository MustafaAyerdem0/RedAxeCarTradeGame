using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBSyncSynchronizer : MonoBehaviour, IDBSync
{
    private void OnEnable()
    {
        PlayfabManager.dbSyncAction += AddDBKeys;
    }

    private void OnDisable()
    {
        PlayfabManager.dbSyncAction -= AddDBKeys;
    }
    public virtual void AddDBKeys() { }
    public virtual void SaveData() { }
}
