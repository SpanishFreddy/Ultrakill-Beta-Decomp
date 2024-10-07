using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
	public Vector3 relativePosition;

	private NewMovement nm;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			other.transform.position = other.transform.position - relativePosition;
		}
	}
}
