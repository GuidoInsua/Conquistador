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
using UnityEngine.Serialization;

public class MusicaFondo : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] AudioSource musicSource;

	[FormerlySerializedAs("BackgroundMusic")]
	[SerializeField] AudioClip musicaFondo;


	private void Start()
	{
		if (musicSource != null && musicaFondo != null)
		{
			musicSource.clip = musicaFondo;
			musicSource.loop = true;
			musicSource.Play();
		}
		else
		{
			Debug.LogWarning("MusicaFondo: falta asignar musicSource o musicaFondo.");
		}
	}
}
