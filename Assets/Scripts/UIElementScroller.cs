using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UIElementType
{
	Button,
	Text
}

public class UIElementScroller : MonoBehaviour
{
	public float textMargin = 30.0f;
	public float buttonMargin = 10.0f;
	public float leftMargin = 100.0f;
	public float itemButtonMargin = 20.0f;
	public float bottomMargin = 50.0f;
	public Canvas uiCanvas = null;
	private LinkedList<GameObject> activeUIElements = new LinkedList<GameObject>();
	private List<DungeonRoomOptionButton> addedButtons = new List<DungeonRoomOptionButton>();
	private float freeCanvasSpace = 0.0f;
	private float canvasHeight = 0.0f;
	
	void Start()
	{
		RectTransform canvasRectTransform = uiCanvas.GetComponent<RectTransform>();
		canvasHeight = canvasRectTransform.rect.height;
		freeCanvasSpace = canvasHeight;
		Debug.Log(freeCanvasSpace.ToString());

	}
	
	public void AddUIElement(GameObject uiElement, UIElementType elementType)
	{
		RectTransform newElementHeightRectTransform = uiElement.GetComponentInChildren<TMP_Text>().gameObject.GetComponent<RectTransform>();
		float height = LayoutUtility.GetPreferredHeight(newElementHeightRectTransform);
		
		float offset = height + (elementType == UIElementType.Button ? buttonMargin : textMargin);

		if (elementType == UIElementType.Button)
		{
			addedButtons.Add(uiElement.GetComponent<DungeonRoomOptionButton>());
		}

		RectTransform newElementOffsetRectTransform = uiElement.GetComponent<RectTransform>();
		Vector3 newElementPosition = newElementOffsetRectTransform.anchoredPosition;
		newElementPosition.x = leftMargin;
		newElementPosition.y = canvasHeight * -0.5f + offset + bottomMargin;
		newElementOffsetRectTransform.anchoredPosition = newElementPosition;

		ScrollElements(offset);
		activeUIElements.AddLast(uiElement);
	}
	
	public void AddUIElementsSameLine(List<GameObject> uiElements, UIElementType elementType)
	{
		if (uiElements.Count == 0)
			return;
		
		RectTransform newElementHeightRectTransform = uiElements[0].GetComponentInChildren<TMP_Text>().gameObject.GetComponent<RectTransform>();
		float height = LayoutUtility.GetPreferredHeight(newElementHeightRectTransform);
		float offset = height + (elementType == UIElementType.Button ? buttonMargin : textMargin);

		ScrollElements(offset);

		float horizontalOffset = leftMargin;
		for (int i = 0; i < uiElements.Count; ++i)
		{
			if (elementType == UIElementType.Button)
			{
				addedButtons.Add(uiElements[i].GetComponent<DungeonRoomOptionButton>());
			}
			
			RectTransform newElementOffsetRectTransform = uiElements[i].GetComponent<RectTransform>();
			Vector3 newElementPosition = newElementOffsetRectTransform.anchoredPosition;
			newElementPosition.x = horizontalOffset;
			newElementPosition.y = canvasHeight * -0.5f + offset + bottomMargin;
			newElementOffsetRectTransform.anchoredPosition = newElementPosition;

			uiElements[i].GetComponent<DynamicButtonWidth>().InitWidth();
			horizontalOffset += newElementOffsetRectTransform.rect.width + itemButtonMargin;

			activeUIElements.AddLast(uiElements[i]);
		}
	}

	public void DisableAddedButtons()
	{
		foreach(DungeonRoomOptionButton button in addedButtons)
		{
			button.DisableButton();
		}
	}

	public void ScrollElements(float offset, bool reduceSpace = true)
	{
		if (reduceSpace)
		{
			freeCanvasSpace -= offset;
		}
		
		foreach (GameObject activeUIElement in activeUIElements)
		{
			RectTransform activeElementRectTransform = activeUIElement.GetComponent<RectTransform>();
			Vector3 activeElementPosition = activeElementRectTransform.localPosition;
			activeElementPosition.y += offset;
			activeElementRectTransform.localPosition = activeElementPosition;
		}
	}
}