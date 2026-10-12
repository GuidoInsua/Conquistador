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
// Desc: Desplaza el fondo hacia abajo para dar sensación de avance.

using UnityEngine;
using UnityEngine.UI;

public class MovimientoFondo : MonoBehaviour
{
	[SerializeField] private RawImage background;
	[SerializeField] private float speed = 0.05f;

	private void Awake()
	{
		if (background == null)
			background = GetComponent<RawImage>();

		if (background == null)
		{
			Debug.LogError("MovimientoFondo: falta asignar el RawImage.");
			enabled = false;
		}
	}

	private void Update()
	{
		Rect uvRect = background.uvRect;

		uvRect.y += speed * Time.deltaTime;

		if (uvRect.y >= 1f)
		{
			uvRect.y -= 1f;
		}

		background.uvRect = uvRect;
	}
}