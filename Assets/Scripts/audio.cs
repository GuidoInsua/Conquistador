using NUnit.Framework.Constraints;
using UnityEngine;

public class audio : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] AudioSource musicSource;

	public AudioClip BackgroundMusic;


	private void Start()
	{
		if (musicSource != null && BackgroundMusic != null)
		{
			musicSource.clip = BackgroundMusic;
			musicSource.loop = true;
			musicSource.Play();
		}
		else
		{
			Debug.LogWarning("AudioSource or BackgroundMusic is not assigned.");
		}
	}
}
