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
// Desc: Crea paredes invisibles en los bordes de la pantalla.

using UnityEngine;

public class BordesMundo : MonoBehaviour
{
	[SerializeField] Camera mainCamera;

	private Vector3 screenBounds;
	private float containerWidth;
	private float containerHeight;
	private int anchoPantalla;
	private int altoPantalla;

	void Start()
	{
		if (mainCamera == null)
			mainCamera = Camera.main;

		if (mainCamera == null)
		{
			Debug.LogError("BordesMundo: no hay cámara.");
			enabled = false;
			return;
		}

		Recalcular();
	}

	// Si cambia el tamaño de la ventana, cambia lo que se ve y las paredes tienen que seguirlo.
	void Update()
	{
		if (Screen.width != anchoPantalla || Screen.height != altoPantalla)
			Recalcular();
	}

	// Rehace las paredes con el tamaño actual de la cámara (por ejemplo, después de alejarla).
	public void Recalcular()
	{
		anchoPantalla = Screen.width;
		altoPantalla = Screen.height;

		foreach (Transform pared in transform)
			Destroy(pared.gameObject);

		screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

		containerWidth = screenBounds.x;
		containerHeight = screenBounds.y;

		CreateBorders();
	}

	private void CreateBorders()
	{
		// Borde Izquierdo
		CreateBorderWall("LeftBorder", -containerWidth - 0.5f, 0, 1, containerHeight * 2);

		// Borde Derecho
		CreateBorderWall("RightBorder", containerWidth + 0.5f, 0, 1, containerHeight * 2);

		// Borde Inferior
		CreateBorderWall("BottomBorder", 0, -containerHeight - 0.5f, containerWidth * 2, 1);

		// Borde Superior
		CreateBorderWall("TopBorder", 0, containerHeight + 0.5f, containerWidth * 2, 1);
	}

	private void CreateBorderWall(string name, float posX, float posY, float width, float height)
	{
		GameObject wall = new GameObject(name);
		wall.transform.SetParent(transform);
		wall.transform.position = new Vector3(posX, posY, 0);

		BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
		collider.size = new Vector2(width, height);
		collider.isTrigger = false;
	}

#if UNITY_EDITOR

	void OnDrawGizmos()
	{
		if (mainCamera == null)
			mainCamera = Camera.main;

		if (mainCamera == null)
			return;

		// Calcular bordes
		Vector3 bounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
		float w = bounds.x;
		float h = bounds.y;

		// Dibujar rectángulo de límites
		Gizmos.color = Color.red;
		Vector3[] corners = new Vector3[4]
		{
			new Vector3(-w, -h, 0),
			new Vector3(w, -h, 0),
			new Vector3(w, h, 0),
			new Vector3(-w, h, 0)
		};

		Gizmos.DrawLine(corners[0], corners[1]);
		Gizmos.DrawLine(corners[1], corners[2]);
		Gizmos.DrawLine(corners[2], corners[3]);
		Gizmos.DrawLine(corners[3], corners[0]);
	}
#endif
}
