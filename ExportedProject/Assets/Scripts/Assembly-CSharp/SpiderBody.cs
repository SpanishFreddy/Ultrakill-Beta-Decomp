using UnityEngine;
using UnityEngine.AI;

public class SpiderBody : MonoBehaviour
{
	private Rigidbody[] rbs;

	public bool limp;

	private GameObject player;

	private NewMovement nmov;

	private Revolver rev;

	private NavMeshAgent nma;

	private Quaternion followPlayerRot;

	public GameObject proj;

	private RaycastHit hit;

	private RaycastHit hit2;

	public LayerMask aimlm;

	private bool readyToShoot = true;

	private float burstCharge;

	private int currentBurst;

	public float health;

	private bool dead;

	private Rigidbody rb;

	private bool falling;

	private Enemy enemy;

	private Transform firstChild;

	private CharacterJoint[] cjs;

	private CharacterJoint cj;

	private Transform[] bodyChild;

	public GameObject impactParticle;

	public GameObject impactSprite;

	private Quaternion spriteRot;

	private Vector3 spritePos;

	private Transform mouth;

	private GameObject currentProj;

	private bool charging;

	public GameObject chargeEffect;

	private GameObject currentCE;

	private float beamCharge;

	private AudioSource ceAud;

	private Light ceLight;

	private Vector3 predictedPlayerPos;

	public GameObject spiderBeam;

	private GameObject currentBeam;

	public GameObject beamExplosion;

	private GameObject currentExplosion;

	private float beamProbability;

	private Quaternion predictedRot;

	private bool rotating;

	public GameObject dripBlood;

	private GameObject currentDrip;

	public GameObject smallBlood;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	private void Start()
	{
		burstCharge = 5f;
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		rev = player.GetComponentInChildren<Revolver>();
		nma = GetComponent<NavMeshAgent>();
		nmov = player.GetComponent<NewMovement>();
		mouth = base.transform.GetChild(0);
	}

	private void FixedUpdate()
	{
		if (!dead && !charging && beamCharge == 0f)
		{
			if (!nma.enabled)
			{
				base.transform.rotation = Quaternion.identity;
				nma.enabled = true;
				nma.isStopped = false;
				nma.speed = 3.5f;
			}
			if (nma != null)
			{
				nma.SetDestination(player.transform.position);
			}
			if (currentBurst > 5 && burstCharge == 0f)
			{
				currentBurst = 0;
				burstCharge = 5f;
			}
			if (burstCharge > 0f)
			{
				burstCharge -= 0.1f;
			}
			if (burstCharge < 0f)
			{
				burstCharge = 0f;
			}
			if (readyToShoot && burstCharge == 0f && Physics.Raycast(base.transform.position, base.transform.forward, out hit, 24f, aimlm) && hit.transform != null && hit.transform.gameObject.tag == "Player")
			{
				if (currentBurst != 0)
				{
					ShootProj();
				}
				else if ((float)Random.Range(0, 9) >= beamProbability && beamProbability <= 5f)
				{
					ShootProj();
					beamProbability += 1f;
				}
				else
				{
					ChargeBeam();
					beamProbability = 0f;
				}
			}
		}
		else if (!dead && charging)
		{
			if (beamCharge + Time.deltaTime < 1f)
			{
				nma.speed = 0f;
				nma.SetDestination(base.transform.position);
				nma.isStopped = true;
				beamCharge += Time.deltaTime * 0.5f;
				currentCE.transform.localScale = Vector3.one * beamCharge * 5f;
				ceAud.pitch = beamCharge * 3f;
				ceLight.intensity = beamCharge * 30f;
			}
			else if (beamCharge + Time.deltaTime >= 1f)
			{
				beamCharge = 1f;
				charging = false;
				BeamChargeEnd();
			}
		}
	}

