using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
<<<<<<< Updated upstream
using UnityEditor.SceneManagement;
using UnityEngine.LightTransport;
using System.Linq;
=======
using System.Collections;
>>>>>>> Stashed changes

public class MapHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] List<RectTransform> _rows = new List<RectTransform>();
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Room Select Prefab")]
    [SerializeField] GameObject _roomSelectionPrefab;

    [Header("Room Data")]
    [SerializeField] RoomData _combatData;
    [SerializeField] RoomData _eventData;
    [SerializeField] RoomData _restData;

    [Header("Map Configuration")]
    [SerializeField] int _masterSeed;
    [SerializeField] float _noiseScale = 0.2f;
    [SerializeField][Range(0, 1)] float _combatRoomSpawnChance = 0.7f;
    [SerializeField][Range(0, 1)] float _eventRoomSpawnChance = 0.25f;
    [SerializeField][Range(0, 1)] float _extraPathChance = 0.3f;

    float _xOffset;
    float _yOffset;
    int? _currentRow;
    MapData _mapData;

    [Header("Path Settings")]
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private RectTransform _lineParent;

    void Start()
    {
        StartCoroutine(GenerateMapRoutine());
    }

    IEnumerator GenerateMapRoutine()
    {
        if (_mapData != null)
        {           
            return;
        }

        if (_masterSeed == 0)
            _masterSeed = Random.Range(1, 1000000);

        Random.InitState(_masterSeed);

        _xOffset = Random.Range(0f, 10000f);
        _yOffset = Random.Range(0f, 10000f);

        for (int currentRow = 0; currentRow < _rows.Count; currentRow++)
        {
            int numOfRooms = GenerateNumberOfRooms(currentRow);

            for (int room = 0; room < numOfRooms; room++)
            {
                SpawnRoomSelection(RandomizeRoomData(currentRow), currentRow);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rows[currentRow]);
            DetermineCurrentPosition();
        }

        if (_scrollRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);

        yield return new WaitForEndOfFrame();

        GeneratePaths();
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
        RoomInfo closestRoom = null;
        RoomInfo secondClosestRoom = null;
        float minDistance = float.MaxValue;
        float secondMinDistance = float.MaxValue;

        foreach (RoomInfo nextRoom in connectableRooms)
        {
            float distance = (currentRoom.transform.position - nextRoom.transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                secondMinDistance = minDistance;
                secondClosestRoom = closestRoom;

                minDistance = distance;
                closestRoom = nextRoom;
            }
            else if (distance < secondMinDistance)
            {
                secondMinDistance = distance;
                secondClosestRoom = nextRoom;
            }
        }

        List<RoomInfo> results = new List<RoomInfo>();
        if (closestRoom != null)
            results.Add(closestRoom);
        if (secondClosestRoom != null)
            results.Add(secondClosestRoom);

        return results;
    }

    int GenerateNumberOfRooms(int currentRow)
    {
        if (currentRow == 0)
            return Random.Range(2, 4);
        if (currentRow == _rows.Count - 1)
            return 1;

        float noiseValue = Mathf.PerlinNoise(currentRow * _noiseScale + _xOffset, _yOffset);
        if (noiseValue > 0.5f)
            return Random.Range(2, 4);

        return Random.Range(1, 3);
    }

    RoomData RandomizeRoomData(int currentRow)
    {
        if (currentRow == 0 || currentRow == _rows.Count - 1)
            return _combatData;

        float randomValue = Random.value;
        if (randomValue < _combatRoomSpawnChance)
            return _combatData;
        if (randomValue < _combatRoomSpawnChance + _eventRoomSpawnChance)
            return _eventData;

        return _restData;
    }

    void SpawnRoomSelection(RoomData roomData, int row)
    {
        GameObject newRoom = Instantiate(_roomSelectionPrefab, _rows[row]);
        newRoom.GetComponent<RoomInfo>().RoomSetup(roomData);
    }

    private void DrawLine(RectTransform start, RectTransform end)
    {
        GameObject lineObj = Instantiate(_linePrefab, _lineParent);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        Vector2 direction = end.position - start.position;
        lineRect.position = start.position + (Vector3)(direction / 2);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
        lineRect.sizeDelta = new Vector2(direction.magnitude, 15f);
    }

    void SnapToBottom()
    {
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    void OnValidate()
    {
        if (_combatRoomSpawnChance + _eventRoomSpawnChance > 1f)
        {
            _eventRoomSpawnChance = 1f - _combatRoomSpawnChance;
        }
    }

    public void DetermineCurrentPosition()
    {
        if (_currentRow == null)
            _currentRow = 0;
        else
            _currentRow++;

        Button[] selectableRooms = _rows[(int)_currentRow].GetComponentsInChildren<Button>();

        for (int room = 0; room < selectableRooms.Length; room++)
        {
            selectableRooms[room].enabled = true;


        }
    }
}