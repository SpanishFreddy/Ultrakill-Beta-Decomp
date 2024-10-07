using UnityEngine;

public class Punch : MonoBehaviour
{
	private bool ready = true;

	private Animator anim;

	private SkinnedMeshRenderer smr;

	private Revolver rev;

	private AudioSource aud;

	private AudioSource aud2;

	private GameObject camObj;

	private CameraController cc;

	private RaycastHit hit;

	public LayerMask deflectionLayerMask;

	public LayerMask ignoreEnemyTrigger;

	private NewMovement nmov;

	private TrailRenderer tr;

	private Light parryLight;

	private GameObject currentDustParticle;

	public GameObject dustParticle;

	public GameObject parryFlash;

	public AudioClip normalHit;

	public AudioClip heavyHit;

	public AudioClip specialHit;

	private StyleHUD shud;

	private void Start()
	{
		anim = GetComponent<Animator>();
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		smr.enabled = false;
		rev = base.transform.parent.parent.GetComponentInChildren<Revolver>();
		camObj = GameObject.FindWithTag("MainCamera");
		cc = camObj.GetComponent<CameraController>();
		aud = GetComponent<AudioSource>();
		aud2 = base.transform.GetChild(2).GetComponent<AudioSource>();
		nmov = GetComponentInParent<NewMovement>();
		tr = GetComponentInChildren<TrailRenderer>();
		tr.enabled = false;
		parryLight = aud2.GetComponent<Light>();
		shud = camObj.GetComponentInChildren<StyleHUD>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F) && ready)
		{
			PunchStart();
		}
	}

	private void PunchStart()
	{
		ready = false;
		smr.enabled = true;
		anim.SetTrigger("Punch");
		rev.Punch();
		aud.Play();
		tr.enabled = true;
	}

	private void ActiveStart()
	{
		if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hit, 3f, ignoreEnemyTrigger))
		{
			if (hit.transform.gameObject.layer == 8)
			{
				aud2.clip = normalHit;
				aud2.Play();
				currentDustParticle = Object.Instantiate(dustParticle, hit.point, base.transform.rotation);
				currentDustParticle.transform.forward = hit.normal;
			}
			else if (hit.transform.gameObject.tag == "Head" || hit.transform.gameObject.tag == "Body" || hit.transform.gameObject.tag == "Limb" || hit.transform.gameObject.tag == "EndLimb")
			{
				cc.HitStop(0.05f);
				cc.CameraShake(0.5f);
				EnemyIdentifier eid = hit.transform.GetComponent<EnemyIdentifierIdentifier>().eid;
				eid.rhit = hit;
				eid.hitter = "punch";
				eid.DeliverDamage(hit.transform.gameObject, 10000, hit.point, 0.5f);
				aud2.clip = heavyHit;
				aud2.Play();
			}
		}
		if (!Physics.BoxCast(camObj.transform.position, new Vector3(1f, 1f, 0.01f), camObj.transform.forward, out hit, camObj.transform.rotation, 3f, deflectionLayerMask) || hit.transform.gameObject.layer != 14)
		{
			return;
		}
		aud2.clip = specialHit;
		aud2.Play();
		if (hit.transform.gameObject.GetComponent<Projectile>() != null)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hitInfo, float.PositiveInfinity, ignoreEnemyTrigger))
			{
				hit.transform.LookAt(hitInfo.point);
			}
			else
			{
				hit.transform.LookAt(camObj.transform.position + camObj.transform.forward * 10f);
			}
			hit.transform.gameObject.GetComponent<Projectile>().friendly = true;
			hit.transform.gameObject.GetComponent<Projectile>().speed *= 2f;
			nmov.GetHealth(25, true);
			ParryFlash();
			parryFlash.GetComponent<AudioSource>().Play();
			cc.TrueStop(0.25f);
			cc.CameraShake(0.5f);
			shud.AddPoints(100, "<color=cyan>PARRY</color>");
		}
		else if (hit.transform.gameObject.GetComponent<Bonus>() != null)
		{
			ParryFlash();
			parryFlash.GetComponent<AudioSource>().Play();
			cc.TrueStop(0.25f);
			cc.CameraShake(0.5f);
			hit.transform.gameObject.GetComponent<Bonus>().Break();
			shud.AddPoints(0, "<color=cyan>BONUS</color>");
		}
	}

	private void ActiveEnd()
	{
	}

	private void PunchEnd()
	{
		smr.enabled = false;
		ready = true;
		tr.enabled = false;
	}

	private void ParryFlash()
	{
		parryLight.enabled = true;
		parryFlash.SetActive(true);
		Invoke("hideFlash", 0.1f);
	}

	private void hideFlash()
	{
		parryLight.enabled = false;
		parryFlash.SetActive(false);
	}
}
