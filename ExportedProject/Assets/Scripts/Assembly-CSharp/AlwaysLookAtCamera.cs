using UnityEngine;

public class AlwaysLookAtCamera : MonoBehaviour
{
	private GameObject camera;

	private void Start()
	{
		camera = GameObject.FindWithTag("MainCamera");
	}

	private void Update()
	{
		base.transform.LookAt(camera.transform);
	}
}
