using UnityEngine;

public class RevolverBeam : MonoBehaviour
{
	private Revolver rev;

	private SecondaryRevolver secRev;

	private LineRenderer lr;

	public int beamType;

	private AudioSource aud;

	private void Awake()
	{
		if (beamType < 2)
		{
			rev = GameObject.FindWithTag("MainCamera").GetComponentInChildren<Revolver>();
		}
		else if (beamType == 2)
		{
			secRev = GameObject.FindWithTag("MainCamera").GetComponentInChildren<SecondaryRevolver>();
		}
		lr = GetComponent<LineRenderer>();
		if (beamType < 3)
		{
			lr.SetPosition(0, base.transform.position);
			if (beamType == 0)
			{
				lr.SetPosition(1, rev.shotHitPoint);
			}
			else if (beamType == 1)
			{
				lr.SetPosition(1, rev.beamReflectPos);
				aud = GetComponent<AudioSource>();
				aud.pitch = Random.Range(0.55f, 0.75f);
				aud.Play();
			}
			else if (beamType == 2)
			{
				lr.SetPosition(1, secRev.shotHitPoint);
			}
		}
	}

	private void Update()
	{
		lr.widthMultiplier -= Time.deltaTime;
		if (lr.widthMultiplier <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
