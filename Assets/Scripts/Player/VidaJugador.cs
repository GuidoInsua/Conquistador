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
// Desc: Maneja la vida del jugador, su texto de HP y la pantalla de muerte.

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class VidaJugador : MonoBehaviour
{
	[SerializeField] private int vidaMaxima = 10;
	[SerializeField] private TMP_Text textoVida;
	[FormerlySerializedAs("cnavasMuerte")]
	[SerializeField] private GameObject canvasMuerte;

	private int vidaActual;

	void Awake()
	{
		vidaActual = vidaMaxima;
		actualizarTextoVida();
	}

	private void actualizarTextoVida()
	{
		textoVida.text = "HP " + vidaActual.ToString();
	}

	public void actualizarVida(int cantidad)
	{
		vidaActual += cantidad;
		vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
		actualizarTextoVida();

		if (vidaActual <= 0)
		{
			if (canvasMuerte != null)
				canvasMuerte.SetActive(true);

			Destroy(gameObject);
		}
	}
}
