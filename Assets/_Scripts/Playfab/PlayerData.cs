using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : DBSyncSynchronizer
{
    public static PlayerData instance;

    [SyncWithDatabase]
    public int ourMoney;
    [SyncWithDatabase]
    public int carIndex0;
    [SyncWithDatabase]
    public int carIndex1;
    [SyncWithDatabase]
    public int carIndex2;
    [SyncWithDatabase]
    public int carIndex3;
    [SyncWithDatabase]
    public int carIndex4;
    [SyncWithDatabase]
    public int carIndex5;
    [SyncWithDatabase]
    public int carIndex6;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public override void AddDBKeys() // Adding json key to the database with its own Script name to the action of classes with DBSyncSynchronizer superclass
    {
        base.AddDBKeys();
        PlayfabManager.instance.dbKeys[GetType().Name] = this;
    }

    [ContextMenu("SaveData")]
    public override void SaveData()
    {
        base.SaveData();
        PlayfabManager.instance.SaveData(GetType().Name);
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

}