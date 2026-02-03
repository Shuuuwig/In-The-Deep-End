using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.LightTransport;
using System.Linq;

public class MapHandler : MonoBehaviour
{
    [Header("Horizontal Rows")]
    [SerializeField] List<RectTransform> _rows = new List<RectTransform>();

    [Header("Room Select Prefab")]
    [SerializeField] GameObject _roomSelectionPrefab;

    [Header("Room Data")]
    [SerializeField] RoomData _combatData;
    [SerializeField] RoomData _eventData;
    [SerializeField] RoomData _restData;

    [Header("Map Configuration")]
    [SerializeField] int _masterSeed;
    [SerializeField] float _noiseScale = 0.2f;
    float _xOffset;
    float _yOffset;
    int? _currentRow;
    MapData _mapData;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
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
                RoomData selectedData = RandomizeRoomData(currentRow);
                SpawnRoomSelection(selectedData, currentRow);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rows[currentRow]);
            DetermineCurrentPosition();
        }
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
        else
            return Random.Range(1, 3);
    }

    RoomData RandomizeRoomData(int currentRow)
    {
        if (currentRow == 0 || currentRow == _rows.Count - 1)
            return _combatData;

        float randomValue = Random.value;

        if (randomValue < 0.75f)
            return _combatData;
        if (randomValue < 0.95f)
            return _eventData;
        else
            return _restData;
    }

    void SpawnRoomSelection(RoomData roomData, int row)
    {
        GameObject newRoom = Instantiate(_roomSelectionPrefab, _rows[row]);

        RoomInfo roomInfo = newRoom.GetComponent<RoomInfo>();
        roomInfo.RoomSetup(roomData);
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