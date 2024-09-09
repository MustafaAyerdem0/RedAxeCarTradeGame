using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : DBSyncSynchronizer
{
    public static PlayerData instance;
    public Dictionary<string, int> fieldsQuantity = new Dictionary<string, int>();
    [SyncWithDatabase]
    public int ourMoney;
    [SyncWithDatabase]
    public int Coupe;
    [SyncWithDatabase]
    public int Ctr;
    [SyncWithDatabase]
    public int Jeep;
    [SyncWithDatabase]
    public int M3_E36;
    [SyncWithDatabase]
    public int M3_E46;
    [SyncWithDatabase]
    public int M5_E30;
    [SyncWithDatabase]
    public int Skyline;

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

        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<SyncWithDatabaseAttribute>() != null && field.Name != "ourMoney")
            {
                fieldsQuantity.Add(field.Name, 0);
            }
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
        if (SceneManager.GetActiveScene().name == "Game")
        {

            for (int i = 0; i < 9; i++)
            {
                var item = PlayfabManager.instance.activeInventory.GetItemAt(i);
                if (!item.IsEmpty && item.quantity != 0)
                {
                    string variableName = item.item.name;
                    int quantityValue = item.quantity;
                    fieldsQuantity[variableName] = fieldsQuantity[variableName] + quantityValue;

                }
            }
            SetVariableValue();
        }
        PlayfabManager.instance.SaveData(GetType().Name);
    }

    private void SetVariableValue()
    {
        foreach (var item in fieldsQuantity)
        {
            FieldInfo field = GetType().GetField(item.Key, BindingFlags.Public | BindingFlags.Instance);

            if (field != null && field.Name != "ourMoney")
            {
                field.SetValue(this, item.Value);
            }
            else
            {
                Debug.LogWarning($"Field with name {item.Key} not found in PlayerData.");
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

}