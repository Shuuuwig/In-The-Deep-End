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
        // Use MapPrefs.Seed instead of local string
        if (PlayerPrefs.HasKey(MapPrefs.Seed))
        {
            _masterSeed = PlayerPrefs.GetInt(MapPrefs.Seed);
        }
        else
        {
            _masterSeed = Random.Range(1, 1000000);
            PlayerPrefs.SetInt(MapPrefs.Seed, _masterSeed);
        }

        // Use MapPrefs.LastRow
        PlayerPrefs.SetInt(MapPrefs.LastRow, _rows.Count - 1);
        StartCoroutine(GenerateMapRoutine());
    }

    void OnValidate()
    {
        if (_combatRoomSpawnChance + _eventRoomSpawnChance > 1f)
        {
            _eventRoomSpawnChance = 1f - _combatRoomSpawnChance;
        }
    }

    IEnumerator GenerateMapRoutine()
    {
        if (_masterSeed == 0)
            _masterSeed = Random.Range(1, 1000000);

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
                if (nextRowReached.Contains(nextRoom))
                    continue;

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
                secondMin = min;
                second = closest;
                min = dist;
                closest = next;
            }
            else if (dist < secondMin)
            {
                secondMin = dist;
                second = next;
            }
        }

        List<RoomInfo> results = new List<RoomInfo>();
        if (closest != null)
            results.Add(closest);
        if (second != null)
            results.Add(second);

        return results;
    }

    public void ManageInitialSelections()
    {
        int savedRow = PlayerPrefs.GetInt(MapPrefs.Row, -1);
        int savedRoomID = PlayerPrefs.GetInt(MapPrefs.RoomID, -1);

        Debug.Log($"THIS IS THE SAVED ROW!!!!{savedRow}");
        Debug.Log($"THIS IS THE SAVED ROOMID!!!!{savedRoomID}");


        for (int row = 0; row < _rows.Count; row++)
        {
            RoomInfo[] roomsInRow = _rows[row].GetComponentsInChildren<RoomInfo>();

            foreach (RoomInfo room in roomsInRow)
            {
                if (savedRow == -1)
                {
                    room.SetInteractable(row == 0);
                }
                else
                {
                    if (row == savedRow && room.transform.GetSiblingIndex() == savedRoomID)
                    {
                        Debug.Log($"Found Saved Room: {room.name}. Connections count: {room.NextConnectedRooms.Count}");

                        foreach (RoomInfo nextRoom in room.NextConnectedRooms)
                        {
                            Debug.Log($"next room is {nextRoom}");
                            nextRoom.SetInteractable(true);
                        }
                    }
                    else
                    {
                        room.SetInteractable(false);
                    }
                }
            }
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

        PlayerPrefs.SetInt(MapPrefs.Row, roomRowIndex);
        PlayerPrefs.SetInt(MapPrefs.RoomID, selectedRoom.transform.GetSiblingIndex());
        PlayerPrefs.Save();

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
            Debug.Log("New rooms selection");
            nextRoom.SetInteractable(true);
        }
    }

    public void ResetMapProgress()
    {
        _masterSeed = 0;
        if (_mapData != null)
            _mapData.CurrentRow = 0;

        PlayerPrefs.DeleteKey(MapPrefs.Seed);
        PlayerPrefs.DeleteKey(MapPrefs.Row);
        PlayerPrefs.DeleteKey(MapPrefs.RoomID);
        PlayerPrefs.DeleteKey(MapPrefs.LastRow);
        PlayerPrefs.Save();
    }

    void StoreRoomPaths(RectTransform start, RectTransform end)
    {
        RoomInfo startRoom = start.GetComponent<RoomInfo>();
        RoomInfo endRoom = end.GetComponent<RoomInfo>();

        if (startRoom && endRoom != null)
        {
            if (!startRoom.NextConnectedRooms.Contains(endRoom))
            {
                startRoom.NextConnectedRooms.Add(endRoom);
            }
        }
    }

    void DrawLine(RectTransform start, RectTransform end)
    {
        GameObject lineObj = Instantiate(_linePrefab, _lineParent);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        Vector2 direction = end.position - start.position;
        lineRect.position = start.position + (Vector3)(direction / 2f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
        lineRect.sizeDelta = new Vector2(direction.magnitude, 15f);

        lineObj.transform.SetAsFirstSibling();
        StoreRoomPaths(start, end);
    }

    int NumberOfRoomsInRow(int currentRow)
    {
        if (currentRow == 0)
        {
            return Random.Range(2, 4);
        }

        if (currentRow == _rows.Count - 1)
        {
            return 1;
        }

        float noise = Mathf.PerlinNoise(currentRow * _noiseScale + _xOffset, _yOffset);

        if (noise > 0.5f)
        {
            return Random.Range(2, 4);
        }
        else
        {
            return Random.Range(1, 3);
        }
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

    void SnapToBottom()
    {
        if (_scrollRect != null)
            _scrollRect.verticalNormalizedPosition = 0f;
    }
}