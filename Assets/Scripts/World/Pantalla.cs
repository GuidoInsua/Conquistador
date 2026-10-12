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
	// Con un margen de 50 píxeles para que no desaparezcan justo en el borde.
	public static bool EstaFuera(Vector3 posicionMundo)
	{
		Camera mainCamera = Camera.main;
		if (mainCamera == null)
			return false;

		Vector3 screenPos = mainCamera.WorldToScreenPoint(posicionMundo);

		return screenPos.x < -50 || screenPos.x > Screen.width + 50 ||
			   screenPos.y < -50 || screenPos.y > Screen.height + 50;
	}
}
