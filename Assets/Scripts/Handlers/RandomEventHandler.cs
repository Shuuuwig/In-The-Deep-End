using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RandomEventHandler : MonoBehaviour
{
    [SerializeField] TeamData _teamData;
    [SerializeField] MapData _mapData;
    [SerializeReference] List<RandomEvent> _randomEvents = new List<RandomEvent>();

    [SerializeField] Image _eventImageDisplay;
    [SerializeField] TMP_Text _eventDialogue;
    [SerializeField] Button _option1Button;
    [SerializeField] Button _option2Button;
    [SerializeField] Button _option3Button;
    [SerializeField] Button _continueButton;
    [SerializeField] EventSystem _eventSystem;

    RandomEvent _chosenEvent;

    void Awake()
    {
        if (_eventSystem == null)
        {
            _eventSystem = EventSystem.current;
        }

        SetRandomEvent();
    }

    void SetRandomEvent()
    {
        if (_randomEvents.Count == 0) return;

        int randomNumber = Random.Range(0, _randomEvents.Count);
        _chosenEvent = _randomEvents[randomNumber];

        _chosenEvent.EventInit();

        _option1Button.GetComponentInChildren<TMP_Text>().text = _chosenEvent.Option1Text;
        _option2Button.GetComponentInChildren<TMP_Text>().text = _chosenEvent.Option2Text;
        _option3Button.GetComponentInChildren<TMP_Text>().text = _chosenEvent.Option3Text;

        _option1Button.onClick.RemoveAllListeners();
        _option2Button.onClick.RemoveAllListeners();
        _option3Button.onClick.RemoveAllListeners();

        _option1Button.onClick.AddListener(() => { _chosenEvent.Option1Result(_teamData); });
        _option2Button.onClick.AddListener(() => { _chosenEvent.Option2Result(_teamData); });
        _option3Button.onClick.AddListener(() => { _chosenEvent.Option3Result(_teamData); });

        _option1Button.onClick.AddListener(() => { EventResult(); });
        _option2Button.onClick.AddListener(() => { EventResult(); });
        _option3Button.onClick.AddListener(() => { EventResult(); });

        if (_chosenEvent.IsRandom)
        {
            _option1Button.transform.SetSiblingIndex(Random.Range(0, 3));
            _option2Button.transform.SetSiblingIndex(Random.Range(0, 3));
            _option3Button.transform.SetSiblingIndex(Random.Range(0, 3));
        }

        _eventDialogue.text = _chosenEvent.ScenarioDialogue;
        _eventImageDisplay.sprite = _chosenEvent.ScenarioImage;

        GameObject firstButton = _option1Button.transform.parent.GetChild(0).gameObject;

        if (_eventSystem != null)
        {
            _eventSystem.SetSelectedGameObject(firstButton);
        }
    }

    public void EventResult()
    {

        _chosenEvent.EventResult();

        _option1Button.gameObject.SetActive(false);
        _option2Button.gameObject.SetActive(false);
        _option3Button.gameObject.SetActive(false);

        _eventDialogue.text = _chosenEvent.ResultDialogue;
        _eventImageDisplay.sprite = _chosenEvent.ResultImage;

        _continueButton.gameObject.SetActive(true);
        _continueButton.onClick.RemoveAllListeners();

        _continueButton.onClick.AddListener(() => _mapData.CurrentRow++);
        _continueButton.onClick.AddListener(() => SceneHandler.GoToMap());

        _eventSystem.SetSelectedGameObject(_continueButton.gameObject);
    }
}