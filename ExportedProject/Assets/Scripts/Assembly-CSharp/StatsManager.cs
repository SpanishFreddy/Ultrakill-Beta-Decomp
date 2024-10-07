using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{
	public GameObject[] checkPoints;

	private GameObject player;

	private Vector3 spawnPos;

	public CheckPoint currentCheckPoint;

	public GameObject debugCheckPoint;

	private void Start()
	{
		player = GameObject.FindWithTag("Player");
		spawnPos = player.transform.position;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			Restart();
		}
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			player.transform.position = debugCheckPoint.transform.position;
		}
	}

	public void GetCheckPoint(Vector3 position)
	{
		spawnPos = position;
	}

	public void Restart()
	{
		if (currentCheckPoint == null)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}
		else
		{
			currentCheckPoint.OnRespawn();
		}
	}
}
