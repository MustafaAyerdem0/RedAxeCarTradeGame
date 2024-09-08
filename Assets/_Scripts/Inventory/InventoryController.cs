// using Inventory.Model;
// using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public EdibleItemSO[] edibleItemSos;

    public static InventoryController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        PrepareUI();
        PrepareInventoryData();
    }


    private void OnEnable()
    {
        inventoryData.OnTradeItemReplicated += HandleReplicateTradeItem;
    }

    private void OnDisable()
    {
        inventoryData.OnTradeItemReplicated -= HandleReplicateTradeItem;
    }

    private void HandleReplicateTradeItem(int itemIndex)
    {
        int edibleItemSoIndex = System.Array.IndexOf(edibleItemSos, inventoryData.inventoryItems[itemIndex].item);
        TradeRequest localTradeRequest = RCC_PhotonDemo.instance.ourPlayer.GetComponent<TradeRequest>();
        if (localTradeRequest.targetPhotonView)
            photonView.RPC("ReplicateTradeItemRPC", localTradeRequest.targetPhotonView.Owner, itemIndex, edibleItemSoIndex, inventoryData.inventoryItems[itemIndex].quantity);
    }

    [PunRPC]
    public void ReplicateTradeItemRPC(int itemIndex, int edibleItemSoIndex, int quantity)
    {
        InventoryItem inventoryItem = inventoryData.inventoryItems[itemIndex];
        inventoryData.AddItem(edibleItemSos[edibleItemSoIndex], quantity, null, true);
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
        if (inventoryItem.IsEmpty || itemIndex > 8)
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
            inventoryUI.ResetSelection();
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

    public void CheckHaveCarInInventory()
    {
        for (int i = 0; i < 9; i++)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(i);
            if (!inventoryItem.IsEmpty && inventoryItem.item.carIndex == RCC_PhotonDemo.instance.selectedCarIndex) return;
        }

        if (RCC_SceneManager.Instance.activePlayerVehicle)
        {
            DriveCar driveCar = RCC_PhotonDemo.instance.ourPlayer.GetComponent<DriveCar>();
            if (driveCar.inCar) driveCar.GetOutCar();
            PhotonNetwork.Destroy(RCC_SceneManager.Instance.activePlayerVehicle.gameObject);
        }

    }

    private void HandleDragging(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        inventoryData.SwapItems(itemIndex_1, itemIndex_2);
    }



    public void ExitTrade()
    {
        TradeRequest localTradeRequest = RCC_PhotonDemo.instance.ourPlayer.GetComponent<TradeRequest>();
        if (localTradeRequest.targetPhotonView)
            photonView.RPC("ExitTradeRPC", localTradeRequest.targetPhotonView.Owner);
        ExitTradeRPC();
    }
    [PunRPC]
    public void ExitTradeRPC()
    {
        if (TradeWindow.instance.ourToggle.isOn && TradeWindow.instance.otherToggle.isOn)
        {
            OnTradeAcceptedOrRejected(9, 13, false);
            OnTradeAcceptedOrRejected(13, 17, true);
            CheckHaveCarInInventory();
        }
        else
        {
            OnTradeAcceptedOrRejected(9, 13, true);
            OnTradeAcceptedOrRejected(13, 17, false);
        }

        tradeWindow.SetActive(false);
    }

    public void OnTradeAcceptedOrRejected(int startIndex, int finishIndex, bool isAdding)
    {
        for (int i = startIndex; i < finishIndex; i++)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(i);
            if (inventoryItem.IsEmpty) continue;
            DropItem(i, inventoryItem.quantity);
            if (isAdding) inventoryData.AddItem(inventoryItem);
        }

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

    public void ShowInventory()
    {
        inventoryUI.Show();
        foreach (var item in inventoryData.GetCurrentInventoryState())
        {
            inventoryUI.UpdateData(item.Key,
                item.Value.item.ItemImage,
                item.Value.quantity);
        }

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isTradePage)
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                ShowInventory();
            }
            else if (!tradeWindow.activeSelf)
            {
                inventoryUI.Hide();
            }

        }
    }
}
