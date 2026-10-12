// ##########################################################################
// ## o ########## o ##                                                     #
// ### \ ######## / ###                                                     #
// #### \ ###### / ####       CONQUISTADOR                                  #
// ##### :-'""'-: #####       ------------                                  #
// ## .-'  ____  `-. ##       Autor:        Guido Insua                     #
// # ( (  (_()_)  ) ) #       Materia:      Programación 1                  #
// ## `-.   ^^   .-' ##       Cuatrimestre: Segundo cuatrimestre 2026       #
// ##### `._==_.' #####                                                     #
// ###### __)(___ #####                                                     #
// ##########################################################################

using UnityEngine;

public class MusicaFondo : MonoBehaviour
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
