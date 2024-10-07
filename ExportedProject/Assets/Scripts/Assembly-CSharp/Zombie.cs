using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
	public bool spawnIn;

	public GameObject spawnEffect;

	public float health;

	private float originalHealth;

	private Rigidbody[] rbs;

	public bool limp;

	public GameObject player;

	public NavMeshAgent nma;

	public Animator anim;

	private float currentSpeed;

	private Rigidbody rb;

	private ZombieMelee zm;

	private ZombieProjectiles zp;

	private AudioSource aud;

	public AudioClip[] hurtSounds;

	public float hurtSoundVol;

	public AudioClip deathSound;

	public float deathSoundVol;

	private GroundCheck gc;

	public bool grounded;

	private float defaultSpeed;

	public Vector3 agentVelocity;

	public GameObject bodyBlood;

	public GameObject limbBlood;

	public GameObject headBlood;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject jawHalf;

	public GameObject brainChunk;

	public GameObject[] giblet;

	private bool customStart;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	private void Start()
	{
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		nma = GetComponent<NavMeshAgent>();
		rb = GetComponent<Rigidbody>();
		zm = GetComponent<ZombieMelee>();
		zp = GetComponent<ZombieProjectiles>();
		anim = GetComponent<Animator>();
		gc = GetComponentInChildren<GroundCheck>();
		if (spawnIn)
		{
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
			Object.Instantiate(spawnEffect, position, base.transform.rotation);
		}
		originalHealth = health;
	}

	private void Update()
	{
		if (!customStart)
		{
			customStart = true;
			nma = GetComponent<NavMeshAgent>();
			player = GameObject.FindWithTag("Player");
			rbs = GetComponentsInChildren<Rigidbody>();
			rb = GetComponent<Rigidbody>();
			zm = GetComponent<ZombieMelee>();
			zp = GetComponent<ZombieProjectiles>();
			anim = GetComponent<Animator>();
			gc = GetComponentInChildren<GroundCheck>();
			nma.radius = Random.Range(2, 5);
			defaultSpeed = nma.speed;
		}
		if (grounded && nma != null && nma.enabled)
		{
			if (nma.isStopped || nma.velocity == Vector3.zero)
			{
				anim.speed = 1f;
			}
			else
			{
				anim.speed = nma.velocity.magnitude / nma.speed;
			}
		}
		if (anim != null)
		{
			anim.SetFloat("Speed", anim.speed);
		}
	}

	private void FixedUpdate()
	{
		if (!limp)
		{
			if (grounded && nma.isOnOffMeshLink)
			{
				grounded = false;
				nma.speed = 8f;
				base.transform.position = base.transform.position + Vector3.up + base.transform.forward;
				rb.AddForce(base.transform.up + agentVelocity * 10f, ForceMode.VelocityChange);
			}
			else if (!grounded && gc.onGround)
			{
				grounded = true;
				nma.speed = defaultSpeed;
			}
		}
	}

	public void GetHurt(GameObject target, int bulletForce, Vector3 hurtPoint, float multiplier)
	{
		string hitLimb = string.Empty;
		bool dead = false;
		bool flag = false;
		if (target.gameObject.tag == "Head")
		{
			health -= 3f * multiplier;
			Object.Instantiate(headBlood, target.transform.position, Quaternion.identity);
			Vector3 normalized = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "head";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.transform.parent.GetComponentInParent<Rigidbody>() != null)
				{
					target.transform.parent.GetComponentInParent<Rigidbody>().AddForceAtPosition((player.transform.position - target.transform.position).normalized * -1f * bulletForce, hurtPoint);
				}
				for (int i = 0; i < 6; i++)
				{
					Object.Instantiate(skullFragment, target.transform.position, Random.rotation);
				}
				for (int j = 0; j < 4; j++)
				{
					Object.Instantiate(brainChunk, target.transform.position, Random.rotation);
				}
				for (int k = 0; k < 2; k++)
				{
					Object.Instantiate(eyeBall, target.transform.position, Random.rotation);
					Object.Instantiate(jawHalf, target.transform.position, Random.rotation);
				}
			}
			else
			{
				nma.Warp(base.transform.position - normalized);
			}
		}
		else if (target.gameObject.tag == "Limb" || target.gameObject.tag == "EndLimb")
		{
			health -= 2f * multiplier;
			Object.Instantiate(limbBlood, target.transform.position, Quaternion.identity);
			Vector3 normalized2 = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "limb";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.gameObject.tag == "Limb")
				{
					for (int l = 0; l < 2; l++)
					{
						Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation);
					}
				}
				else
				{
					target.transform.localScale = Vector3.zero;
				}
			}
			else
			{
				nma.Warp(base.transform.position - normalized2);
			}
		}
		else if (target.gameObject.tag == "Body")
		{
			health -= 1f * multiplier;
			if (multiplier >= 1f || health <= 0f)
			{
				Object.Instantiate(bodyBlood, target.transform.position, Quaternion.identity);
			}
			Vector3 normalized3 = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "body";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.GetComponentInParent<Rigidbody>() != null)
				{
					target.GetComponentInParent<Rigidbody>().AddForceAtPosition((player.transform.position - target.transform.position).normalized * -1f * bulletForce, hurtPoint);
				}
			}
			else
			{
				nma.Warp(base.transform.position - normalized3);
			}
		}
		if (health <= 0f && (target.gameObject.tag == "Limb" || target.gameObject.tag == "Head"))
		{
			if (target.transform.childCount > 0)
			{
				Transform child = target.transform.GetChild(0);
				CharacterJoint[] componentsInChildren = target.GetComponentsInChildren<CharacterJoint>();
				if (componentsInChildren.Length > 0)
				{
					CharacterJoint[] array = componentsInChildren;
					foreach (CharacterJoint obj in array)
					{
						Object.Destroy(obj);
					}
				}
				CharacterJoint component = target.GetComponent<CharacterJoint>();
				if (component != null)
				{
					component.connectedBody = null;
					Object.Destroy(component);
				}
				target.transform.DetachChildren();
				target.transform.position = child.position;
				target.transform.SetParent(child);
				Object.Destroy(target.GetComponent<Rigidbody>());
			}
			Object.Destroy(target.GetComponent<Collider>());
			target.transform.localScale = Vector3.zero;
		}
		else if (health <= 0f && target.gameObject.tag == "EndLimb")
		{
			target.transform.localScale = Vector3.zero;
		}
		if (health > 0f && hurtSounds.Length > 0)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
			aud.volume = hurtSoundVol;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 12;
			aud.Play();
		}
		if (flag)
		{
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
				dead = true;
			}
			scalc.HitCalculator(eid.hitter, "zombie", hitLimb, dead, base.gameObject);
		}
	}

	public void GoLimp()
	{
		if (zm != null)
		{
			zm.track = false;
			if (zm.tr != null)
			{
				zm.tr.enabled = false;
			}
			Object.Destroy(zm);
		}
		if (zp != null)
		{
			if (zp.tr != null)
			{
				zp.tr.enabled = false;
			}
			Object.Destroy(zp);
		}
		customStart = true;
		Object.Destroy(nma);
		nma = null;
		Object.Destroy(anim);
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		Object.Destroy(base.gameObject.GetComponentInChildren<SwingCheck>());
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		Object.Destroy(rb);
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		if (deathSound != null)
		{
			aud.clip = deathSound;
			aud.volume = deathSoundVol;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 11;
			aud.Play();
		}
		if (!limp)
		{
			Rigidbody[] array = rbs;
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.isKinematic = false;
				rigidbody.useGravity = true;
			}
		}
		if (!limp)
		{
			ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
			if (componentInParent != null)
			{
				componentInParent.deadEnemies++;
			}
		}
		limp = true;
	}
}
