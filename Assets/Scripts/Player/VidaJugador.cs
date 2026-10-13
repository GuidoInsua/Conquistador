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
// Desc: Maneja la vida del jugador, su barra de fichas y la pantalla de muerte.

using UnityEngine;
using UnityEngine.Serialization;

public class VidaJugador : MonoBehaviour
{
	[SerializeField] private int vidaMaxima = 5;
	[SerializeField] private BarraVida barraVida;
	[FormerlySerializedAs("cnavasMuerte")]
	[SerializeField] private GameObject canvasMuerte;

	private int vidaActual;

	public int VidaActual => vidaActual;

	void Awake()
	{
		if (barraVida == null)
		{
			Debug.LogError("VidaJugador: falta asignar barraVida.");
			enabled = false;
			return;
		}

		vidaActual = vidaMaxima;
		ActualizarBarra();
	}

	private void ActualizarBarra()
	{
		barraVida.Mostrar(vidaActual);
	}

	public void actualizarVida(int cantidad)
	{
		vidaActual += cantidad;
		vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
		ActualizarBarra();

		if (vidaActual <= 0)
		{
			if (canvasMuerte != null)
				canvasMuerte.SetActive(true);

			Destroy(gameObject);
		}
	}
}
