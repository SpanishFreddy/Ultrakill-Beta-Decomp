using UnityEngine;
using UnityEngine.UI;

public class StyleHUD : MonoBehaviour
{
	private GameObject styleHud;

	public Image styleRank;

	public Sprite[] ranks;

	private int currentRank;

	private float rankShaking;

	private Vector3 defaultPos;

	private float rankScale;

	private Slider styleMeter;

	public float currentMeter;

	public int maxMeter;

	public float drainSpeed = 1f;

	private Text styleInfo;

	private bool ascending;

	private int testNumber;

	private void Start()
	{
		styleHud = base.transform.GetChild(0).gameObject;
		styleMeter = GetComponentInChildren<Slider>();
		styleInfo = GetComponentInChildren<Text>();
		ComboOver();
		defaultPos = styleRank.transform.localPosition;
	}

	private void Update()
	{
		if (currentMeter > 0f && !styleHud.activeSelf)
		{
			ComboStart();
		}
		if (currentMeter != 0f)
		{
			if (currentMeter < 0f)
			{
				if (currentRank == 0)
				{
					ComboOver();
				}
				else if (!ascending)
				{
					ascending = true;
					DescendRank();
				}
			}
			else
			{
				currentMeter -= Time.deltaTime * (drainSpeed * 16f);
			}
		}
		styleMeter.value = currentMeter;
		if (currentMeter > (float)maxMeter && !ascending)
		{
			if (currentRank < 7)
			{
				ascending = true;
				AscendRank();
			}
			else
			{
				currentMeter = maxMeter;
			}
		}
		if (rankShaking > 0f)
		{
			styleRank.transform.localPosition = new Vector3(defaultPos.x + rankShaking * (float)Random.Range(-3, 3), defaultPos.y + rankShaking * (float)Random.Range(-3, 3), defaultPos.z);
			rankShaking -= Time.deltaTime * 5f;
		}
		if (rankScale > 0f)
		{
			styleRank.transform.localScale = new Vector3(2f + rankScale, 1f + rankScale, 1f + rankScale);
			rankScale -= Time.deltaTime;
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			AddPoints(50, "BANANA" + testNumber);
			testNumber++;
		}
	}

	public void ComboStart()
	{
		maxMeter = 100;
		styleHud.SetActive(true);
	}

	public void ComboOver()
	{
		currentMeter = 0f;
		styleHud.SetActive(false);
		maxMeter = 9999;
	}

	private void AscendRank()
	{
		float num = currentMeter - (float)maxMeter;
		currentRank++;
		float num2 = currentRank;
		maxMeter = 100 + currentRank * 20;
		styleMeter.maxValue = maxMeter;
		drainSpeed = 1f + num2 / 5f;
		styleRank.sprite = ranks[currentRank];
		rankScale = 1f;
		if (num > (float)(maxMeter / 4))
		{
			currentMeter = num;
		}
		else
		{
			currentMeter = maxMeter / 4;
		}
		ascending = false;
	}

	private void DescendRank()
	{
		currentRank--;
		float num = currentRank;
		maxMeter = 100 + currentRank * 20;
		styleMeter.maxValue = maxMeter;
		drainSpeed = 1f + num / 5f;
		styleRank.sprite = ranks[currentRank];
		currentMeter = maxMeter - maxMeter / 4;
		ascending = false;
	}

	public void AddPoints(int points, string pointName)
	{
		currentMeter += points;
		rankScale = 0.2f;
		if (pointName != string.Empty)
		{
			styleInfo.text = "+ " + pointName + "\n" + styleInfo.text;
			Invoke("RemoveText", 3f);
		}
	}

	public void RemovePoints(int points)
	{
		currentMeter -= points;
		rankShaking = 5f;
	}

	private void RemoveText()
	{
		styleInfo.text = styleInfo.text.Substring(0, styleInfo.text.LastIndexOf("+"));
	}
}
