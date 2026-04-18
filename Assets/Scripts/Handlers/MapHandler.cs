using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapHandler : MonoBehaviour
{
    [Header("Input & Navigation")]
    [SerializeField] EventSystem _eventSystem;

    [Header("Horizontal Rows")]
    [SerializeField] List<RectTransform> _rows = new List<RectTransform>();

    [Header("Room Prefabs & Data")]
    [SerializeField] GameObject _roomSelectionPrefab;
    [SerializeField] MapData _mapData;
    [SerializeField] TeamData _teamData;
    [SerializeField] RoomData _combatData;
    [SerializeField] RoomData _eventData;
    [SerializeField] RoomData _restData;
    [SerializeField] AudioClip _mapMusic;

    [Header("Map Generation Settings")]
    [SerializeField] int _masterSeed;
    [SerializeField] float _noiseScale = 0.2f;
    [SerializeField][Range(0, 1)] float _combatRoomSpawnChance = 0.7f;
    [SerializeField][Range(0, 1)] float _eventRoomSpawnChance = 0.25f;
    [Range(0f, 1f)][SerializeField] float _extraPathChance = 0.3f;

    [Header("Path Rendering")]
    [SerializeField] GameObject _linePrefab;
    [SerializeField] RectTransform _lineParent;
    [SerializeField] ScrollRect _scrollRect;

    float _xOffset;
    float _yOffset;

    void Start()
    {
        if (_eventSystem == null)
            _eventSystem = EventSystem.current;

        // Sync local master seed with the ScriptableObject
        if (_mapData.MapSeed != 0)
        {
            _masterSeed = _mapData.MapSeed;
        }
        else
        {
            _masterSeed = Random.Range(1, 1000000);
            _mapData.MapSeed = _masterSeed;
            _mapData.SaveProgress();
        }

        _mapData.NumberOfRows = _rows.Count;
        if (_mapMusic != null)
        {
            AudioHandler.Instance.PlayMusic(_mapMusic, true);
        }

        StartCoroutine(GenerateMapRoutine());
    }

    IEnumerator GenerateMapRoutine()
    {
        Random.InitState(_masterSeed);

        _xOffset = Random.Range(0f, 10000f);
        _yOffset = Random.Range(0f, 10000f);

        for (int currentRow = 0; currentRow < _rows.Count; currentRow++)
        {
            int numOfRooms = NumberOfRoomsInRow(currentRow);
            for (int room = 0; room < numOfRooms; room++)
            {
                SpawnRoomSelection(RandomizeRoomData(currentRow), currentRow);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rows[currentRow]);
        }

        if (_scrollRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);

        yield return new WaitForEndOfFrame();

        GeneratePaths();
        ManageInitialSelections();

        float target = 1f - (float)_mapData.CurrentRow / (_rows.Count - 1);
        StartCoroutine(SmoothFocusRow(target, 0.5f));
    }

    public void ManageInitialSelections()
    {
        int savedRow = _mapData.CurrentRow;
        int savedRoomID = _mapData.CurrentRoom;
        GameObject objectToSelect = null;

        foreach (var rowRect in _rows)
        {
            foreach (var room in rowRect.GetComponentsInChildren<RoomInfo>())
                room.SetInteractable(false);
        }

        if (savedRow == -1)
        {
            RoomInfo[] firstRowRooms = _rows[0].GetComponentsInChildren<RoomInfo>();
            foreach (var room in firstRowRooms)
                room.SetInteractable(true);

            if (firstRowRooms.Length > 0)
                objectToSelect = firstRowRooms[0].gameObject;
        }
        else
        {
            RoomInfo[] roomsInSavedRow = _rows[savedRow].GetComponentsInChildren<RoomInfo>();

            foreach (RoomInfo room in roomsInSavedRow)
            {
                if (room.transform.GetSiblingIndex() == savedRoomID)
                {
                    foreach (RoomInfo nextRoom in room.NextConnectedRooms)
                    {
                        nextRoom.SetInteractable(true);
                        if (objectToSelect == null)
                            objectToSelect = nextRoom.gameObject;
                    }
                    break;
                }
            }
        }

        if (_eventSystem != null && objectToSelect != null)
        {
            _eventSystem.SetSelectedGameObject(objectToSelect);
        }
    }

    public void EnterRoom(RoomInfo selectedRoom)
    {
        _mapData.CurrentRoom = selectedRoom.transform.GetSiblingIndex();
        // No need to increment Row here if your Battle/Event system handles it after victory,
        // but if you want to look at the NEXT row immediately:
        int targetRow = Mathf.Min(_mapData.CurrentRow + 1, _rows.Count - 1);

        _mapData.SaveProgress();

        // 1. Calculate target based on where the player is GOING
        float target = 1f - (float)targetRow / (_rows.Count - 1);

        // 2. Start the animation (and remove the instant FocusOnCurrentRow call)
        StartCoroutine(SmoothFocusRow(target, 0.4f));

        foreach (RectTransform row in _rows)
        {
            RoomInfo[] rooms = row.GetComponentsInChildren<RoomInfo>();
            foreach (RoomInfo room in rooms)
            {
                room.SetInteractable(false);
            }
        }

        Debug.Log($"Entered Room {_mapData.CurrentRoom} on Row {_mapData.CurrentRow}.");
    }

    public void ResetMapProgress()
    {
        _mapData.ResetProgress();
    }

    void GeneratePaths()
    {
        foreach (Transform child in _lineParent)
            Destroy(child.gameObject);

        for (int currentRow = 0; currentRow < _rows.Count - 1; currentRow++)
        {
            RoomInfo[] currentRowRooms = _rows[currentRow].GetComponentsInChildren<RoomInfo>();
            RoomInfo[] nextRowRooms = _rows[currentRow + 1].GetComponentsInChildren<RoomInfo>();
            HashSet<RoomInfo> nextRowReached = new HashSet<RoomInfo>();

            foreach (RoomInfo currentRoom in currentRowRooms)
            {
                List<RoomInfo> targets = GetNearestRooms(currentRoom, nextRowRooms);
                if (targets.Count == 0)
                    continue;

                DrawLine(currentRoom.GetComponent<RectTransform>(), targets[0].GetComponent<RectTransform>());
                nextRowReached.Add(targets[0]);

                if (targets.Count > 1 && Random.value < _extraPathChance)
                {
                    DrawLine(currentRoom.GetComponent<RectTransform>(), targets[1].GetComponent<RectTransform>());
                    nextRowReached.Add(targets[1]);
                }
            }

            foreach (RoomInfo nextRoom in nextRowRooms)
            {
                if (nextRowReached.Contains(nextRoom)) continue;
                List<RoomInfo> sources = GetNearestRooms(nextRoom, currentRowRooms);
                if (sources.Count > 0)
                {
                    DrawLine(sources[0].GetComponent<RectTransform>(), nextRoom.GetComponent<RectTransform>());
                }
            }
        }
    }

    List<RoomInfo> GetNearestRooms(RoomInfo currentRoom, RoomInfo[] connectableRooms)
    {
        RoomInfo closest = null;
        RoomInfo second = null;
        float min = float.MaxValue;
        float secondMin = float.MaxValue;

        foreach (RoomInfo next in connectableRooms)
        {
            float dist = (currentRoom.transform.position - next.transform.position).sqrMagnitude;
            if (dist < min)
            {
                secondMin = min; second = closest; min = dist; closest = next;
            }
            else if (dist < secondMin)
            {
                secondMin = dist; second = next;
            }
        }

        List<RoomInfo> results = new List<RoomInfo>();
        if (closest != null)
            results.Add(closest);
        if (second != null)
            results.Add(second);
        return results;
    }

    void StoreRoomPaths(RectTransform start, RectTransform end)
    {
        RoomInfo startRoom = start.GetComponent<RoomInfo>();
        RoomInfo endRoom = end.GetComponent<RoomInfo>();
        if (startRoom && endRoom != null && !startRoom.NextConnectedRooms.Contains(endRoom))
        {
            startRoom.NextConnectedRooms.Add(endRoom);
        }
    }

    void DrawLine(RectTransform start, RectTransform end)
    {
        GameObject lineObj = Instantiate(_linePrefab, _lineParent);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        Vector3 startPos = _lineParent.InverseTransformPoint(start.position);
        Vector3 endPos = _lineParent.InverseTransformPoint(end.position);

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;

        lineRect.localPosition = startPos + (Vector3)(direction / 2f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);
        lineRect.sizeDelta = new Vector2(distance, 15f);

        lineObj.transform.SetAsFirstSibling();
        StoreRoomPaths(start, end);
    }

    int NumberOfRoomsInRow(int currentRow)
    {
        if (currentRow == 0) return Random.Range(2, 4);
        if (currentRow == _rows.Count - 1) return 1;

        float noise = Mathf.PerlinNoise(currentRow * _noiseScale + _xOffset, _yOffset);
        return noise > 0.5f ? Random.Range(2, 4) : Random.Range(1, 3);
    }

    RoomData RandomizeRoomData(int row)
    {
        if (row == 0 || row == _rows.Count - 1) return _combatData;

        float randomValue = Random.value;
        if (randomValue < _combatRoomSpawnChance) return _combatData;
        if (randomValue < (_combatRoomSpawnChance + _eventRoomSpawnChance)) return _eventData;
        return _restData;
    }

    void SpawnRoomSelection(RoomData roomData, int row)
    {
        GameObject newRoom = Instantiate(_roomSelectionPrefab, _rows[row]);
        newRoom.GetComponent<RoomInfo>().RoomSetup(_mapData, roomData, this);
    }

    IEnumerator SmoothFocusRow(float targetPos, float duration)
    {
        float elapsed = 0;
        float startPos = _scrollRect.verticalNormalizedPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }
        _scrollRect.verticalNormalizedPosition = targetPos;
    }

    public void FocusOnCurrentRow()
    {
        if (_scrollRect == null || _rows.Count <= 1) return;

        // We use the saved CurrentRow index from your MapData
        int currentRow = _mapData.CurrentRow;

        // If we haven't started yet (-1), focus on the bottom
        if (currentRow <= 0)
        {
            _scrollRect.verticalNormalizedPosition = 0f;
            return;
        }

        // Calculate the normalized position (0 to 1)
        // We divide the current row index by the total available gaps between rows
        float targetNormalizedPos = (float)currentRow / (_rows.Count - 1);

        // If your map scrolls from bottom to top, 0 is the start. 
        // If it's inverted, you might need (1 - targetNormalizedPos)
        _scrollRect.verticalNormalizedPosition = targetNormalizedPos;
    }
}