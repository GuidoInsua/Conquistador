using UnityEngine;

public class BordesMundo : MonoBehaviour
{
	[SerializeField] Camera mainCamera;

	private Vector3 screenBounds;
	private float containerWidth;
	private float containerHeight;

	void Start()
	{
		if (mainCamera == null)
			mainCamera = Camera.main;

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

		Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Static;
		rb.gravityScale = 0;
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
