using UnityEngine;

public class PlayerDataBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerDataSo playerOneData;
    [SerializeField] private PlayerDataSo playerTwoData;

    private void Awake()
    {
        PlayerPersistentData.InitializeFromData(playerOneData, playerTwoData);
    }
}