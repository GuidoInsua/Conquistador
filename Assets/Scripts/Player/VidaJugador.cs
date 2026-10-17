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
// Desc: Maneja la vida del jugador, su barra de fichas, el titileo de invulnerable tras un golpe y la pantalla de muerte.

using UnityEngine;
using UnityEngine.Serialization;

public class VidaJugador : MonoBehaviour
{
	[SerializeField] private int vidaMaxima = 5;
	[SerializeField] private BarraVida barraVida;
	[FormerlySerializedAs("cnavasMuerte")]
	[SerializeField] private GameObject canvasMuerte;

	[Header("Golpe")]
	[Tooltip("Tiempo sin recibir daño después de un golpe.")]
	[SerializeField] private float duracionInvulnerable = 1f;
	[Tooltip("Cada cuánto alterna entre normal y blanco mientras es invulnerable.")]
	[SerializeField] private float intervaloTitileo = 0.08f;
	[Tooltip("Pinta el sprite entero de blanco (shader GUI/Text Shader).")]
	[SerializeField] private Material materialDestello;
	[SerializeField] private AudioClip sonidoGolpe;

	private int vidaActual;
	private float finInvulnerable;
	private SpriteRenderer spriteRenderer;
	private Material materialNormal;

	public int VidaActual
	{
		get { return vidaActual; }
	}

	public bool EstaInvulnerable
	{
		get { return Time.time < finInvulnerable; }
	}

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		if (barraVida == null || spriteRenderer == null || materialDestello == null)
		{
			Debug.LogError("VidaJugador: falta asignar barraVida, materialDestello o el SpriteRenderer.");
			enabled = false;
			return;
		}

		materialNormal = spriteRenderer.sharedMaterial;
		vidaActual = vidaMaxima;
		ActualizarBarra();
	}

	private void Update()
	{
		// Arranca en blanco y alterna cada intervalo hasta que se termina la invulnerabilidad.
		bool isBlanco = false;
		if (EstaInvulnerable)
		{
			// Cuántos intervalos de titileo faltan para terminar: con número par va blanco, con impar normal.
			float tiempoRestante = finInvulnerable - Time.time;
			int intervalosRestantes = (int)(tiempoRestante / intervaloTitileo);
			isBlanco = intervalosRestantes % 2 == 0;
		}

		Material material = materialNormal;
		if (isBlanco)
		{
			material = materialDestello;
		}
		if (spriteRenderer.sharedMaterial != material)
		{
			spriteRenderer.sharedMaterial = material;
		}
	}

	private void ActualizarBarra()
	{
		barraVida.Mostrar(vidaActual);
	}

	public void actualizarVida(int cantidad)
	{
		if (cantidad < 0)
		{
			if (EstaInvulnerable)
			{
				return;
			}

			finInvulnerable = Time.time + duracionInvulnerable;
			EfectosSonido.Reproducir(sonidoGolpe);
		}

		vidaActual += cantidad;
		vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
		ActualizarBarra();

		if (vidaActual <= 0)
		{
			if (canvasMuerte != null)
			{
				canvasMuerte.SetActive(true);
			}

			Destroy(gameObject);
		}
	}
}
