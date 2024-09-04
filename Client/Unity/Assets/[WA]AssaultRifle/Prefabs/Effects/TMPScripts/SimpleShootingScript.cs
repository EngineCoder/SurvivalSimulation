using UnityEngine;
using System.Collections;



public class SimpleShootingScript : MonoBehaviour 
{

	public int m_ShootSpeed = 15;
	private float mLightOffTime;

	public ParticleSystem[] m_VelocityShootParticles;
	public ParticleSystem[] m_OtherParticles;


	public Light m_ShootLight;
	private bool ShootOn;
	public GameObject ShootFX;
	public Transform m_ShootSound;

	void Start()
	{
		foreach (ParticleSystem i in m_VelocityShootParticles)
		{
			i.GetComponent<ParticleSystem>().emissionRate = m_ShootSpeed;	
		}

	}


	void Update() 
	{
		if (Input.GetButton("Fire1"))
		{
			//ShootFX.SetActive(true);
			ShootNow();

			foreach (ParticleSystem i in m_VelocityShootParticles)
			{
				i.GetComponent<ParticleSystem>().enableEmission = true;
			}

			foreach (ParticleSystem i in m_OtherParticles)
			{
				i.GetComponent<ParticleSystem>().enableEmission = true;
			}

		}
		else 
		{
			//ShootFX.SetActive(false);

			foreach (ParticleSystem i in m_VelocityShootParticles)
			{
				i.GetComponent<ParticleSystem>().enableEmission = false;
			}


			foreach (ParticleSystem i in m_OtherParticles)
			{
				i.GetComponent<ParticleSystem>().enableEmission = false;
			}
		}
	}


	public void ShootNow()
	{
		if (mLightOffTime < Time.time)
		{
			Instantiate(m_ShootSound, ShootFX.transform.position,ShootFX.transform.rotation);
			m_ShootLight.enabled = !m_ShootLight.enabled;
			mLightOffTime = Time.time + 1.0f/ (m_ShootSpeed * 2);
		}
	}
}
