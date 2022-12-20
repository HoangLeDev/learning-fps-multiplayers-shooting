using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : Singleton<PlayerSpawner>
{
    public GameObject playerPrefab;
    private GameObject player;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float respawnTime = 3f;

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

    public void PlayerDie(string attacker)
    {
        UIController.I.deathTMP.text = ConstantHolder.MESSAGE_WHEN_DEATH + attacker;

        if (player != null)
        {
            StartCoroutine(CoDieCooldown());
        }
    }

    public IEnumerator CoDieCooldown()
    {
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(player);
        UIController.I.deathScreen.SetActive(true);
        yield return new WaitForSeconds(respawnTime);
        UIController.I.deathScreen.SetActive(false);
        SpawnPlayer();
    }
}