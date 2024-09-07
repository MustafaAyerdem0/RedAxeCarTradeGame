// using Inventory.Model;
// using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Inventory.Model;
using Photon.Pun;
using UnityEngine;


public class InventoryController : MonoBehaviourPun
{
    [SerializeField]
    private UIInventoryPage inventoryUI;

    [SerializeField]
    private GameObject tradeWindow;

    [SerializeField]
    private InventorySO inventoryData;

    public List<InventoryItem> initialItems = new List<InventoryItem>();

    [SerializeField]
    private AudioClip dropClip;

    [SerializeField]
    private AudioSource audioSource;

    public bool isTradePage;

    private void Start()
    {
        PrepareUI();
        PrepareInventoryData();
    }

    private void PrepareInventoryData()
    {
        inventoryData.Initialize();
        inventoryData.OnInventoryUpdated += UpdateInventoryUI;
        foreach (InventoryItem item in initialItems)
        {
            if (item.IsEmpty)
                continue;
            inventoryData.AddItem(item);
        }
    }

    private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
    {
        inventoryUI.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                item.Value.quantity);
        }
    }

    private void PrepareUI()
    {
        inventoryUI.InitializeInventoryUI(inventoryData.Size); //! bunu koy
        inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        inventoryUI.OnSwapItems += HandleSwapItems;
        inventoryUI.OnStartDragging += HandleDragging;
        inventoryUI.OnItemActionRequested += HandleItemActionRequest;
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;

        IItemAction itemAction = inventoryItem.item as IItemAction;
        if (itemAction != null)
        {
            inventoryUI.ShowItemAction(itemIndex);
            inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            if (inventoryItem.quantity > 1) inventoryUI.AddAction("Split", () => SplitItem(itemIndex));
        }

        IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
        if (destroyableItem != null)
        {
            inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
        }

    }

    public void SplitItem(int itemIndex)
    {
        inventoryData.SplitItem(itemIndex);
        inventoryUI.ResetSelection();
    }

    private void DropItem(int itemIndex, int quantity)
    {
        inventoryData.RemoveItem(itemIndex, quantity);
        inventoryUI.ResetSelection();
        //audioSource.PlayOneShot(dropClip);
    }

    public void PerformAction(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;

        IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
        if (destroyableItem != null)
        {
            //! default    inventoryData.RemoveItem(itemIndex, 1);
            RCC_PhotonDemo.instance.selectedCarIndex = inventoryItem.item.carIndex;
            RCC_PhotonDemo.instance.Spawn();
        }

        IItemAction itemAction = inventoryItem.item as IItemAction;
        if (itemAction != null)
        {
            itemAction.PerformAction(gameObject, inventoryItem.itemState);
            //audioSource.PlayOneShot(itemAction.actionSFX);
            if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                inventoryUI.ResetSelection();
        }
    }

    private void HandleDragging(int itemIndex)
    {
        Debug.LogError("test1");
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        Debug.LogError("test2");

    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        inventoryData.SwapItems(itemIndex_1, itemIndex_2);
    }



    public void ExitTrade()
    {
        photonView.RPC("ExitTradeRPC", RCC_PhotonDemo.instance.ourPlayer.GetComponent<TradeRequest>().targetPhotonView.Owner);
        ExitTradeRPC();
    }
    [PunRPC]
    public void ExitTradeRPC()
    {
        for (int i = 9; i < 13; i++)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(i);
            if (inventoryItem.IsEmpty) continue;
            Debug.LogError("exitTrade");
            DropItem(i, inventoryItem.quantity);
            inventoryData.AddItem(inventoryItem);
        }
        tradeWindow.SetActive(false);

    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }
        ItemSO item = inventoryItem.item;
        string description = PrepareDescription(inventoryItem);
        inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
            item.name, description);
    }

    private string PrepareDescription(InventoryItem inventoryItem)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(inventoryItem.item.Description);
        sb.AppendLine();
        sb.AppendLine();
        for (int i = 0; i < inventoryItem.itemState.Count; i++)
        {
            sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                $": {inventoryItem.itemState[i].value}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isTradePage)
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                Debug.Log("show");
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else if (!tradeWindow.activeSelf)
            {
                inventoryUI.Hide();
            }

        }
    }
}
