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
// Desc: Elige la animación de la nave según la dirección del input, con las transiciones de inclinación.

using System.Collections;
using UnityEngine;

public class AnimacionJugador : MonoBehaviour
{
	private enum LadoHorizontal
	{
		Ninguno,
		Izquierda,
		Derecha
	}

	[SerializeField] Animator animator;

	[Header("Animación de transición")]
	[SerializeField] float duracionTransicion = 0.08f;

	[Header("Pausa en Idle al cambiar de lado")]
	[SerializeField] float duracionPausaIdle = 0.05f;

	private static readonly int AnimIdle = Animator.StringToHash("Jugador_Idle");
	private static readonly int AnimIzquierda = Animator.StringToHash("Jugador_Izquierda");
	private static readonly int AnimDerecha = Animator.StringToHash("Jugador_Derecha");
	private static readonly int AnimIzquierdaIntermedio = Animator.StringToHash("Jugador_IzquierdaIntermedio");
	private static readonly int AnimDerechaIntermedio = Animator.StringToHash("Jugador_DerechaIntermedio");

	private LadoHorizontal ladoActual = LadoHorizontal.Ninguno;
	private int animacionActual;
	private Coroutine transicion;

	// Mientras la nave cambia de inclinación, MovimientoJugador la frena un poco.
	public bool EstaInclinando { get; private set; }

	void Awake()
	{
		if (animator == null)
			animator = GetComponent<Animator>();

		if (animator == null)
		{
			Debug.LogError("AnimacionJugador: falta el Animator.");
			enabled = false;
		}
	}

	// Se llama en cada tick de física con el input de movimiento.
	// Solo cuenta el lado: subir o bajar no cambia el sprite, así que no reinicia la inclinación.
	public void ActualizarDireccion(Vector2 direccion)
	{
		LadoHorizontal nuevo = direccion.x < -0.5f ? LadoHorizontal.Izquierda
			: direccion.x > 0.5f ? LadoHorizontal.Derecha
			: LadoHorizontal.Ninguno;

		if (nuevo == ladoActual)
			return;

		LadoHorizontal anterior = ladoActual;
		ladoActual = nuevo;

		if (transicion != null)
			StopCoroutine(transicion);

		transicion = StartCoroutine(ReproducirTransicion(anterior, nuevo));
	}

	private IEnumerator ReproducirTransicion(LadoHorizontal desde, LadoHorizontal hasta)
	{
		EstaInclinando = true;

		if (desde != LadoHorizontal.Ninguno)
		{
			MostrarIntermedio(desde);
			yield return new WaitForSeconds(duracionTransicion);

			// Sale de un lado: pasa por Idle antes de seguir (Izquierda -> Idle, Izquierda -> Derecha).
			CambiarAnimacion(AnimIdle);
			yield return new WaitForSeconds(duracionPausaIdle);
		}

		if (hasta != LadoHorizontal.Ninguno)
		{
			MostrarIntermedio(hasta);
			yield return new WaitForSeconds(duracionTransicion);
		}

		CambiarAnimacion(hasta == LadoHorizontal.Izquierda ? AnimIzquierda
			: hasta == LadoHorizontal.Derecha ? AnimDerecha
			: AnimIdle);

		EstaInclinando = false;
		transicion = null;
	}

	private void MostrarIntermedio(LadoHorizontal lado)
	{
		CambiarAnimacion(lado == LadoHorizontal.Izquierda ? AnimIzquierdaIntermedio : AnimDerechaIntermedio);
	}

	private void CambiarAnimacion(int nuevoHash)
	{
		if (animacionActual == nuevoHash)
			return;

		animacionActual = nuevoHash;
		animator.Play(nuevoHash);
	}
}
