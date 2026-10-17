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
// Desc: Botones que cambian de escena (jugar, reiniciar, volver al menú y salir); devuelve el tiempo a la normalidad si venía de la pausa.

using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
	// Desde la pausa el tiempo está congelado: la escena nueva arranca con el tiempo normal.
	public void CargarEscena(string nombre)
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(nombre);
	}

	public void Reiniciar()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Salir()
	{
		Application.Quit();
	}
}
