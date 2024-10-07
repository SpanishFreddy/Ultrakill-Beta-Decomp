using UnityEngine;

public class SlowMo : MonoBehaviour
{
	private void Awake()
	{
		Time.timeScale = 0.1f;
		Time.fixedDeltaTime = 0.002f;
	}
}
