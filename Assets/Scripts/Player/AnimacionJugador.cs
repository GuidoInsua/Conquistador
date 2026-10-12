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
	private enum EstadoMovimiento
	{
		Idle,
		Izquierda,
		Derecha,
		Arriba,
		ArribaIzquierda,
		ArribaDerecha
	}

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

	private EstadoMovimiento estadoActual = EstadoMovimiento.Idle;
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
	public void ActualizarDireccion(Vector2 direccion)
	{
		EstadoMovimiento nuevo = CalcularEstado(direccion);

		if (nuevo == estadoActual)
			return;

		EstadoMovimiento anterior = estadoActual;
		estadoActual = nuevo;

		if (transicion != null)
		{
			StopCoroutine(transicion);
			transicion = null;
		}

		EstaInclinando = false;

		// Solo hay transición si alguno de los dos estados tiene inclinación lateral.
		if (ObtenerLado(anterior) == LadoHorizontal.Ninguno && ObtenerLado(nuevo) == LadoHorizontal.Ninguno)
		{
			MostrarNormal(nuevo);
			return;
		}

		transicion = StartCoroutine(ReproducirTransicion(anterior, nuevo));
	}

	private EstadoMovimiento CalcularEstado(Vector2 direccion)
	{
		bool izquierda = direccion.x < -0.5f;
		bool derecha = direccion.x > 0.5f;

		// Hacia abajo solo se ve la inclinación lateral: S solo es Idle, S+A es Izquierda, S+D es Derecha.
		if (direccion.y > 0.5f)
		{
			return izquierda ? EstadoMovimiento.ArribaIzquierda
				: derecha ? EstadoMovimiento.ArribaDerecha
				: EstadoMovimiento.Arriba;
		}

		return izquierda ? EstadoMovimiento.Izquierda
			: derecha ? EstadoMovimiento.Derecha
			: EstadoMovimiento.Idle;
	}

	private IEnumerator ReproducirTransicion(EstadoMovimiento desde, EstadoMovimiento hasta)
	{
		EstaInclinando = true;

		LadoHorizontal ladoDesde = ObtenerLado(desde);
		LadoHorizontal ladoHasta = ObtenerLado(hasta);

		if (ladoDesde != LadoHorizontal.Ninguno)
		{
			MostrarIntermedio(ladoDesde);
			yield return new WaitForSeconds(duracionTransicion);

			// Sale de un lado: pasa por Idle antes de seguir (Izquierda -> Idle, Izquierda -> Derecha).
			if (ladoHasta != ladoDesde)
			{
				CambiarAnimacion(AnimIdle);
				yield return new WaitForSeconds(duracionPausaIdle);
			}
		}

		if (ladoHasta != LadoHorizontal.Ninguno)
		{
			MostrarIntermedio(ladoHasta);
			yield return new WaitForSeconds(duracionTransicion);
		}

		MostrarNormal(hasta);

		EstaInclinando = false;
		transicion = null;
	}

	private LadoHorizontal ObtenerLado(EstadoMovimiento estado)
	{
		switch (estado)
		{
			case EstadoMovimiento.Izquierda:
			case EstadoMovimiento.ArribaIzquierda:
				return LadoHorizontal.Izquierda;

			case EstadoMovimiento.Derecha:
			case EstadoMovimiento.ArribaDerecha:
				return LadoHorizontal.Derecha;

			default:
				return LadoHorizontal.Ninguno;
		}
	}

	private void MostrarNormal(EstadoMovimiento estado)
	{
		switch (ObtenerLado(estado))
		{
			case LadoHorizontal.Izquierda:
				CambiarAnimacion(AnimIzquierda);
				break;

			case LadoHorizontal.Derecha:
				CambiarAnimacion(AnimDerecha);
				break;

			default:
				CambiarAnimacion(AnimIdle);
				break;
		}
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
