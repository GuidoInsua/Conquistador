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
// Desc: Ayuda para saber si una posición quedó fuera de la pantalla.

using UnityEngine;

public static class Pantalla
{
	// Margen en píxeles para que no desaparezcan justo en el borde.
	private const float margenPixeles = 50f;

	public static bool EstaFuera(Vector3 posicionMundo)
	{
		Camera mainCamera = Camera.main;
		if (mainCamera == null)
			return false;

		Vector3 screenPos = mainCamera.WorldToScreenPoint(posicionMundo);

		return screenPos.x < -margenPixeles || screenPos.x > Screen.width + margenPixeles ||
			   screenPos.y < -margenPixeles || screenPos.y > Screen.height + margenPixeles;
	}
}
