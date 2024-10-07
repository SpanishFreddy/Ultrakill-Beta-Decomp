using UnityEngine;

public class Door : MonoBehaviour
{
	private Vector3 closedPos;

	public Vector3 openPos;

	private Vector3 openPosRelative;

	public bool startOpen;

	private Vector3 targetPos;

	public float speed;

	private bool inPos = true;

	private AudioSource aud;

	public AudioClip openSound;

	public AudioClip closeSound;

	private AudioSource aud2;

	public bool locked;

	public GameObject noPass;

	private void Start()
	{
		closedPos = base.transform.position;
		openPosRelative = base.transform.position + openPos;
		aud = GetComponent<AudioSource>();
		aud2 = base.transform.GetChild(0).GetComponent<AudioSource>();
		if (startOpen)
		{
			base.transform.position = openPosRelative;
		}
	}

	private void Update()
	{
		if (!inPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, targetPos, Time.deltaTime * speed);
			if (base.transform.position == targetPos)
			{
				inPos = true;
				aud.clip = closeSound;
				aud.Play();
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			Open();
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			Close();
		}
	}

	public void Open()
	{
		aud.clip = openSound;
		aud.Play();
		targetPos = openPosRelative;
		inPos = false;
	}

	public void Close()
	{
		aud.clip = openSound;
		aud.Play();
		targetPos = closedPos;
		inPos = false;
	}

	public void Lock()
	{
		locked = true;
		noPass.SetActive(true);
		if (base.transform.position != closedPos)
		{
			Close();
		}
		aud2.pitch = 0.2f;
		aud2.Play();
	}

	public void Unlock()
	{
		locked = false;
		noPass.SetActive(false);
		aud2.pitch = 0.5f;
		aud2.Play();
	}
}
