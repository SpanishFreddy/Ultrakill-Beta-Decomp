using UnityEngine;

public class ActivateNextWave : MonoBehaviour
{
	public bool lastWave;

	private bool activated;

	public int deadEnemies;

	public int enemyCount;

	public float delay;

	public GameObject[] enemies;

	private int currentEnemy;

	public Door[] doors;

	private int currentDoor;

	public GameObject[] toActivate;

	private bool objectsActivated;

	public bool needNextRoomInfo;

	private void FixedUpdate()
	{
		if (!activated && deadEnemies == enemyCount)
		{
			activated = true;
			if (!lastWave)
			{
				Invoke("SpawnEnemy", delay);
			}
			else
			{
				Invoke("EndWaves", delay);
			}
		}
	}

	private void SpawnEnemy()
	{
		enemies[currentEnemy].SetActive(true);
		currentEnemy++;
		if (currentEnemy < enemies.Length)
		{
			Invoke("SpawnEnemy", 0.1f);
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void EndWaves()
	{
		if (toActivate.Length > 0 && !objectsActivated)
		{
			GameObject[] array = toActivate;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
			}
			objectsActivated = true;
			EndWaves();
		}
		else if (currentDoor < doors.Length)
		{
			doors[currentDoor].Unlock();
			doors[currentDoor].Open();
			currentDoor++;
			Invoke("EndWaves", 0.1f);
		}
		else
		{
			if (needNextRoomInfo)
			{
				GetComponentInParent<NextRoomInfo>().nextRoom.SetActive(true);
			}
			Object.Destroy(this);
		}
	}
}
