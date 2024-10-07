using UnityEngine;
using UnityEngine.UI;

public class StyleCalculator : MonoBehaviour
{
	private StyleHUD shud;

	private GameObject player;

	private NewMovement nmov;

	public Text airTimeText;

	public float airTime = 1f;

	private Vector3 airTimePos;

	private void Start()
	{
		shud = GetComponent<StyleHUD>();
		player = GameObject.FindWithTag("Player");
		nmov = player.GetComponent<NewMovement>();
		airTimePos = airTimeText.transform.localPosition;
	}

	private void Update()
	{
		if (!nmov.gc.onGround)
		{
			if (airTime + Time.deltaTime < 3f)
			{
				airTime += Time.deltaTime * 2f;
			}
			else
			{
				airTime = 3f;
			}
			if (!airTimeText.gameObject.activeSelf)
			{
				airTimeText.gameObject.SetActive(true);
			}
			if (airTime >= 2f && airTime < 3f)
			{
				airTimeText.text = "<color=orange><size=60>x" + airTime.ToString("F2") + "</size></color>";
				airTimeText.transform.localPosition = new Vector3(airTimePos.x + (float)Random.Range(-3, 3), airTimePos.y + (float)Random.Range(-3, 3), airTimePos.z);
			}
			else if (airTime == 3f)
			{
				airTimeText.text = "<color=red><size=72>x" + airTime.ToString("F2") + "</size></color>";
				airTimeText.transform.localPosition = new Vector3(airTimePos.x + (float)Random.Range(-6, 6), airTimePos.y + (float)Random.Range(-6, 6), airTimePos.z);
			}
			else
			{
				airTimeText.text = "x" + airTime.ToString("F2");
				airTimeText.transform.localPosition = airTimePos;
			}
		}
		else
		{
			airTime = 1f;
			airTimeText.transform.localPosition = airTimePos;
			if (airTimeText.gameObject.activeSelf)
			{
				airTimeText.gameObject.SetActive(false);
			}
		}
	}

	public void HitCalculator(string hitter, string enemyType, string hitLimb, bool dead, GameObject sender)
	{
		switch (hitter)
		{
		case "punch":
			if (dead)
			{
				if (hitLimb == "head" || hitLimb == "limb")
				{
					AddPoints(60, "CRITICAL PUNCH");
				}
				else if (enemyType == "spider")
				{
					AddPoints(150, "<color=orange>BIG FISTKILL</color>");
				}
				else
				{
					AddPoints(30, "KILL");
				}
			}
			else if (enemyType == "spider")
			{
				AddPoints(60, "PIMP SLAP");
			}
			else
			{
				AddPoints(20, string.Empty);
			}
			break;
		case "revolver":
			if (dead)
			{
				if (hitLimb == "head")
				{
					AddPoints(80, "<color=orange>HEADSHOT</color>");
				}
				else if (hitLimb == "limb")
				{
					AddPoints(60, "LIMB HIT");
				}
				else if (enemyType == "spider")
				{
					AddPoints(100, "<color=orange>BIG KILL</color>");
				}
				else
				{
					AddPoints(30, "KILL");
				}
			}
			else if (hitLimb == "head")
			{
				AddPoints(50, string.Empty);
			}
			else if (hitLimb == "limb" || enemyType == "spider")
			{
				AddPoints(30, string.Empty);
			}
			else
			{
				AddPoints(20, string.Empty);
			}
			break;
		case "projectile":
			if (dead)
			{
				AddPoints(150, "<color=red>FRIENDLY FIRE</color>");
			}
			else
			{
				AddPoints(80, "<color=orange>FRIENDLY FIRE</color>");
			}
			break;
		}
	}

	private void AddPoints(int points, string pointName)
	{
		int num = Mathf.RoundToInt((float)points * airTime - (float)points);
		shud.AddPoints(points + num, pointName);
	}
}
