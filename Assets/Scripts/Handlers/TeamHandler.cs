using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrepRoomHandler : MonoBehaviour
{
    [SerializeField] protected TeamData _teamData; // 0 - Front, 1 - Forward Centre, 2 - Back Centre, 3 - Back
    [SerializeField] protected List<GameObject> UnitSlots = new List<GameObject>();
    protected GameObject _selectedUnitSlot;
    protected Unit SelectedUnit1;
    protected Unit SelectedUnit2;
    protected Unit SelectedUnit3;
    protected Unit SelectedUnit4;
    protected EventSystem _eventSystem;

    void Awake()
    {

    }

    public void SelectSlot(GameObject unitSlot)
    {
        _selectedUnitSlot = unitSlot;
    }

    public void SetUnitToSlot(UnitData unitData)
    {
        int slotIndex = UnitSlots.IndexOf(_selectedUnitSlot);

        if (slotIndex != -1)
        {
            _selectedUnitSlot.GetComponent<Image>().sprite = unitData.CharacterSprite;

            _teamData.UnitsInParty[slotIndex] = unitData.UnitPrefab;
        }
    }
}
