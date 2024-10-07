using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class NewMovement : MonoBehaviour
{
	public float walkSpeed;

	public float jumpPower;

	public float airAcceleration;

	private bool jumpCooldown;

	private bool falling;

	public Rigidbody rb;

	private Vector3 movementDirection;

	private Vector3 movementDirection2;

	private Vector3 airDirection;

	public float timeBetweenSteps;

	private float stepTime;

	private int currentStep;

	public Animator anim;

	private GameObject body;

	private Quaternion tempRotation;

	private GameObject forwardPoint;

	public GroundCheck gc;

	private WallCheck wc;

	private PlayerAnimations pa;

	private Vector3 wallJumpPos;

	private int currentWallJumps;

	private AudioSource aud;

	private AudioSource aud2;

	private AudioSource aud3;

	private int currentSound;

	public AudioClip[] jumpSounds;

	public AudioClip landingSound;

	public AudioClip finalWallJump;

	public bool walking;

	public Slider healthBar;

	public Slider hpAfterImage;

	public Text hpText;

	public int hp = 100;

	public Image hurtScreen;

	private AudioSource hurtAud;

	private Color hurtColor;

	private Color currentColor;

	private bool hurting;

	public bool dead;

	public Image blackScreen;

	private Color blackColor;

	public Text youDiedText;

	private Color youDiedColor;

	public Image greenHpFlash;

	private Color greenHpColor;

	private AudioSource greenHpAud;

	public AudioMixer audmix;

	private float currentAllPitch = 1f;

	private float currentAllVolume;

	public bool boost;

	public Vector3 dodgeDirection;

	private float boostLeft;

	public float boostCharge = 300f;

	public AudioClip dodgeSound;

	public CameraController cc;

	private AudioSource ccAud;

	public Slider[] staminaSliders;

	private GameObject screenHud;

	private Vector3 hudOriginalPos;

	public GameObject dodgeParticle;

	public GameObject scrnBlood;

	private Canvas fullHud;

	private GameObject hudCam;

	private Vector3 camOriginalPos;

	private RigidbodyConstraints defaultRBConstraints;

	private GameObject revolver;

	private StyleHUD shud;

	public GameObject scrapePrefab;

	private GameObject scrapeParticle;

	public LayerMask lmask;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		aud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		body = GameObject.FindWithTag("Body");
		gc = GetComponentInChildren<GroundCheck>();
		wc = GetComponentInChildren<WallCheck>();
		aud2 = gc.GetComponent<AudioSource>();
		pa = GetComponentInChildren<PlayerAnimations>();
		aud3 = wc.GetComponent<AudioSource>();
		cc = GetComponentInChildren<CameraController>();
		ccAud = cc.GetComponent<AudioSource>();
		healthBar.value = hp;
		hpAfterImage.value = hp;
		hpText.text = hp.ToString();
		hurtColor = hurtScreen.color;
		currentColor = hurtColor;
		currentColor.a = 0f;
		hurtScreen.color = currentColor;
		hurtAud = hurtScreen.GetComponent<AudioSource>();
		blackColor = blackScreen.color;
		youDiedColor = youDiedText.color;
		greenHpColor = greenHpFlash.color;
		screenHud = GetComponentInChildren<Canvas>().transform.parent.gameObject;
		fullHud = hurtScreen.GetComponentInParent<Canvas>();
		hudCam = screenHud.GetComponentInParent<Camera>().gameObject;
		hudOriginalPos = screenHud.transform.localPosition;
		camOriginalPos = hudCam.transform.localPosition;
		currentAllPitch = 1f;
		audmix.SetFloat("allPitch", 1f);
		defaultRBConstraints = rb.constraints;
	}

	private void Update()
	{
		float axisRaw = Input.GetAxisRaw("Horizontal");
		float axisRaw2 = Input.GetAxisRaw("Vertical");
		cc.movementHor = axisRaw;
		cc.movementVer = axisRaw2;
		movementDirection = (axisRaw * base.transform.right + axisRaw2 * base.transform.forward).normalized;
		if (Input.GetKeyDown(KeyCode.K))
		{
			GetHurt(100);
		}
		if (dead)
		{
			currentAllPitch -= 0.1f * Time.deltaTime;
			audmix.SetFloat("allPitch", currentAllPitch);
			if (blackColor.a < 0.5f)
			{
				blackColor.a += 0.75f * Time.deltaTime;
				youDiedColor.a += 0.75f * Time.deltaTime;
			}
			else
			{
				blackColor.a += 0.05f * Time.deltaTime;
				youDiedColor.a += 0.05f * Time.deltaTime;
			}
			blackScreen.color = blackColor;
			youDiedText.color = youDiedColor;
		}
		if (gc.onGround != pa.onGround)
		{
			pa.onGround = gc.onGround;
		}
		if (!gc.onGround && rb.velocity.y < -10f)
		{
			falling = true;
		}
		if (!gc.onGround && rb.velocity.y < -20f)
		{
			if (rb.velocity.y > -120f)
			{
				aud3.pitch = rb.velocity.y * -1f / 80f;
			}
			else
			{
				aud3.pitch = 1.5f;
			}
			aud3.volume = rb.velocity.y * -1f / 80f;
		}
		else if (rb.velocity.y > -20f)
		{
			aud3.pitch = 0f;
			aud3.volume = 0f;
		}
		if (gc.onGround && falling)
		{
			falling = false;
			aud2.clip = landingSound;
			aud2.volume = 0.5f;
			aud2.Play();
		}
		if (Input.GetButtonDown("Jump") && gc.onGround && !jumpCooldown)
		{
			Jump();
		}
		if (!gc.onGround && wc.onWall)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, movementDirection, out hitInfo, 1f, lmask))
			{
				if (rb.velocity.y < -1f)
				{
					rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -1f, 1f), -1f, Mathf.Clamp(rb.velocity.z, -1f, 1f));
					if (scrapeParticle == null)
					{
						scrapeParticle = Object.Instantiate(scrapePrefab, hitInfo.point, Quaternion.identity);
					}
					scrapeParticle.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + 1f, hitInfo.point.z);
					scrapeParticle.transform.forward = hitInfo.normal;
				}
			}
			else if (scrapeParticle != null)
			{
				Object.Destroy(scrapeParticle);
				scrapeParticle = null;
			}
			if (Input.GetButtonDown("Jump") && !jumpCooldown && currentWallJumps < 3)
			{
				WallJump();
			}
		}
		else if (scrapeParticle != null)
		{
			Object.Destroy(scrapeParticle);
			scrapeParticle = null;
		}
		if (Input.GetButtonDown("Fire3") && boostCharge >= 100f)
		{
			boostLeft = 100f;
			boost = true;
			dodgeDirection = movementDirection;
			if (dodgeDirection == Vector3.zero)
			{
				dodgeDirection = base.transform.forward;
			}
			Quaternion identity = Quaternion.identity;
			identity.SetLookRotation(dodgeDirection * -1f);
			Object.Instantiate(dodgeParticle, base.transform.position + dodgeDirection * 10f, identity);
			boostCharge -= 100f;
			if (dodgeDirection == base.transform.forward)
			{
				cc.dodgeDirection = 0;
			}
			else if (dodgeDirection == base.transform.forward * -1f)
			{
				cc.dodgeDirection = 1;
			}
			else
			{
				cc.dodgeDirection = 2;
			}
			aud.clip = dodgeSound;
			aud.volume = 1f;
			aud.pitch = 1f;
			aud.Play();
		}
		if (!walking && (axisRaw2 != 0f || axisRaw != 0f))
		{
			walking = true;
			anim.SetBool("WalkF", true);
		}
		else if (walking && axisRaw2 == 0f && axisRaw == 0f)
		{
			walking = false;
			anim.SetBool("WalkF", false);
		}
		if (hurting && hp > 0)
		{
			currentColor.a -= Time.deltaTime;
			hurtScreen.color = currentColor;
			if (currentColor.a <= 0f)
			{
				hurting = false;
			}
		}
		if (greenHpColor.a > 0f)
		{
			greenHpColor.a -= Time.deltaTime;
			greenHpFlash.color = greenHpColor;
		}
		if (boostCharge != 300f)
		{
			if (boostCharge + 70f * Time.deltaTime < 300f)
			{
				boostCharge += 70f * Time.deltaTime;
			}
			else
			{
				boostCharge = 300f;
			}
			Slider[] array = staminaSliders;
			foreach (Slider slider in array)
			{
				slider.value = boostCharge;
			}
		}
		if (hpAfterImage.value != healthBar.value)
		{
			hpAfterImage.value -= Time.deltaTime * 60f;
			if (hpAfterImage.value < healthBar.value)
			{
				hpAfterImage.value = healthBar.value;
			}
		}
		Vector3 vector = hudOriginalPos - cc.transform.InverseTransformDirection(rb.velocity) / 1000f;
		float num = Vector3.Distance(vector, screenHud.transform.localPosition);
		screenHud.transform.localPosition = Vector3.MoveTowards(screenHud.transform.localPosition, vector, Time.deltaTime * 15f * num);
		Vector3 vector2 = Vector3.ClampMagnitude(camOriginalPos - cc.transform.InverseTransformDirection(rb.velocity) / 350f * -1f, 0.2f);
		float num2 = Vector3.Distance(vector2, hudCam.transform.localPosition);
		hudCam.transform.localPosition = Vector3.MoveTowards(hudCam.transform.localPosition, vector2, Time.deltaTime * 25f * num2);
	}

	private void FixedUpdate()
	{
		if (!boost)
		{
			Move();
		}
		else
		{
			Dodge();
		}
	}

	private void Move()
	{
		if (!hurting)
		{
			base.gameObject.layer = 2;
		}
		if (gc.onGround)
		{
			aud.pitch = 1f;
			currentWallJumps = 0;
			movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime);
			rb.velocity = movementDirection2;
			anim.SetBool("Run", false);
			return;
		}
		movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime);
		airDirection.y = 0f;
		if ((movementDirection2.x > 0f && rb.velocity.x < movementDirection2.x) || (movementDirection2.x < 0f && rb.velocity.x > movementDirection2.x))
		{
			airDirection.x = movementDirection2.x;
		}
		else
		{
			airDirection.x = 0f;
		}
		if ((movementDirection2.z > 0f && rb.velocity.z < movementDirection2.z) || (movementDirection2.z < 0f && rb.velocity.z > movementDirection2.z))
		{
			airDirection.z = movementDirection2.z;
		}
		else
		{
			airDirection.z = 0f;
		}
		rb.AddForce(airDirection.normalized * airAcceleration);
	}

	private void Dodge()
	{
		base.gameObject.layer = 15;
		movementDirection2 = new Vector3(dodgeDirection.x * walkSpeed * Time.deltaTime, 0f, dodgeDirection.z * walkSpeed * Time.deltaTime);
		rb.velocity = movementDirection2 * 3f;
		boostLeft -= 10f;
		if (boostLeft <= 0f)
		{
			boost = false;
			if (!gc.onGround)
			{
				rb.velocity = movementDirection2;
			}
		}
	}

	private void Jump()
	{
		currentSound = Random.Range(0, jumpSounds.Length);
		aud.clip = jumpSounds[currentSound];
		aud.volume = 0.75f;
		aud.pitch = 1f;
		aud.Play();
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.AddForce(Vector3.up * jumpPower * 1500f);
		jumpCooldown = true;
		Invoke("JumpReady", 0.1f);
		boost = false;
	}

	private void WallJump()
	{
		currentWallJumps++;
		currentSound = Random.Range(0, jumpSounds.Length);
		aud.clip = jumpSounds[currentSound];
		aud.pitch += 0.25f;
		aud.volume = 0.75f;
		aud.Play();
		if (currentWallJumps == 3)
		{
			aud2.clip = finalWallJump;
			aud2.volume = 0.75f;
			aud2.Play();
		}
		wallJumpPos = base.transform.position - wc.poc;
		rb.velocity = new Vector3(0f, 0f, 0f);
		rb.AddForceAtPosition(wallJumpPos.normalized * 150000f, base.transform.position);
		rb.AddForce(Vector3.up * 100f);
		jumpCooldown = true;
		Invoke("JumpReady", 0.1f);
	}

	private void JumpReady()
	{
		jumpCooldown = false;
	}

	public void GetHurt(int damage)
	{
		if (dead)
		{
			return;
		}
		base.gameObject.layer = 15;
		if (damage >= 50)
		{
			currentColor.a = 0.8f;
		}
		else
		{
			currentColor.a = 0.5f;
		}
		hurting = true;
		cc.CameraShake(damage / 10);
		hurtAud.pitch = Random.Range(0.8f, 1f);
		hurtAud.PlayOneShot(hurtAud.clip);
		if (hp - damage > 0)
		{
			hp -= damage;
		}
		else
		{
			hp = 0;
		}
		healthBar.value = hp;
		hpText.text = hp.ToString();
		if (shud == null)
		{
			shud = GetComponentInChildren<StyleHUD>();
		}
		shud.RemovePoints(500);
		if (hp == 0)
		{
			blackScreen.gameObject.SetActive(true);
			rb.constraints = RigidbodyConstraints.None;
			ccAud.Play();
			cc.enabled = false;
			revolver = GetComponentInChildren<Revolver>().gameObject;
			revolver.SetActive(false);
			if (GetComponentInChildren<SecondaryRevolver>() != null)
			{
				GetComponentInChildren<SecondaryRevolver>().gameObject.SetActive(false);
			}
			rb.constraints = RigidbodyConstraints.None;
			dead = true;
			screenHud.SetActive(false);
		}
	}

	public void GetHealth(int health, bool silent)
	{
		if (dead)
		{
			return;
		}
		if (hp + health < 100)
		{
			hp += health;
		}
		else
		{
			hp = 100;
		}
		greenHpColor.a = 1f;
		greenHpFlash.color = greenHpColor;
		healthBar.value = hp;
		hpText.text = hp.ToString();
		if (!silent)
		{
			if (greenHpAud == null)
			{
				greenHpAud = greenHpFlash.GetComponent<AudioSource>();
			}
			greenHpAud.Play();
			Object.Instantiate(scrnBlood, fullHud.transform);
		}
	}

	public void Respawn()
	{
		hp = 100;
		rb.constraints = defaultRBConstraints;
		blackScreen.gameObject.SetActive(false);
		cc.enabled = true;
		if (revolver == null)
		{
			revolver = GetComponentInChildren<Revolver>().gameObject;
		}
		revolver.SetActive(true);
		if (GetComponentInChildren<SecondaryRevolver>() != null)
		{
			GetComponentInChildren<SecondaryRevolver>().gameObject.SetActive(true);
		}
		screenHud.SetActive(true);
		dead = false;
		blackColor.a = 0f;
		youDiedColor.a = 0f;
		currentAllPitch = 1f;
		blackScreen.color = blackColor;
		youDiedText.color = youDiedColor;
		audmix.SetFloat("allPitch", currentAllPitch);
	}
}
