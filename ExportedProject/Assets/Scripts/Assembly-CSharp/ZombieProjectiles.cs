using UnityEngine;
using UnityEngine.AI;

public class ZombieProjectiles : MonoBehaviour
{
	public bool stationary;

	private Zombie zmb;

	private GameObject player;

	private GameObject camObj;

	private NavMeshAgent nma;

	private NavMeshHit hit;

	private Animator anim;

	public Vector3 targetPosition;

	private float coolDown;

	private AudioSource aud;

	public TrailRenderer tr;

	public GameObject projectile;

	private GameObject currentProjectile;

	public Transform shootPos;

	public GameObject head;

	private bool playerSpotted;

	private RaycastHit rhit;

	private RaycastHit bhit;

	public LayerMask lookForPlayerMask;

	private bool seekingPlayer;

	private float raySize = 1f;

	private void Start()
	{
		zmb = GetComponent<Zombie>();
		player = GameObject.FindWithTag("Player");
		camObj = GameObject.FindWithTag("MainCamera");
		nma = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		if (stationary)
		{
			raySize = 0.25f;
		}
	}

	private void Update()
	{
		if (zmb.limp)
		{
			return;
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
		Vector3 normalized = (camObj.transform.position - head.transform.position).normalized;
		if (playerSpotted && Physics.BoxCast(head.transform.position, Vector3.one * raySize, normalized, out bhit, Quaternion.identity, float.PositiveInfinity, lookForPlayerMask))
		{
			if (bhit.transform.gameObject.tag == "Player")
			{
				nma.SetDestination(base.transform.position);
				if (coolDown == 0f && nma.velocity == Vector3.zero)
				{
					Swing();
				}
			}
			else if (!stationary)
			{
				nma.SetDestination(player.transform.position);
			}
		}
		else if (!playerSpotted && Physics.Raycast(head.transform.position, normalized, out rhit, float.PositiveInfinity, lookForPlayerMask) && rhit.transform.gameObject.tag == "Player")
		{
			playerSpotted = true;
			coolDown = Random.Range(1, 2);
		}
		Vector3 vector = player.transform.position - base.transform.position;
		if (Vector3.Distance(player.transform.position, base.transform.position) < 15f)
		{
			nma.updateRotation = true;
			targetPosition = new Vector3(base.transform.position.x + vector.normalized.x * -10f, base.transform.position.y, base.transform.position.z + vector.normalized.z * -10f);
			nma.SetDestination(targetPosition);
			if (NavMesh.SamplePosition(targetPosition, out hit, 1f, nma.areaMask))
			{
				nma.SetDestination(targetPosition);
			}
			else
			{
				NavMesh.FindClosestEdge(targetPosition, out hit, nma.areaMask);
				targetPosition = hit.position;
				nma.SetDestination(targetPosition);
			}
		}
		if (nma.velocity == Vector3.zero && playerSpotted)
		{
			anim.SetBool("Running", false);
			nma.updateRotation = false;
			base.transform.LookAt(new Vector3(player.transform.position.x, base.transform.position.y, player.transform.position.z));
		}
		else if (nma.velocity != Vector3.zero)
		{
			anim.SetBool("Running", true);
		}
	}

	public void Swing()
	{
		coolDown = Random.Range(1f, 2.5f);
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
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.enabled = true;
		currentProjectile = Object.Instantiate(projectile, shootPos.position, base.transform.rotation);
		currentProjectile.transform.LookAt(camObj.transform);
	}

	public void StopTracking()
	{
	}

	public void DamageEnd()
	{
	}
}
