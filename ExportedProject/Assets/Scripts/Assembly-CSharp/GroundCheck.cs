using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	public bool onGround;

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			onGround = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			onGround = false;
		}
	}
}
