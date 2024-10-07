using UnityEngine;

public class DoorController : MonoBehaviour
{
	public int type;

	private Door dc;

	private void Start()
	{
		dc = base.transform.parent.GetComponentInChildren<Door>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && !dc.locked)
		{
			if (type == 0)
			{
				dc.Open();
			}
			else if (type == 1)
			{
				dc.Open();
				Object.Destroy(this);
			}
			else if (type == 2)
			{
				dc.Close();
				Object.Destroy(this);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" && !dc.locked && type == 0)
		{
			dc.Close();
		}
	}
}
