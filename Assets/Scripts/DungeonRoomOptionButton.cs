using UnityEngine;
using UnityEngine.UI;

public enum ButtonEventType
{
	DungeonRoomOption,
	EquipItem,
	CloseBackpack,
	Back,
	PickedItem
}

public class DungeonRoomOptionButton : MonoBehaviour
{
	public ButtonEventType buttonEventType = ButtonEventType.DungeonRoomOption;
	public int optionIndex = -1;
	public ItemType item;

	public void OnOptionSelected()
	{
		switch (buttonEventType)
		{
			case ButtonEventType.DungeonRoomOption:
				DungeonGenerator.Instance().ExecuteDungeonRoomOption(optionIndex);
				break;
			case ButtonEventType.EquipItem:
				DungeonGenerator.Instance().ExecuteEquipItem();
				break;
			case ButtonEventType.CloseBackpack:
				DungeonGenerator.Instance().ExecuteCloseBackpack();
				break;
			case ButtonEventType.Back:
				DungeonGenerator.Instance().ExecuteBack();
				break;
			case ButtonEventType.PickedItem:
				DungeonGenerator.Instance().ExecutePickedItem(item);
				break;
			default:
				break;
		}
	}

	public void DisableButton()
	{
		GetComponent<Button>().interactable = false;
	}

	public void EnableButton()
	{
		GetComponent<Button>().interactable = true;
	}
}