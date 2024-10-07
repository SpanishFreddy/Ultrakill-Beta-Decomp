using System;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : MonoBehaviour
{
	public class RaycastResult : IComparable<RaycastResult>
	{
		public float distance;

		public Transform transform;

		public RaycastHit rrhit;

		public RaycastResult(RaycastHit hit)
		{
			distance = hit.distance;
			transform = hit.transform;
			rrhit = hit;
		}

		public int CompareTo(RaycastResult other)
		{
			return distance.CompareTo(other.distance);
		}
	}

	public int gunVariation;

	public Transform kickBackPos;

	public Transform pickUpPos;

	private Vector3 defaultGunPos;

	private Quaternion defaultGunRot;

	private AudioSource gunAud;

	private AudioSource superGunAud;

	public AudioClip[] gunShots;

	public AudioClip[] superGunShots;

	private int currentGunShot;

	private GameObject gunBarrel;

	private bool gunReady;

	private bool shootReady = true;

	private bool pierceReady = true;

	public float shootCharge;

	public float pierceCharge;

	public LayerMask pierceLayerMask;

	public LayerMask enemyLayerMask;

	private bool chargingPierce;

	public float pierceShotCharge;

	public Vector3 shotHitPoint;

	public GameObject beamPoint;

	public GameObject hitParticle;

	public GameObject superBeamPoint;

	public GameObject ricochetBeamPoint;

	public GameObject reflectedBeam;

	public GameObject superHitParticle;

	public GameObject ricochetHitParticle;

	public RaycastHit hit;

	public RaycastHit[] allHits;

	private int currentHit;

	private int currentHitMultiplier;

	public float recoilFOV;

	public GameObject chargeEffect;

	private AudioSource ceaud;

	private Light celight;

	private GameObject camObj;

	private Camera cam;

	private CameraController cc;

	private Transform camPositioner;

	private Vector3 tempCamPos;

	public Vector3 beamReflectPos;

	public GameObject beamDirectionSetter;

	private MeshRenderer screenMR;

	public Material batteryFull;

	public Material batteryMid;

	public Material batteryLow;

	private AudioSource screenAud;

	public AudioClip chargedSound;

	public AudioClip chargingSound;

	private int bodiesPierced;

	public GameObject blood;

	public GameObject headBlood;

	public GameObject smallBlood;

	public GameObject dripBlood;

	private Enemy enemy;

	private CharacterJoint[] cjs;

	private CharacterJoint cj;

	private GameObject limb;

	private Transform firstChild;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject[] giblet;

	public GameObject brainChunk;

	public GameObject jawHalf;

	private int bulletForce;

	private bool slowMo;

	private bool timeStopped;

	private float untilTimeResume;

	private int enemiesLeftToHit;

	private int enemiesPierced;

	private RaycastHit subHit;

	private int currentShotType;

	private float damageMultiplier = 1f;

	private List<RaycastResult> hitList = new List<RaycastResult>();

	private SecondaryRevolver secRev;

	private bool twirling;

	private AudioClip chargeEffectSound;

	public AudioClip twirlSound;

	public bool twirlRecovery;

	private GameObject currentDrip;

	public LayerMask ignoreEnemyTrigger;

	private void Awake()
	{
		Debug.Log("Main Awake!");
		gunReady = false;
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		camObj = cam.gameObject;
		cc = camObj.GetComponent<CameraController>();
		camPositioner = camObj.transform.parent.GetChild(5);
		shootCharge = 0f;
		pierceShotCharge = 0f;
		pierceCharge = 0f;
		pierceReady = false;
		shootReady = false;
		gunBarrel = base.transform.GetChild(0).gameObject;
		gunAud = GetComponent<AudioSource>();
		kickBackPos = base.transform.parent.GetChild(1);
		pickUpPos = base.transform.parent.GetChild(2);
		superGunAud = kickBackPos.GetComponent<AudioSource>();
		defaultGunPos = base.transform.localPosition;
		defaultGunRot = base.transform.localRotation;
		screenMR = base.transform.GetChild(1).GetComponent<MeshRenderer>();
		screenAud = screenMR.gameObject.GetComponent<AudioSource>();
		secRev = base.transform.parent.GetChild(3).GetComponent<SecondaryRevolver>();
		chargeEffect = base.transform.GetChild(0).GetChild(0).gameObject;
		ceaud = chargeEffect.GetComponent<AudioSource>();
		celight = chargeEffect.GetComponent<Light>();
		chargeEffectSound = ceaud.clip;
		base.transform.localPosition = pickUpPos.localPosition;
		base.transform.localRotation = pickUpPos.localRotation;
		Debug.Log(defaultGunRot);
		if (gunVariation != 2)
		{
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			if (gunVariation == 0)
			{
				screenAud.pitch = 1f;
			}
			else if (gunVariation == 1)
			{
				screenAud.pitch = 1.25f;
			}
			screenAud.volume = 0.25f;
			screenAud.Play();
		}
		if (gunVariation == 2)
		{
			secRev.gameObject.SetActive(true);
			secRev.enabled = true;
		}
	}

	private void Update()
	{
		if (gunVariation == 2)
		{
			pierceCharge = shootCharge;
		}
		if (base.transform.localPosition != defaultGunPos && base.transform.localRotation != defaultGunRot)
		{
			gunReady = false;
		}
		else
		{
			gunReady = true;
		}
		if (base.transform.localRotation.eulerAngles == defaultGunRot.eulerAngles)
		{
			if (ceaud.volume != 0f)
			{
				ceaud.volume = 0f;
			}
			twirlRecovery = false;
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (!slowMo)
			{
				Time.timeScale *= 0.1f;
				Time.fixedDeltaTime *= 0.1f;
				slowMo = true;
			}
			else
			{
				Time.timeScale *= 10f;
				Time.fixedDeltaTime *= 10f;
				slowMo = false;
			}
		}
		if (!shootReady)
		{
			if (gunVariation != 0)
			{
				if (shootCharge + 175f * Time.deltaTime < 100f)
				{
					shootCharge += 175f * Time.deltaTime;
				}
				else
				{
					shootCharge = 100f;
					shootReady = true;
				}
			}
			else if (shootCharge + 200f * Time.deltaTime < 100f)
			{
				shootCharge += 200f * Time.deltaTime;
			}
			else
			{
				shootCharge = 100f;
				shootReady = true;
			}
		}
		if (!pierceReady)
		{
			if (gunVariation == 0)
			{
				if (pierceCharge + 40f * Time.deltaTime < 100f)
				{
					pierceCharge += 40f * Time.deltaTime;
				}
				else
				{
					pierceCharge = 100f;
					pierceReady = true;
					screenAud.clip = chargedSound;
					screenAud.loop = false;
					screenAud.volume = 0.75f;
					screenAud.pitch = UnityEngine.Random.Range(2f, 2.1f);
					screenAud.Play();
				}
			}
			else if (gunVariation == 1)
			{
				if (pierceCharge + 80f * Time.deltaTime < 100f)
				{
					pierceCharge += 80f * Time.deltaTime;
				}
				else
				{
					pierceCharge = 100f;
					pierceReady = true;
					screenAud.clip = chargedSound;
					screenAud.loop = false;
					screenAud.volume = 0.75f;
					screenAud.pitch = UnityEngine.Random.Range(2f, 2.1f);
					screenAud.Play();
				}
			}
			if (pierceCharge < 50f)
			{
				screenMR.material = batteryLow;
			}
			else if (pierceCharge < 100f)
			{
				screenMR.material = batteryMid;
			}
			else
			{
				screenMR.material = batteryFull;
			}
		}
		if (gunVariation == 0 && gunReady)
		{
			if (Input.GetButtonDown("Fire1") && shootReady && pierceShotCharge == 100f)
			{
				Shoot(2);
				pierceShotCharge = 0f;
			}
			else if (Input.GetButtonDown("Fire1") && shootReady && !chargingPierce)
			{
				Shoot(1);
			}
			else if (Input.GetButton("Fire2") && shootReady && pierceReady)
			{
				chargingPierce = true;
				if (pierceShotCharge + 175f * Time.deltaTime < 100f)
				{
					pierceShotCharge += 175f * Time.deltaTime;
				}
				else
				{
					pierceShotCharge = 100f;
				}
			}
			else
			{
				chargingPierce = false;
				if (pierceShotCharge - 175f * Time.deltaTime > 0f)
				{
					pierceShotCharge -= 175f * Time.deltaTime;
				}
				else
				{
					pierceShotCharge = 0f;
				}
			}
		}
		else if (gunVariation == 1)
		{
			if (Input.GetButton("Fire2"))
			{
				Twirl();
				twirling = true;
			}
			else if (gunReady && Input.GetButtonDown("Fire1") && shootReady && pierceCharge == 100f)
			{
				Shoot(4);
				pierceCharge = 0f;
				twirling = false;
				if (ceaud.volume != 0f)
				{
					ceaud.volume = 0f;
				}
				twirlRecovery = false;
			}
			else
			{
				twirling = false;
			}
		}
		else if (gunVariation == 2 && gunReady && Input.GetButtonDown("Fire2") && shootReady)
		{
			Shoot(1);
		}
		if (pierceShotCharge == 0f && celight.enabled)
		{
			celight.enabled = false;
		}
		else if (pierceShotCharge != 0f)
		{
			celight.enabled = true;
			celight.range = pierceShotCharge * 0.01f;
		}
		chargeEffect.transform.localScale = Vector3.one * pierceShotCharge * 0.02f;
		if (gunVariation == 0)
		{
			ceaud.volume = pierceShotCharge * 0.01f;
			ceaud.pitch = pierceShotCharge * 0.005f;
		}
		if (base.transform.localPosition != defaultGunPos)
		{
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, defaultGunPos, Time.deltaTime * 3f);
		}
		if (base.transform.localRotation != defaultGunRot && !twirling && twirlRecovery)
		{
			if (base.transform.localRotation.eulerAngles.z <= defaultGunRot.eulerAngles.z)
			{
				base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, defaultGunRot, Time.deltaTime * 1000f);
			}
			else
			{
				base.transform.Rotate(0f, 0f, Time.deltaTime * -1000f, Space.Self);
			}
		}
		else if (base.transform.localRotation != defaultGunRot && !twirling)
		{
			base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, defaultGunRot, Time.deltaTime * 160f);
		}
	}

	private void Shoot(int shotType)
	{
		if (camPositioner == null)
		{
			camPositioner = camObj.transform.GetChild(5);
		}
		switch (shotType)
		{
		case 1:
			cc.StopShake();
			bulletForce = 5000;
			Physics.Raycast(camPositioner.position, camObj.transform.forward, out hit, float.PositiveInfinity, ignoreEnemyTrigger);
			ExecuteHits(hit);
			shotHitPoint = hit.point;
			UnityEngine.Object.Instantiate(beamPoint, gunBarrel.transform.position, gunBarrel.transform.rotation);
			UnityEngine.Object.Instantiate(hitParticle, hit.point, base.transform.rotation);
			shootReady = false;
			shootCharge = 0f;
			currentGunShot = UnityEngine.Random.Range(0, gunShots.Length);
			gunAud.clip = gunShots[currentGunShot];
			gunAud.volume = 0.5f;
			gunAud.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			gunAud.Play();
			cam.fieldOfView = (recoilFOV - cam.fieldOfView) / 2f + cam.fieldOfView;
			break;
		case 2:
		{
			cc.StopShake();
			bulletForce = 20000;
			Physics.Raycast(camPositioner.position, camObj.transform.forward, out hit, float.PositiveInfinity, pierceLayerMask);
			shotHitPoint = hit.point;
			UnityEngine.Object.Instantiate(superBeamPoint, gunBarrel.transform.position, gunBarrel.transform.rotation);
			UnityEngine.Object.Instantiate(superHitParticle, hit.point, base.transform.rotation);
			beamDirectionSetter.transform.position = camObj.transform.position;
			beamDirectionSetter.transform.LookAt(hit.point);
			bodiesPierced = 0;
			Ray ray = new Ray(camObj.transform.position, camObj.transform.forward);
			allHits = Physics.RaycastAll(ray, hit.distance);
			enemiesPierced = 0;
			currentShotType = shotType;
			PiercingShotOrder();
			shootReady = false;
			shootCharge = 0f;
			pierceReady = false;
			pierceCharge = 0f;
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			screenAud.pitch = 1f;
			screenAud.volume = 0.25f;
			screenAud.Play();
			currentGunShot = UnityEngine.Random.Range(0, superGunShots.Length);
			superGunAud.clip = superGunShots[currentGunShot];
			superGunAud.volume = 0.5f;
			superGunAud.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			superGunAud.Play();
			cam.fieldOfView = recoilFOV + 10f;
			break;
		}
		case 4:
			cc.StopShake();
			Debug.ClearDeveloperConsole();
			bulletForce = 5000;
			Physics.Raycast(camPositioner.position, camObj.transform.forward, out hit, float.PositiveInfinity, ignoreEnemyTrigger);
			if (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Floor" || hit.transform.gameObject.tag == "Glass" || hit.transform.gameObject.tag == "GlassFloor")
			{
				bodiesPierced = 0;
				tempCamPos = Vector3.Reflect(camObj.transform.forward, hit.normal);
				Physics.Raycast(hit.point, tempCamPos, out subHit, float.PositiveInfinity, pierceLayerMask);
				beamReflectPos = subHit.point;
				UnityEngine.Object.Instantiate(reflectedBeam, hit.point, base.transform.rotation);
				beamDirectionSetter.transform.position = hit.point;
				beamDirectionSetter.transform.LookAt(beamReflectPos);
				allHits = Physics.BoxCastAll(beamDirectionSetter.transform.position, Vector3.one * 0.3f, beamDirectionSetter.transform.forward, beamDirectionSetter.transform.rotation, hit.distance, enemyLayerMask);
				currentShotType = shotType;
				PiercingShotOrder();
			}
			else
			{
				ExecuteHits(hit);
			}
			shotHitPoint = hit.point;
			UnityEngine.Object.Instantiate(beamPoint, gunBarrel.transform.position, gunBarrel.transform.rotation);
			UnityEngine.Object.Instantiate(hitParticle, hit.point, base.transform.rotation);
			shootReady = false;
			shootCharge = 0f;
			pierceReady = false;
			pierceCharge = 0f;
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			screenAud.pitch = 1.25f;
			screenAud.volume = 0.25f;
			screenAud.Play();
			currentGunShot = UnityEngine.Random.Range(0, gunShots.Length);
			gunAud.clip = gunShots[currentGunShot];
			gunAud.volume = 0.5f;
			gunAud.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			gunAud.Play();
			cam.fieldOfView = recoilFOV;
			break;
		}
		base.transform.localPosition = kickBackPos.localPosition;
		base.transform.localRotation = kickBackPos.localRotation;
	}

	private void PiercingShotOrder()
	{
		hitList.Clear();
		RaycastHit[] array = allHits;
		foreach (RaycastHit raycastHit in array)
		{
			hitList.Add(new RaycastResult(raycastHit));
		}
		if (gunVariation == 1)
		{
			hitList.Add(new RaycastResult(subHit));
		}
		else
		{
			hitList.Add(new RaycastResult(hit));
		}
		hitList.Sort();
		PiercingShotCheck();
	}

	private void PiercingShotCheck()
	{
		if (enemiesPierced < hitList.Count)
		{
			if ((hitList[enemiesPierced].transform.gameObject.tag == "Limb" || hitList[enemiesPierced].transform.gameObject.tag == "Head" || hitList[enemiesPierced].transform.gameObject.tag == "EndLimb" || hitList[enemiesPierced].transform.gameObject.tag == "Body") && bodiesPierced < 3)
			{
				EnemyIdentifier eid = hitList[enemiesPierced].transform.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
				if (eid != null)
				{
					if (eid.type == EnemyType.Zombie && !eid.zombie.limp)
					{
						UnityEngine.Object.Instantiate(superHitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
						Debug.Log(hitList[enemiesPierced].transform.gameObject);
						ExecuteHits(hitList[enemiesPierced].rrhit);
						bodiesPierced++;
						cc.HitStop(0.05f);
						if (eid.zombie.health <= 0f)
						{
							enemiesPierced++;
						}
						Invoke("PiercingShotCheck", 0.05f);
					}
					else if (eid.type == EnemyType.Spider)
					{
						UnityEngine.Object.Instantiate(superHitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
						ExecuteHits(hitList[enemiesPierced].rrhit);
						bodiesPierced++;
						cc.HitStop(0.05f);
						Invoke("PiercingShotCheck", 0.05f);
					}
					else
					{
						ExecuteHits(hitList[enemiesPierced].rrhit);
						enemiesPierced++;
						PiercingShotCheck();
					}
				}
				else if (hitList[enemiesPierced].transform.gameObject.GetComponentInParent<SpiderBody>() != null)
				{
					UnityEngine.Object.Instantiate(superHitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
					ExecuteHits(hitList[enemiesPierced].rrhit);
					bodiesPierced++;
					cc.HitStop(0.05f);
					if (hitList[enemiesPierced].rrhit.transform == null)
					{
						enemiesPierced++;
					}
					Invoke("PiercingShotCheck", 0.05f);
				}
				else
				{
					ExecuteHits(hitList[enemiesPierced].rrhit);
					enemiesPierced++;
					PiercingShotCheck();
				}
			}
			else if (currentShotType == 4 && (hitList[enemiesPierced].transform.gameObject.tag == "Wall" || hitList[enemiesPierced].transform.gameObject.tag == "Floor") && bodiesPierced < 2)
			{
				Debug.Log("Got here");
				Invoke("ReflectCheck", 0.1f);
			}
			else if (currentShotType == 2 && (hitList[enemiesPierced].transform.gameObject.tag == "Glass" || hitList[enemiesPierced].transform.gameObject.tag == "GlassFloor"))
			{
				Glass component = hitList[enemiesPierced].transform.gameObject.GetComponent<Glass>();
				if (!component.broken)
				{
					component.Shatter();
				}
				enemiesPierced++;
				PiercingShotCheck();
			}
			else
			{
				enemiesPierced++;
				PiercingShotCheck();
			}
		}
		else
		{
			enemiesPierced = 0;
		}
	}

	private void ReflectCheck()
	{
		tempCamPos = Vector3.Reflect(beamDirectionSetter.transform.forward, hitList[enemiesPierced].rrhit.normal);
		Physics.Raycast(hitList[enemiesPierced].rrhit.point, tempCamPos, out subHit, float.PositiveInfinity, pierceLayerMask);
		beamReflectPos = subHit.point;
		UnityEngine.Object.Instantiate(reflectedBeam, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
		beamDirectionSetter.transform.position = hitList[enemiesPierced].rrhit.point;
		beamDirectionSetter.transform.LookAt(beamReflectPos);
		allHits = Physics.BoxCastAll(beamDirectionSetter.transform.position, Vector3.one * 0.3f, beamDirectionSetter.transform.forward, beamDirectionSetter.transform.rotation, hit.distance, enemyLayerMask);
		Debug.Log(allHits);
		bodiesPierced++;
		PiercingShotOrder();
	}

	public void ExecuteHits(RaycastHit currentHit)
	{
		if (currentHit.transform != null && (currentHit.transform.gameObject.tag == "Body" || currentHit.transform.gameObject.tag == "Limb" || currentHit.transform.gameObject.tag == "EndLimb" || currentHit.transform.gameObject.tag == "Head"))
		{
			cc.CameraShake(0.5f);
			EnemyIdentifier eid = currentHit.transform.GetComponent<EnemyIdentifierIdentifier>().eid;
			eid.rhit = currentHit;
			eid.hitter = "revolver";
			eid.DeliverDamage(currentHit.transform.gameObject, bulletForce, currentHit.point, damageMultiplier);
		}
	}

	private void Twirl()
	{
		twirlRecovery = true;
		if (ceaud.volume != 0.5f)
		{
			ceaud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
			ceaud.volume = 0.5f;
		}
		base.transform.Rotate(0f, 0f, Time.deltaTime * -1000f, Space.Self);
	}

	private void ReadyToShoot()
	{
		shootReady = true;
	}

	public void SwitchVariation(int variation)
	{
		gunVariation = variation;
		shootCharge = 0f;
		pierceCharge = 0f;
		pierceShotCharge = 0f;
		pierceReady = false;
		shootReady = false;
		if (gunVariation != 2)
		{
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			if (gunVariation == 0)
			{
				twirling = false;
				screenAud.pitch = 1f;
				ceaud.clip = chargeEffectSound;
				ceaud.volume = 0f;
				ceaud.Play();
			}
			else if (gunVariation == 1)
			{
				screenAud.pitch = 1.25f;
				ceaud.clip = twirlSound;
				ceaud.volume = 0f;
				ceaud.pitch = 1f;
				ceaud.Play();
			}
			screenAud.volume = 0.25f;
			screenAud.Play();
			secRev.gameObject.SetActive(false);
			secRev.enabled = false;
		}
		else
		{
			screenAud.volume = 0f;
			twirling = false;
			secRev.gameObject.SetActive(true);
			secRev.enabled = true;
			secRev.PickUp();
		}
		base.transform.localPosition = pickUpPos.localPosition;
		base.transform.localRotation = pickUpPos.localRotation;
	}

	public void Punch()
	{
		base.transform.localPosition = kickBackPos.localPosition;
		base.transform.localRotation = kickBackPos.localRotation;
	}
}
