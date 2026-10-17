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
// Desc: Bala enemiga: baja, con balanceo opcional, y le quita una ficha al jugador al tocarlo; también reparte balas en abanico.

using UnityEngine;

public class BalaEnemiga : MonoBehaviour
{
	[SerializeField] private float velocidad = 5f;
	// Todo golpe enemigo le saca una ficha al jugador. Es const para que ningún valor viejo del Inspector lo cambie.
	private const int danoAlJugador = 1;

	[Header("Balanceo (copo de nieve)")]
	[SerializeField] private float amplitudBalanceo = 0f; // Velocidad lateral máxima; 0 = cae derecho
	[SerializeField] private float frecuenciaBalanceo = 3f;

	private float faseBalanceo;

	// Reparte las balas en abanico, de -anguloCono/2 a +anguloCono/2 alrededor de la rotación dada.
	public static void DispararAbanico(GameObject prefab, Vector3 posicion, Quaternion rotacion, int cantidad, float anguloCono)
	{
		for (int i = 0; i < cantidad; i++)
		{
			float angulo = AnguloAbanico(i, cantidad, anguloCono);
			Quaternion rotacionBala = rotacion * Quaternion.Euler(0f, 0f, angulo);
			Instantiate(prefab, posicion, rotacionBala);
		}
	}

	// Ángulo de la bala número "indice" dentro del abanico.
	// Ejemplo: 5 balas en 70 grados salen a -35, -17.5, 0, 17.5 y 35.
	public static float AnguloAbanico(int indice, int cantidad, float anguloCono)
	{
		// Una sola bala sale derecha.
		if (cantidad <= 1)
		{
			return 0f;
		}

		// Qué parte del abanico le toca a esta bala: 0 la primera, 1 la última.
		float proporcion = indice / (float)(cantidad - 1);
		float anguloInicial = -anguloCono / 2f;
		float anguloFinal = anguloCono / 2f;

		// Lerp devuelve el valor que está en esa proporción entre el inicio y el final.
		return Mathf.Lerp(anguloInicial, anguloFinal, proporcion);
	}

	private void Start()
	{
		faseBalanceo = Random.value * Mathf.PI * 2f;
	}

	private void Update()
	{
		float lateral = Mathf.Cos(Time.time * frecuenciaBalanceo + faseBalanceo) * amplitudBalanceo;
		transform.Translate(new Vector2(lateral, -velocidad) * Time.deltaTime);

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
				vidaJugador.actualizarVida(-danoAlJugador);
			}

			Destroy(gameObject);
		}
	}
}
