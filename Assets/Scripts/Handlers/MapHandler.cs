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

    private float _xOffset;
    private float _yOffset;

    void Start()
    {
        StartCoroutine(GenerateMapRoutine());
    }

    private void OnValidate()
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
            for (int r = 0; r < numOfRooms; r++)
            {
                SpawnRoomSelection(RandomizeRoomData(currentRow), currentRow);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rows[currentRow]);
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
    }

    int NumberOfRoomsInRow(int currentRow)
    {
        if (currentRow == 0)
            return Random.Range(2, 4);
        if (currentRow == _rows.Count - 1)
            return 1;

        float noise = Mathf.PerlinNoise(currentRow * _noiseScale + _xOffset, _yOffset);

        if (noise > 0.5)
            return Random.Range(2, 4);

        return Random.Range(1, 3);
    }

    RoomData RandomizeRoomData(int i)
    {
        if (i == 0 || i == _rows.Count - 1)
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
        newRoom.GetComponent<RoomInfo>().RoomSetup(roomData);
    }

    void SnapToBottom()
    {
        if (_scrollRect != null)
            _scrollRect.verticalNormalizedPosition = 0f;
    }
}