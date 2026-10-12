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
