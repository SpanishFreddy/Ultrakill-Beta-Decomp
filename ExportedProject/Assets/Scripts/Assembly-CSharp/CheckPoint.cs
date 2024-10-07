using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	private StatsManager sm;

	private bool activated;

	public GameObject toActivate;

	public GameObject[] rooms;

	private GameObject[] roomsPriv;

	public GameObject[] roomPrefabs;

	public GameObject[] defaultRooms;

	private List<GameObject> newRooms = new List<GameObject>();

	private int i;

	private GameObject tempRoom;

	private GameObject player;

	private NewMovement nm;

	private float tempRot;

	public GameObject graphic;

	private void Start()
	{
		defaultRooms = rooms;
		for (int i = 0; i < defaultRooms.Length; i++)
		{
			Debug.Log("current " + i);
			newRooms.Add(tempRoom);
			newRooms[i] = Object.Instantiate(defaultRooms[i], defaultRooms[i].transform.position, defaultRooms[i].transform.rotation, defaultRooms[i].transform.parent);
			defaultRooms[i].gameObject.SetActive(false);
			newRooms[i].gameObject.SetActive(true);
			defaultRooms[i].transform.position = new Vector3(defaultRooms[i].transform.position.x + 10000f, defaultRooms[i].transform.position.y, defaultRooms[i].transform.position.z);
		}
		player = GameObject.FindWithTag("Player");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!activated && other.gameObject.tag == "Player")
		{
			sm = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
			sm.currentCheckPoint = this;
			activated = true;
			GetComponent<AudioSource>().Play();
			Object.Destroy(graphic);
		}
	}

	public void OnRespawn()
	{
		player.transform.position = Vector3.one * -1000f;
		i = 0;
		ResetRoom();
	}

	private void ResetRoom()
	{
		Vector3 position = newRooms[i].transform.position;
		Object.Destroy(newRooms[i]);
		newRooms[i] = Object.Instantiate(defaultRooms[i], position, defaultRooms[i].transform.rotation, defaultRooms[i].transform.parent);
		newRooms[i].SetActive(true);
		if (i + 1 < defaultRooms.Length)
		{
			i++;
			ResetRoom();
			return;
		}
		player.transform.position = base.transform.position;
		if (nm == null)
		{
			nm = player.GetComponent<NewMovement>();
		}
		nm.Respawn();
		nm.GetHealth(0, true);
		nm.cc.StopShake();
		tempRot = nm.transform.rotation.eulerAngles.y - base.transform.rotation.eulerAngles.y;
		nm.cc.additionalRotationY += tempRot * -1f;
		for (tempRot = (nm.cc.transform.rotation.eulerAngles.x - base.transform.rotation.eulerAngles.x) * -1f; tempRot < nm.cc.minimumX + nm.cc.additionalRotationX; tempRot += 360f)
		{
		}
		while (tempRot > nm.cc.maximumX + nm.cc.additionalRotationX)
		{
			tempRot -= 360f;
		}
		nm.cc.additionalRotationX += tempRot;
		toActivate.SetActive(true);
	}
}
