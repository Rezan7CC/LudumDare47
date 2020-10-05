using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	Invalid,
	Torch,
	RoundGem,
	RopeLadder,
	RustyKey,
	Dagger,
	WoodenClub,
	SkeletonBones,
	WaterFlask,
	DogFood
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DungeonRoom", order = 1)]
public class DungeonRoom : ScriptableObject
{
	public TextAsset roomText;
	public string roomID;

	[System.Serializable]
	public struct Option
	{
		public string optionName;
		public ItemType requiredItem;
		public ItemType itemReward;
		public TextAsset completedText;
		public string nextRoomID;
	}

	public List<Option> dungeonRoomOptions;
}