using UnityEngine;
using UnityEngine.AI;

public class ZombieMelee : MonoBehaviour
{
	public bool damaging;

	public TrailRenderer tr;

	public bool track;

	private AudioSource aud;

	public float coolDown;

	public Zombie zmb;

	private NavMeshAgent nma;

	private GameObject player;

	private Animator anim;

	private bool customStart;

	private void Start()
	{
		zmb = GetComponent<Zombie>();
		nma = zmb.nma;
		anim = zmb.anim;
		player = zmb.player;
	}

	private void Update()
	{
		if (!customStart)
		{
			customStart = true;
			zmb = GetComponent<Zombie>();
			nma = zmb.nma;
			anim = zmb.anim;
			player = zmb.player;
		}
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		if (zmb.grounded && nma != null && nma.enabled)
		{
			nma.SetDestination(player.transform.position);
			if (nma.isStopped || nma.velocity == Vector3.zero)
			{
				anim.SetBool("Running", false);
			}
			else
			{
				anim.SetBool("Running", true);
			}
		}
		if (damaging)
		{
			base.transform.Translate(Vector3.forward * 40f * Time.deltaTime, Space.Self);
		}
		if (track)
		{
			if (player == null)
			{
				player = zmb.player;
			}
			base.transform.LookAt(new Vector3(player.transform.position.x, base.transform.position.y, player.transform.position.z));
		}
		if (coolDown != 0f)
		{
			if (coolDown - Time.deltaTime > 0f)
			{
				coolDown -= Time.deltaTime;
			}
			else
			{
				coolDown = 0f;
			}
		}
	}

	public void Swing()
	{
		if (aud == null)
		{
			aud = GetComponentInChildren<SwingCheck>().GetComponent<AudioSource>();
		}
		if (nma == null)
		{
			nma = zmb.nma;
		}
		track = true;
		coolDown = 1.5f;
		nma.isStopped = true;
		anim.SetTrigger("Swing");
	}

	public void SwingEnd()
	{
		nma.isStopped = false;
		tr.enabled = false;
	}

	public void DamageStart()
	{
		damaging = true;
		aud.Play();
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.enabled = true;
	}

	public void DamageEnd()
	{
		damaging = false;
	}

	public void StopTracking()
	{
		track = false;
	}
}
