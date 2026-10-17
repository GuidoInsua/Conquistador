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
// Desc: Rayo continuo que el boss lanza desde la boca: primero avisa y después saca fichas mientras el jugador esté dentro.

using System.Collections;
using UnityEngine;

public class RayoBoss : MonoBehaviour
{
    [Tooltip("Tiempo que parpadea fino antes de hacer daño, para que el jugador pueda esquivarlo.")]
    [SerializeField] private float duracionAviso = 0.8f;
    [SerializeField] private float duracionRayo = 2f;
    [Tooltip("Cada cuánto saca una ficha mientras el jugador sigue dentro del rayo.")]
    [SerializeField] private float intervaloDano = 0.5f;

    // Igual que BalaEnemiga: cada golpe saca una ficha.
    private const int danoAlJugador = 1;
    private const string animAviso = "RayoAviso";
    private const string animActivo = "RayoActivo";

    private Animator animator;
    private Collider2D zonaDano;
    // Opcional. Es propio y no de EfectosSonido para que se corte cuando el rayo se apaga.
    private AudioSource sonido;
    private VidaJugador jugadorDentro;
    private float proximoDano;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        zonaDano = GetComponent<Collider2D>();
        sonido = GetComponent<AudioSource>();

        if (animator == null || zonaDano == null)
        {
            Debug.LogError("RayoBoss: falta el Animator o el Collider2D.");
            enabled = false;
            return;
        }

        zonaDano.enabled = false;
    }

    // La corre el boss: el rayo está apagado (inactivo) entre disparos.
    public IEnumerator Disparar()
    {
        gameObject.SetActive(true);
        if (!enabled)
            yield break;

        animator.Play(animAviso);
        yield return new WaitForSeconds(duracionAviso);

        animator.Play(animActivo);
        if (sonido != null)
            sonido.Play();
        zonaDano.enabled = true;
        yield return new WaitForSeconds(duracionRayo);

        zonaDano.enabled = false;
        jugadorDentro = null;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (jugadorDentro == null || Time.time < proximoDano)
            return;

        jugadorDentro.actualizarVida(-danoAlJugador);
        proximoDano = Time.time + intervaloDano;
    }

    // Enter/Exit en vez de Stay: Stay deja de llamarse si el Rigidbody2D del jugador se duerme quieto.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorDentro = collision.GetComponent<VidaJugador>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorDentro = null;
    }
}
