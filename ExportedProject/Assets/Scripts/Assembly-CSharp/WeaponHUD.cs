using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
	private Revolver rev;

	public Sprite variation1;

	public Sprite variation2;

	public Sprite variation3;

	public Color color1;

	public Color color2;

	public Color color3;

	private Image img;

	private void Awake()
	{
		rev = GameObject.FindWithTag("MainCamera").GetComponentInChildren<Revolver>();
		img = GetComponent<Image>();
	}

	private void Update()
	{
		if (rev.gunVariation == 0 && img.sprite != variation1)
		{
			img.sprite = variation1;
			img.color = color1;
		}
		else if (rev.gunVariation == 1 && img.sprite != variation2)
		{
			img.sprite = variation2;
			img.color = color2;
		}
		else if (rev.gunVariation == 2 && img.sprite != variation3)
		{
			img.sprite = variation3;
			img.color = color3;
		}
	}
}
