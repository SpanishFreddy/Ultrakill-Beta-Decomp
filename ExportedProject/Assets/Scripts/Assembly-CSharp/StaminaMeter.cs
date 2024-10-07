using UnityEngine;
using UnityEngine.UI;

public class StaminaMeter : MonoBehaviour
{
	private Slider stm;

	private Image staminaFlash;

	private Color flashColor;

	private bool full = true;

	private AudioSource aud;

	private void Start()
	{
		stm = GetComponent<Slider>();
		staminaFlash = base.transform.GetChild(1).GetChild(0).GetChild(0)
			.GetComponent<Image>();
		flashColor = staminaFlash.color;
	}

	private void Update()
	{
		if (stm.value >= stm.maxValue && !full)
		{
			full = true;
			Flash();
		}
		if (flashColor.a > 0f)
		{
			if (flashColor.a - Time.deltaTime > 0f)
			{
				flashColor.a -= Time.deltaTime;
			}
			else
			{
				flashColor.a = 0f;
			}
			staminaFlash.color = flashColor;
		}
		if (stm.value < stm.maxValue)
		{
			full = false;
		}
	}

	private void Flash()
	{
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		aud.Play();
		flashColor.a = 1f;
		staminaFlash.color = flashColor;
	}
}
