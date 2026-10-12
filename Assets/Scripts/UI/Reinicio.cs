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
// Desc: Botones de fin de partida: reiniciar, volver al menú y salir.

using UnityEngine;
using UnityEngine.SceneManagement;

public class Reinicio : MonoBehaviour
{
	public void ReiniciarEscena()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void salirDelJuego()
	{
		Application.Quit();
	}

	public void abrirMenu()
	{
		SceneManager.LoadScene("Menu");
	}
}
