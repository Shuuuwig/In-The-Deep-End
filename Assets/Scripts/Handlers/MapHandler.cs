using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapHandler : MonoBehaviour
{
    [Header("Horizontal Rows")]
    [SerializeField] List<RectTransform> _rows = new List<RectTransform>();

    [Header("Room Prefabs & Data")]
    [SerializeField] GameObject _roomSelectionPrefab;
    [SerializeField] MapData _mapData;
    [SerializeField] RoomData _combatData;
    [SerializeField] RoomData _eventData;
    [SerializeField] RoomData _restData;

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
        if (_mapData.MapSeed != 0)
        {
            _masterSeed = _mapData.MapSeed;
        }
        else
        {
            _masterSeed = Random.Range(1, 1000000);
            _mapData.MapSeed = _masterSeed;
        }

        _mapData.NumberOfRows = _rows.Count;
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
        SnapToBottom();
    }

    public void ManageInitialSelections()
    {
        int savedRow = _mapData.CurrentRow;
        int savedRoomID = _mapData.CurrentRoom;

        Debug.Log($"Checking Progress: Row {savedRow}, ID {savedRoomID}");

        foreach (var rowRect in _rows)
        {
            foreach (var room in rowRect.GetComponentsInChildren<RoomInfo>())
                room.SetInteractable(false);
        }

        if (savedRow == -1)
        {
            foreach (var room in _rows[0].GetComponentsInChildren<RoomInfo>())
                room.SetInteractable(true);
            return;
        }

        RoomInfo[] roomsInSavedRow = _rows[savedRow].GetComponentsInChildren<RoomInfo>();
        bool foundSavedRoom = false;

        foreach (RoomInfo room in roomsInSavedRow)
        {
            if (room.transform.GetSiblingIndex() == savedRoomID)
            {
                foundSavedRoom = true;
                Debug.Log($"Found Saved Room: {room.name}. Child Count: {room.NextConnectedRooms.Count}");

                if (room.NextConnectedRooms.Count == 0)
                {
                    Debug.LogError("The saved room has NO connections! Pathfinding didn't link them.");
                }

                foreach (RoomInfo nextRoom in room.NextConnectedRooms)
                {
                    nextRoom.SetInteractable(true);
                }
                break;
            }
        }

        if (!foundSavedRoom)
        {
            Debug.LogError($"Could not find room with ID {savedRoomID} in Row {savedRow}!");
        }
    }

    public void EnterRoom(RoomInfo selectedRoom)
    {
        int roomRowIndex = _rows.IndexOf(selectedRoom.transform.parent.GetComponent<RectTransform>());

        if (roomRowIndex == _rows.Count - 1)
        {
            ResetMapProgress();
            return;
        }

        _mapData.CurrentRow = roomRowIndex;
        _mapData.CurrentRoom = selectedRoom.transform.GetSiblingIndex();

        foreach (RectTransform row in _rows)
        {
            RoomInfo[] rooms = row.GetComponentsInChildren<RoomInfo>();
            foreach (RoomInfo room in rooms)
            {
                room.SetInteractable(false);
            }
        }

        foreach (RoomInfo nextRoom in selectedRoom.NextConnectedRooms)
        {
            nextRoom.SetInteractable(true);
        }
    }

    public void ResetMapProgress()
    {
        _mapData.MapSeed = 0;
        _mapData.CurrentRow = -1;
        _mapData.CurrentRoom = -1;
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
                if (targets.Count == 0) continue;

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
        RoomInfo closest = null; RoomInfo second = null;
        float min = float.MaxValue; float secondMin = float.MaxValue;

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
        if (currentRow == 0)
            return Random.Range(2, 4);
        if (currentRow == _rows.Count - 1)
            return 1;
        float noise = Mathf.PerlinNoise(currentRow * _noiseScale + _xOffset, _yOffset);
        return noise > 0.5f ? Random.Range(2, 4) : Random.Range(1, 3);
    }

    RoomData RandomizeRoomData(int row)
    {
        if (row == 0 || row == _rows.Count - 1)
            return _combatData;

        float randomValue = Random.value;

        if (randomValue < _combatRoomSpawnChance)
            return _combatData;
        if (randomValue < (_combatRoomSpawnChance + _eventRoomSpawnChance))
            return _eventData;
        return _restData;
    }

    void SpawnRoomSelection(RoomData roomData, int row)
    {
        GameObject newRoom = Instantiate(_roomSelectionPrefab, _rows[row]);
        newRoom.GetComponent<RoomInfo>().RoomSetup(_mapData, roomData, this);
    }

    void SnapToBottom()
    {
        if (_scrollRect != null)
            _scrollRect.verticalNormalizedPosition = 0f;
    }
}