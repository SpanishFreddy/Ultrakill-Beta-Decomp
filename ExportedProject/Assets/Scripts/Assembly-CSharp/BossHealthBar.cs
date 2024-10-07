using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
	public GameObject bossBar;

	public Slider hpSlider;

	public Slider hpAfterImage;

	private EnemyIdentifier eid;

	private SpiderBody spb;

	public string bossName;

	private GameObject filler;

	private float shakeTime;

	private Vector3 originalPosition;

	private bool done;

	private void Awake()
	{
		eid = GetComponent<EnemyIdentifier>();
		hpSlider.gameObject.GetComponentInChildren<Text>().text = bossName;
		if (eid.type == EnemyType.Spider)
		{
			spb = GetComponent<SpiderBody>();
			hpSlider.maxValue = spb.health;
			hpSlider.value = spb.health;
		}
		hpAfterImage.maxValue = hpSlider.maxValue;
		hpAfterImage.value = hpSlider.value;
		filler = bossBar.transform.GetChild(0).gameObject;
		originalPosition = filler.transform.position;
		bossBar.SetActive(true);
	}

	private void Update()
	{
		if (hpSlider.value != spb.health)
		{
			shakeTime = 5f * (hpSlider.value - spb.health);
			hpSlider.value = spb.health;
		}
		if (hpAfterImage.value != hpSlider.value)
		{
			hpAfterImage.value -= Time.deltaTime * 6f;
			if (hpAfterImage.value < hpSlider.value)
			{
				hpAfterImage.value = hpSlider.value;
			}
		}
		if (shakeTime != 0f)
		{
			shakeTime -= Time.deltaTime * 10f;
			filler.transform.position = new Vector3(originalPosition.x + Random.Range(0f - shakeTime, shakeTime), originalPosition.y + Random.Range(0f - shakeTime, shakeTime), originalPosition.z);
			if (shakeTime < 0f)
			{
				shakeTime = 0f;
				filler.transform.position = originalPosition;
			}
		}
		if (!done && hpAfterImage.value <= 0f)
		{
			done = true;
			Invoke("Disappear", 3f);
		}
	}

	private void Disappear()
	{
		bossBar.SetActive(false);
	}
}
