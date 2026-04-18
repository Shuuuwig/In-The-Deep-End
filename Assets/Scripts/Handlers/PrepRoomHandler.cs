using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrepRoomHandler : MonoBehaviour
{
    [SerializeField] protected TeamData _teamData;
    [SerializeField] protected List<GameObject> _positionSlots = new List<GameObject>();
    [SerializeField] protected List<Button> _unitSelections = new List<Button>();
    [SerializeField] protected Button _continueButton;
    [SerializeField] protected Sprite _emptySlotSprite;

    protected GameObject _selectedUnitSlot;
    protected EventSystem _eventSystem;

    void Awake()
    {
        if (_eventSystem == null)
        {
            _eventSystem = EventSystem.current;
        }

        while (_teamData.UnitsInParty.Count < _positionSlots.Count)
        {
            _teamData.UnitsInParty.Add(null);
        }

        CheckMinimumTeamCount();
    }

    public void SelectSlot(GameObject unitSlot)
    {
        _selectedUnitSlot = unitSlot;
        RefreshUnitSelectionButtons();
    }

    private void RefreshUnitSelectionButtons()
    {
        if (_selectedUnitSlot == null)
        {
            return;
        }

        int currentSlotIndex = _positionSlots.IndexOf(_selectedUnitSlot);

        foreach (GameObject slot in _positionSlots)
        {
            if (slot.TryGetComponent(out Button slotButton))
            {
                slotButton.interactable = false;
            }
        }

        Button firstAvailableButton = null;

        foreach (Button button in _unitSelections)
        {
            UnitButtonInfo info = button.GetComponent<UnitButtonInfo>();
            if (info != null)
            {
                bool isInThisSlot = _teamData.UnitsInParty[currentSlotIndex] == info.Data.UnitPrefab;
                button.interactable = !isInThisSlot;

                if (button.interactable && firstAvailableButton == null)
                {
                    firstAvailableButton = button;
                }
            }
        }

        if (firstAvailableButton != null && _eventSystem != null)
        {
            _eventSystem.SetSelectedGameObject(firstAvailableButton.gameObject);
        }
    }

    public void SetUnitToSlot(UnitData unitData)
    {
        if (_selectedUnitSlot == null)
        {
            return;
        }

        int newSlotIndex = _positionSlots.IndexOf(_selectedUnitSlot);
        int existingSlotIndex = _teamData.UnitsInParty.IndexOf(unitData.UnitPrefab);

        if (existingSlotIndex != -1 && existingSlotIndex != newSlotIndex)
        {
            _teamData.UnitsInParty[existingSlotIndex] = null;

            Transform oldPortraitTransform = _positionSlots[existingSlotIndex].transform.Find("UnitPortrait");
            if (oldPortraitTransform != null)
            {
                Image oldSlotImage = oldPortraitTransform.GetComponent<Image>();
                if (oldSlotImage != null)
                {
                    oldSlotImage.sprite = _emptySlotSprite;
                }
            }
        }

        Transform newPortraitTransform = _selectedUnitSlot.transform.Find("UnitPortrait");
        if (newPortraitTransform != null)
        {
            Image characterImage = newPortraitTransform.GetComponent<Image>();
            if (characterImage != null)
            {
                characterImage.sprite = unitData.CharacterSprite;
                characterImage.enabled = true;
            }
        }

        _teamData.UnitsInParty[newSlotIndex] = unitData.UnitPrefab;

        foreach (Button unitSelection in _unitSelections)
        {
            unitSelection.interactable = false;
        }

        CheckMinimumTeamCount();
        _eventSystem.SetSelectedGameObject(_selectedUnitSlot);
        _selectedUnitSlot = null;

        foreach (GameObject slot in _positionSlots)
        {
            if (slot.TryGetComponent(out Button slotButton))
            {
                slotButton.interactable = true;
            }
        }
    }

    protected void CheckMinimumTeamCount()
    {
        bool hasAtLeastOneUnit = false;
        foreach (var unit in _teamData.UnitsInParty)
        {
            if (unit != null)
            {
                hasAtLeastOneUnit = true;
                break;
            }
        }
        _continueButton.interactable = hasAtLeastOneUnit;
    }

    public void SaveTeam()
    {
        _teamData.SaveTeam();
    }
}