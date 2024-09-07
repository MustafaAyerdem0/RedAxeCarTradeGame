using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private RectTransform tradeContentPanel;

    [SerializeField]
    private RectTransform tradeOtherContentPanel;

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    [SerializeField]
    private UIInventoryDescription itemDescription;

    [SerializeField]
    private MouseFollower mouseFollower;


    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging;

    public event Action<int, int> OnSwapItems;

    [SerializeField]
    private ItemActionPanel actionPanel;

    private void Awake()
    {
        mouseFollower.Toggle(false);
        itemDescription?.ResetDescription();
    }

    public void InitializeInventoryUI(int inventorysize)
    {
        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            if (i < 9) uiItem.transform.SetParent(contentPanel);
            else if (i < 13) uiItem.transform.SetParent(tradeContentPanel);
            else uiItem.transform.SetParent(tradeOtherContentPanel);
            listOfUIItems.Add(uiItem);
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    internal void ResetAllItems()
    {
        foreach (var item in listOfUIItems)
        {
            item.ResetData();
            item.Deselect();
        }
    }

    internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        itemDescription.SetDescription(itemImage, name, description);
        DeselectAllItems();
        listOfUIItems[itemIndex].Select();
    }

    public void UpdateData(int itemIndex,
        Sprite itemImage, int itemQuantity)
    {
        if (listOfUIItems.Count > itemIndex)
        {
            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            return;
        }
        OnItemActionRequested?.Invoke(index);
    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        ResetDraggedItem();
    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1 || index > 12) return;
        else if ((TradeWindow.instance.ourToggle.isOn || TradeWindow.instance.otherToggle.isOn) && index > 8) return;
        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        HandleItemSelection(inventoryItemUI);
    }

    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1 || index > 8) return;
        currentlyDraggedItemIndex = index;
        HandleItemSelection(inventoryItemUI);
        OnStartDragging?.Invoke(index);
    }

    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }




    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        OnDescriptionRequested?.Invoke(index);

        Debug.Log(inventoryItemUI.name); //!sil
    }

    public void Show()
    {
        Debug.Log("show2");
        gameObject.SetActive(true);
        ResetSelection();
    }

    public void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    public void AddAction(string actionName, Action performAction)
    {
        actionPanel.AddButon(actionName, performAction);
    }

    public void ShowItemAction(int itemIndex)
    {
        actionPanel.Toggle(true);
        actionPanel.transform.position = listOfUIItems[itemIndex].transform.position;
    }

    private void DeselectAllItems()
    {
        foreach (UIInventoryItem item in listOfUIItems)
        {
            item.Deselect();
        }
        actionPanel.Toggle(false);
    }


    public void Hide()
    {
        actionPanel.Toggle(false);
        gameObject.SetActive(false);
        ResetDraggedItem();
    }
}
