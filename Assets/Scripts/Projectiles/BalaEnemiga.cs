using UnityEngine;

public class BalaEnemiga : MonoBehaviour
{
	[SerializeField] private float velocidad = 5f;

	private void Update()
	{
		transform.Translate(Vector2.down * velocidad * Time.deltaTime);

		if (SalidoDePantalla())
		{
			Destroy(gameObject);
		}
	}

	private bool SalidoDePantalla()
	{
		Camera mainCamera = Camera.main;
		if (mainCamera == null)
			return false;

		Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

		return screenPos.x < -50 || screenPos.x > Screen.width + 50 ||
			   screenPos.y < -50 || screenPos.y > Screen.height + 50;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			VidaJugador vidaJugador = collision.GetComponent<VidaJugador>();
			if (vidaJugador != null)
			{
				vidaJugador.actualizarVida(-10);
			}

			Destroy(gameObject);
		}
	}
}
