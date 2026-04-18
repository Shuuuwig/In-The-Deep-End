using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] TeamData _teamData;
    [SerializeField] MapData _mapData;


    void Awake()
    {
        _teamData.ResetTeam();
        _mapData.ResetProgress();
    }
}
