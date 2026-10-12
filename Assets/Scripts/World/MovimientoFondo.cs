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