using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : Singleton<PlayerSpawner>
{
    public GameObject playerPrefab;
    private GameObject player;
    [SerializeField] private GameObject deathEffect;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.I.GetSpawnPosition();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    public void PlayerDie()
    {
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(player);
        SpawnPlayer();
    }
}