using UnityEngine;

public class BalaEnemiga : MonoBehaviour
{
	[SerializeField] private float velocidad = 5f;

	private void Update()
	{
		transform.Translate(Vector2.down * velocidad * Time.deltaTime);

		if (Pantalla.EstaFuera(transform.position))
		{
			Destroy(gameObject);
		}
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
