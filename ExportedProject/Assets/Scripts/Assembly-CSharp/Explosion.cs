using UnityEngine;

public class Explosion : MonoBehaviour
{
	private bool harmless;

	private CameraController cc;

	private Light light;

	private MeshRenderer mr;

	private Color materialColor;

	private bool fading;

	private void Start()
	{
		cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		cc.CameraShake(1f);
		light = GetComponent<Light>();
		Invoke("Fade", 1f);
	}

	private void FixedUpdate()
	{
		base.transform.localScale += Vector3.one * 0.15f;
		light.range += 0.15f;
		if (fading)
		{
			materialColor.a -= 0.05f;
			light.intensity -= 0.5f;
			mr.material.SetColor("_TintColor", materialColor);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!harmless && other.gameObject.tag == "Player")
		{
			other.gameObject.GetComponent<NewMovement>().GetHurt(50);
			harmless = true;
		}
	}

	private void Fade()
	{
		harmless = true;
		mr = GetComponent<MeshRenderer>();
		materialColor = mr.material.GetColor("_TintColor");
		fading = true;
	}
}
