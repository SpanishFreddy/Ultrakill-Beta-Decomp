using UnityEngine;

public class WallCheck : MonoBehaviour
{
	public bool onWall;

	public Vector3 poc;

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			onWall = true;
			poc = other.ClosestPoint(base.transform.position);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			onWall = false;
		}
	}
}