	private void Update()
	{
		if (!dead)
		{
			if (beamCharge < 1f)
			{
				base.transform.LookAt(player.transform);
			}
			else if (rotating && beamCharge == 1f)
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, predictedRot, Time.deltaTime * 200f);
			}
			else if (!rotating && beamCharge == 1f)
			{
				predictedRot = Quaternion.LookRotation(player.transform.position - base.transform.position);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, predictedRot, Time.deltaTime * 100f);
			}
		}
	}

	public void GetHurt(GameObject target, int bulletForce, RaycastHit currentHit, float multiplier)
	{
		bool flag = false;
		Object.Instantiate(smallBlood, currentHit.point, Quaternion.identity);
		currentDrip = Object.Instantiate(dripBlood, currentHit.point, Quaternion.Euler(currentHit.normal));
		currentDrip.transform.parent = base.transform;
		if (!dead)
		{
			health -= 1f * multiplier;
			if (scalc == null)
			{
				scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
			}
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			if (health <= 0f)
			{
				flag = true;
			}
			scalc.HitCalculator(eid.hitter, "spider", string.Empty, flag, base.gameObject);
			if (health <= 0f)
			{
				dead = true;
				Die();
			}
		}
	}

	public void Die()
	{
		rb = GetComponent<Rigidbody>();
		falling = true;
		if (rev == null)
		{
			rev = player.GetComponentInChildren<Revolver>();
		}
		rb.isKinematic = false;
		rb.useGravity = true;
		for (int i = 1; i < base.transform.parent.childCount - 1; i++)
		{
			Object.Destroy(base.transform.parent.GetChild(i).gameObject);
		}
		if (currentCE != null)
		{
			Object.Destroy(currentCE);
		}
		Object.Destroy(nma);
		NavMeshObstacle component = GetComponent<NavMeshObstacle>();
		component.enabled = true;
	}

	private void ShootProj()
	{
		currentBurst++;
		currentProj = Object.Instantiate(proj, mouth.position, base.transform.rotation);
		currentProj.transform.LookAt(hit.point);
		readyToShoot = false;
		Invoke("ReadyToShoot", 0.1f);
	}

	private void ChargeBeam()
	{
		charging = true;
		currentCE = Object.Instantiate(chargeEffect, mouth);
		currentCE.transform.localScale = Vector3.zero;
		ceAud = currentCE.GetComponent<AudioSource>();
		ceLight = currentCE.GetComponent<Light>();
	}

	private void BeamChargeEnd()
	{
		ceAud.Stop();
		predictedPlayerPos = player.transform.position + nmov.rb.velocity * 0.5f;
		nma.enabled = false;
		predictedRot = Quaternion.LookRotation(predictedPlayerPos - base.transform.position);
		rotating = true;
		Invoke("BeamFire", 0.5f);
	}

	private void BeamFire()
	{
		if (!dead)
		{
			Object.Destroy(currentCE);
			Physics.Raycast(mouth.position, predictedPlayerPos - mouth.position, out hit2, float.PositiveInfinity, aimlm);
			currentBeam = Object.Instantiate(spiderBeam, mouth.position, mouth.rotation);
			LineRenderer component = currentBeam.GetComponent<LineRenderer>();
			component.SetPosition(0, mouth.position);
			component.SetPosition(1, hit2.point);
			currentExplosion = Object.Instantiate(beamExplosion, hit2.point, Quaternion.identity);
			currentExplosion.transform.forward = hit2.normal;
			rotating = false;
			Invoke("StopWaiting", 1f);
		}
	}

	private void StopWaiting()
	{
		if (!dead)
		{
			beamCharge = 0f;
		}
	}

	private void ReadyToShoot()
	{
		readyToShoot = true;
	}

	public void TriggerHit(Collider other)
	{
		if (!falling || (!(other.gameObject.tag == "Head") && !(other.gameObject.tag == "Body") && !(other.gameObject.tag == "Limb") && !(other.gameObject.tag == "LimbEnd")))
		{
			return;
		}
		enemy = other.transform.gameObject.GetComponentInParent<Enemy>();
		if (enemy != null)
		{
			enemy.GoLimp();
			enemy = null;
		}
		if (other.gameObject.tag == "Body")
		{
			for (int i = 0; i < other.transform.childCount; i++)
			{
				bodyChild = other.transform.GetChild(i).gameObject.GetComponentsInChildren<Transform>();
				Transform[] array = bodyChild;
				foreach (Transform other2 in array)
				{
					Splatter(other2);
				}
			}
			Object.Instantiate(rev.smallBlood, other.transform.position, Quaternion.identity);
		}
		else
		{
			Splatter(other.transform);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Floor")
		{
			rb.isKinematic = true;
			rb.useGravity = false;
			Object.Instantiate(impactParticle, base.transform.position, base.transform.rotation);
			spriteRot.eulerAngles = new Vector3(other.contacts[0].normal.x + 90f, other.contacts[0].normal.y, other.contacts[0].normal.z);
			spritePos = new Vector3(other.contacts[0].point.x, other.contacts[0].point.y + 0.1f, other.contacts[0].point.z);
			Object.Instantiate(impactSprite, spritePos, spriteRot);
			base.transform.position = base.transform.position - base.transform.up * 1.5f;
			falling = false;
			CameraController componentInChildren = player.GetComponentInChildren<CameraController>();
			componentInChildren.CameraShake(2f);
		}
	}

	private void Splatter(Transform other)
	{
		if (!(other.transform.gameObject.tag == "Limb") && !(other.transform.gameObject.tag == "Head") && !(other.transform.gameObject.tag == "EndLimb"))
		{
			return;
		}
		if (other.transform.gameObject.tag == "Head")
		{
			Object.Instantiate(rev.headBlood, other.transform.position, Quaternion.identity);
			for (int i = 0; i < 6; i++)
			{
				Object.Instantiate(rev.skullFragment, other.transform.position, Random.rotation);
			}
			for (int j = 0; j < 4; j++)
			{
				Object.Instantiate(rev.brainChunk, other.transform.position, Random.rotation);
			}
			for (int k = 0; k < 2; k++)
			{
				Object.Instantiate(rev.eyeBall, other.transform.position, Random.rotation);
				Object.Instantiate(rev.jawHalf, other.transform.position, Random.rotation);
			}
		}
		else if (other.transform.gameObject.tag != "EndLimb")
		{
			for (int l = 0; l < 2; l++)
			{
				Object.Instantiate(rev.giblet[Random.Range(0, rev.giblet.Length)], other.transform.position, Random.rotation);
			}
		}
		if (other.transform.gameObject.tag != "Head")
		{
			Object.Instantiate(rev.blood, other.transform.position, Quaternion.identity);
		}
		if (other.transform.gameObject.tag != "EndLimb")
		{
			if (other.transform.childCount > 0)
			{
				firstChild = other.transform.GetChild(0);
				cjs = other.transform.GetComponentsInChildren<CharacterJoint>();
				if (cjs.Length > 0)
				{
					CharacterJoint[] array = cjs;
					foreach (CharacterJoint obj in array)
					{
						Object.Destroy(obj);
					}
				}
				cj = other.transform.GetComponent<CharacterJoint>();
				if (cj != null)
				{
					cj.connectedBody = null;
					Object.Destroy(cj);
				}
				other.transform.DetachChildren();
				other.transform.position = firstChild.position;
				other.transform.SetParent(firstChild);
				Object.Destroy(other.transform.GetComponent<Rigidbody>());
			}
			Object.Destroy(other.transform.GetComponent<Collider>());
			other.transform.localScale = Vector3.zero;
			enemy = null;
			firstChild = null;
		}
		else
		{
			other.transform.localScale = Vector3.zero;
		}
	}
}
