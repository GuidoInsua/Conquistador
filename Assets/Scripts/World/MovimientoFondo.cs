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