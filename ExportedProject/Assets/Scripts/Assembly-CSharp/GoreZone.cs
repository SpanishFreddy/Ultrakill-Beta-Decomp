using UnityEngine;

public class GoreZone : MonoBehaviour
{
	public Transform goreZone;

	private MeshFilter tempFilter;

	public void Combine()
	{
		StaticBatchingUtility.Combine(goreZone.gameObject);
	}
}
