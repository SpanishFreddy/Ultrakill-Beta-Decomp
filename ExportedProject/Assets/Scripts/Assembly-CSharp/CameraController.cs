using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CameraController : MonoBehaviour
{
	public bool invert;

	public float minimumX = -89f;

	public float maximumX = 89f;

	public float minimumY = -360f;

	public float maximumY = 360f;

	public float sensitivityX;

	public float sensitivityY;

	public float scroll;

	public Vector3 defaultPos;

	private Vector3 targetPos;

	public GameObject player;

	private NewMovement pm;

	private Camera cam;

	public GameObject gun;

	private Revolver rev;

	private float rotationY;

	private float rotationX;

	public float cameraShaking;

	public float movementHor;

	public float movementVer;

	public int dodgeDirection;

	public float additionalRotationY;

	public float additionalRotationX;

	private float defaultFov;

	public AudioMixer audmix;

	private bool mouseUnlocked;

	private void Start()
	{
		player = GameObject.FindWithTag("Player");
		pm = player.GetComponent<NewMovement>();
		Cursor.lockState = CursorLockMode.Locked;
		Debug.Log(Cursor.lockState);
		cam = GetComponent<Camera>();
		gun = base.transform.GetChild(0).gameObject;
		rev = GetComponentInChildren<Revolver>();
		defaultPos = base.transform.localPosition;
		targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.2f, defaultPos.z);
		defaultFov = cam.fieldOfView;
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll > 0f)
		{
			sensitivityX += 0.1f;
			sensitivityY += 0.1f;
		}
		else if (scroll < 0f && sensitivityX > 0.1f)
		{
			sensitivityX -= 0.1f;
			sensitivityY -= 0.1f;
		}
		rotationX += Input.GetAxis("Mouse Y") * sensitivityX;
		rotationY += Input.GetAxis("Mouse X") * sensitivityY;
		rotationX = Mathf.Clamp(rotationX, minimumX + additionalRotationX, maximumX + additionalRotationX);
		if (pm.boost)
		{
			if (dodgeDirection == 0)
			{
				cam.fieldOfView = defaultFov - 5f;
			}
			else if (dodgeDirection == 1)
			{
				cam.fieldOfView = defaultFov + 10f;
			}
		}
		else if (cam.fieldOfView > defaultFov)
		{
			cam.fieldOfView -= Time.deltaTime * 100f;
		}
		else if (cam.fieldOfView < defaultFov)
		{
			cam.fieldOfView = defaultFov;
		}
		player.transform.localEulerAngles = new Vector3(0f, rotationY + additionalRotationY, 0f);
		if (!pm.boost)
		{
			base.transform.localEulerAngles = new Vector3(0f - rotationX + additionalRotationX, 0f, movementHor * -1f);
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(0f - rotationX + additionalRotationX, 0f, movementHor * -5f);
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
		if (Input.GetKey(KeyCode.O))
		{
			CameraShake(1f);
		}
		if ((rev == null && Input.GetKeyDown(KeyCode.Alpha1)) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
		{
			rev = GetComponentInChildren<Revolver>();
		}
		if (rev != null && Input.GetKeyDown(KeyCode.Alpha1))
		{
			rev.SwitchVariation(0);
		}
		else if (rev != null && Input.GetKeyDown(KeyCode.Alpha2))
		{
			rev.SwitchVariation(1);
		}
		else if (rev != null && Input.GetKeyDown(KeyCode.Alpha3))
		{
			rev.SwitchVariation(2);
		}
		if (cameraShaking > 0f)
		{
			if (cameraShaking > 1f)
			{
				base.transform.localPosition = new Vector3(defaultPos.x + (float)Random.Range(-1, 1), defaultPos.y + (float)Random.Range(-1, 1), defaultPos.z);
			}
			else
			{
				base.transform.localPosition = new Vector3(defaultPos.x + cameraShaking * (float)Random.Range(-1, 1), defaultPos.y + cameraShaking * (float)Random.Range(-1, 1), defaultPos.z);
			}
			cameraShaking -= Time.deltaTime * 5f;
		}
		else if (pm.walking)
		{
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, targetPos, Time.deltaTime * 0.5f);
			if (base.transform.localPosition == targetPos && targetPos != defaultPos)
			{
				targetPos = defaultPos;
			}
			else if (base.transform.localPosition == targetPos && targetPos == defaultPos)
			{
				targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
			}
		}
		else
		{
			base.transform.localPosition = defaultPos;
			targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
		}
	}

	public void CameraShake(float shakeAmount)
	{
		cameraShaking = shakeAmount;
	}

	public void StopShake()
	{
		cameraShaking = 0f;
	}

	public void HitStop(float length)
	{
		Time.timeScale = 0f;
		StartCoroutine(TimeIsStopped(length, false));
	}

	public void TrueStop(float length)
	{
		Time.timeScale = 0f;
		audmix.SetFloat("allPitch", 0.1f);
		StartCoroutine(TimeIsStopped(length, true));
	}

	private IEnumerator TimeIsStopped(float length, bool trueStop)
	{
		yield return new WaitForSecondsRealtime(length);
		ContinueTime(trueStop);
	}

	private void ContinueTime(bool trueStop)
	{
		Time.timeScale = 1f;
		if (trueStop)
		{
			audmix.SetFloat("allPitch", 1f);
		}
	}
}
