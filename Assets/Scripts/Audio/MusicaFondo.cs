// ##########################################################################
// ## o ########## o ##                                                     #
// ### \ ######## / ###                                                     #
// #### \ ###### / ####       CONQUISTADOR                                  #
// ##### :-'""'-: #####       ------------                                  #
// ## .-'  ____  `-. ##       Autor:   Guido Insua                          #
// # ( (  (_()_)  ) ) #       Mail:    insua.guido@gmail.com                #
// ## `-.   ^^   .-' ##       Materia: Programacion 1 2do Cuatri 2026       #
// ##### `._==_.' #####                                                     #
// ###### __)(___ #####                                                     #
// ##########################################################################
// Desc: Reproduce en bucle la música de fondo de la escena.

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
