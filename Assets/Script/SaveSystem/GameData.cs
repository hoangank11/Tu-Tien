using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    // money
    public int Copper;
    public int LinhThach;
    public int TienThach;
    // list item
    public List<InventoryItem> itemList;
    // ItemSaveID - StackSize
    public SerializableDictionary<string, int> inventory;
    public SerializableDictionary<string, int> storageItems;
    public SerializableDictionary<string, int> storageMaterials;
    // SlotType - ItemSaveID
    public SerializableDictionary<string, ItemType> equipItem;
    // skill
    public SerializableDictionary<string, bool> skillTreeUI;
    public SerializableDictionary<SkillName, string> skillName;
    public int skillPoint;
    // player level / exp
    public float currentExp;
    public LevelType currentLevel;
    public BodyType currentBody;

    // Player Level derived base stats
    public float playerLevelMaxMP;
    public float playerLevelMaxDmg;
    public float playerLevelMaxElementalDmg;

    // Body Level derived base stats
    public float playerBodyMaxHP;
    public float playerBodyMaxArmor;

    // hp
    public float currentHPPercent;
    // mp
    public float currentMPPercent;
    // save checkpoint
    public Vector3 saveCheckPoint;
    public SerializableDictionary<string, bool> unlockCheckPoint;
    // portal
    public SerializableDictionary<string, Vector3> inScenesPortal;
    public string portalDestinationSceneName;
    public bool returnningFromSence;
    // save last scene played
    public string lastScenePlayed;
    public Vector3 lastPlayerPosition;
    // save quest
    public SerializableDictionary<string, bool> completedQuest; // quest done (MainQuest/SideQuest)
    public SerializableDictionary<string, int> activeQuest; // current quest 
    public SerializableDictionary<string, string> dailyQuestNextAvailableTime; // questSaveID - thời điểm (UTC, dạng "o") có thể nhận lại DailyQuest

    public GameData()
    {
        skillPoint = 1000;
        currentHPPercent = 1f;
        currentMPPercent = 1f;
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();
        equipItem = new SerializableDictionary<string, ItemType>();

        skillTreeUI = new SerializableDictionary<string, bool>();
        skillName = new SerializableDictionary<SkillName, string>();
        unlockCheckPoint = new SerializableDictionary<string, bool>();

        inScenesPortal = new SerializableDictionary<string, Vector3>();
        completedQuest = new SerializableDictionary<string, bool>();
        activeQuest = new SerializableDictionary<string, int>();
        dailyQuestNextAvailableTime = new SerializableDictionary<string, string>();
    }
}