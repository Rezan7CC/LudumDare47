using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory
{
    public HashSet<ItemType> availableItems = new HashSet<ItemType>();
    public ItemType equippedItem = ItemType.Invalid;
}

public class DungeonGenerator : MonoBehaviour
{
    public List<DungeonRoom> availableDungeonRooms;
    public string startRoomID = "OutsideDungeon";
    public Canvas uiCanvas;
    public GameObject textPrefab;
    public GameObject optionPrefab;

    private Inventory inventory = new Inventory();
    private DungeonRoom currentRoom = null;
    private UIElementScroller elementScroller = null;
    private static DungeonGenerator instance = null;

    public static DungeonGenerator Instance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        elementScroller = uiCanvas.GetComponent<UIElementScroller>();
        if (!elementScroller)
        {
            Debug.LogError("UIElementScroller not found on canvas!");
        }
    }
	
    void Start()
    {
        StartDungeonRoom(startRoomID);
    }

    void StartDungeonRoom(string roomID)
    {
        currentRoom = FindDungeonRoom(roomID);
        if (currentRoom == null)
        {
            Debug.LogError("Dungeon room with ID " + roomID + " not found!");
            return;
        }

        AddTextElement(currentRoom.roomText.text);

        AddDungeonRoomOptions();
    }

    private void AddDungeonRoomOptions()
    {
        for (int i = 0; i < currentRoom.dungeonRoomOptions.Count; ++i)
        {
            DungeonRoom.Option option = currentRoom.dungeonRoomOptions[i];

            if (option.requiredItem != ItemType.Invalid && inventory.equippedItem != option.requiredItem)
            {
                continue;
            }
            
            GameObject optionButton = AddButtonElement(option.optionName);
            optionButton.GetComponentInChildren<DungeonRoomOptionButton>().optionIndex = i;
        }
    }

    public void ExecuteDungeonRoomOption(int index)
    {
        if (index == -1)
        {
            Debug.LogError("Dungeon room option with index -1 selected.");
        }
		
        elementScroller.ScrollElements(80.0f);
        elementScroller.DisableAddedButtons();
        
        AddTextElement(currentRoom.dungeonRoomOptions[index].completedText.text);

        ItemType itemReward = currentRoom.dungeonRoomOptions[index].itemReward;
        if (itemReward != ItemType.Invalid && inventory != null && !inventory.availableItems.Contains(itemReward))
        {
            inventory.availableItems.Add(itemReward);
        }
        
        string nextRoomID = currentRoom.dungeonRoomOptions[index].nextRoomID;
        if(nextRoomID == "")
            return;
        
        StartDungeonRoom(nextRoomID);
    }

    public void ExecuteEquipItem()
    {
        elementScroller.ScrollElements(80.0f);
        elementScroller.DisableAddedButtons();
        AddTextElement("Which item to equip?");
        
        List<GameObject> itemButtons = new List<GameObject>();

        foreach(ItemType item in inventory.availableItems)
        {
            GameObject itemGO = Instantiate(optionPrefab, uiCanvas.transform);
            itemButtons.Add(itemGO);

            DungeonRoomOptionButton buttonComp = itemGO.GetComponent<DungeonRoomOptionButton>();
            buttonComp.buttonEventType = ButtonEventType.PickedItem;
            buttonComp.item = item;
            
            string buttonText = SplitCamelCase(item.ToString());
            if (item == inventory.equippedItem)
            {
                buttonText += " (equipped)";
                buttonComp.DisableButton();
            }
            
            itemGO.GetComponentInChildren<TMP_Text>().text = buttonText;
        }

        elementScroller.AddUIElementsSameLine(itemButtons, UIElementType.Button);

        GameObject closeBackpackButtonGO = AddButtonElement("Close backpack");
        closeBackpackButtonGO.GetComponent<DungeonRoomOptionButton>().buttonEventType = ButtonEventType.CloseBackpack;
    }

    public void ExecuteCloseBackpack()
    {
        elementScroller.ScrollElements(80.0f);
        elementScroller.DisableAddedButtons();
        
        AddTextElement("You closed your backpack.");
        AddDungeonRoomOptions();
        OpenBackpackButton.Instance().EnableButton();
    }

    public void ExecuteBack()
    {
        
    }

    public void ExecutePickedItem(ItemType item)
    {
        inventory.equippedItem = item;
        
        elementScroller.ScrollElements(80.0f);
        elementScroller.DisableAddedButtons();
        
        AddTextElement("Equipped " + SplitCamelCase(item.ToString()) + ".");
        AddDungeonRoomOptions();
        OpenBackpackButton.Instance().EnableButton();
    }
    
    public static string SplitCamelCase( string str )
    {
        return Regex.Replace( 
            Regex.Replace( 
                str, 
                @"(\P{Ll})(\P{Ll}\p{Ll})", 
                "$1 $2" 
            ), 
            @"(\p{Ll})(\P{Ll})", 
            "$1 $2" 
        );
    }
    
    public void OpenBackpack()
    {
        elementScroller.ScrollElements(80.0f);
        elementScroller.DisableAddedButtons();
        
        string openBackpackText = "You opened your backpack but there is nothing in there.";

        int itemCount = inventory.availableItems.Count;
        if (itemCount > 0)
        {
            openBackpackText = "In your backpack you can find: ";

            int iteratedCount = 0;
            foreach (ItemType item in inventory.availableItems)
            {
                openBackpackText += SplitCamelCase(item.ToString());
                if (inventory.equippedItem == item)
                {
                    openBackpackText += " (equipped)";
                }
                
                ++iteratedCount;

                if (iteratedCount < itemCount)
                {
                    openBackpackText += " | ";
                }
            }
        }

        AddTextElement(openBackpackText);
        if (itemCount > 0)
        {
            GameObject equipButtonGO = AddButtonElement("Equip item");
            equipButtonGO.GetComponent<DungeonRoomOptionButton>().buttonEventType = ButtonEventType.EquipItem;
        }

        GameObject closeBackpackButtonGO = AddButtonElement("Close backpack");
        closeBackpackButtonGO.GetComponent<DungeonRoomOptionButton>().buttonEventType = ButtonEventType.CloseBackpack;
        
        OpenBackpackButton.Instance().DisableButton();
    }

    private void AddTextElement(string text)
    {
        GameObject textGO = Instantiate(textPrefab, uiCanvas.transform);
        textGO.GetComponentInChildren<TMP_Text>().text = text;
        elementScroller.AddUIElement(textGO, UIElementType.Text);
    }

    private GameObject AddButtonElement(string text)
    {
        GameObject optionButton = Instantiate(optionPrefab, uiCanvas.transform);
        optionButton.GetComponentInChildren<TMP_Text>().text = text;
        elementScroller.AddUIElement(optionButton, UIElementType.Button);
        return optionButton;
    }

    DungeonRoom FindDungeonRoom(string roomID)
    {
        foreach (DungeonRoom dungeonRoom in availableDungeonRooms)
        {
            if (dungeonRoom.roomID == roomID)
                return dungeonRoom;
        }

        return null;
    }
}