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
// Desc: Muestra la vida del jugador como una fila de fichas animadas.

using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
	[SerializeField] Image[] fichas;

	void Awake()
	{
		if (fichas == null || fichas.Length == 0)
		{
			Debug.LogError("BarraVida: faltan las fichas.");
			enabled = false;
		}
	}

	// Muestra una ficha por punto de vida. Las ocultas siguen animándose para no perder la sincronía.
	public void Mostrar(int vida)
	{
		if (fichas == null)
		{
			return;
		}

		for (int i = 0; i < fichas.Length; i++)
		{
			if (fichas[i] != null)
			{
				fichas[i].enabled = i < vida;
			}
		}
	}
}
