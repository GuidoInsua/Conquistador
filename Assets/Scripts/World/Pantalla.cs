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
