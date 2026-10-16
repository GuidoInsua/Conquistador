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
// Desc: Esbirro del boss: sale por un costado, baja oscilando hacia el jugador, lo golpea al tocarlo y rebota.

using UnityEngine;

public class Esbirro : MonoBehaviour
{
    [Header("Salida")]
    [Tooltip("Cuánto se corre hacia el costado del boss antes de ir por el jugador.")]
    [SerializeField] private float distanciaSalida = 3f;
    [SerializeField] private float velocidadSalida = 4f;

    [Header("Persecución")]
    [SerializeField] private float velocidadPersecucion = 1.2f;
    [SerializeField] private Vector2 amplitudOscilacion = new Vector2(0.6f, 0.4f);
    [SerializeField] private float frecuenciaOscilacion = 0.8f;

    [Header("Golpe")]
    [Tooltip("Velocidad con la que rebota al tocar al jugador. Se frena de a poco.")]
    [SerializeField] private float velocidadRetroceso = 7f;
    [SerializeField] private float duracionRetroceso = 0.6f;

    // Igual que BalaEnemiga: cada golpe saca una ficha.
    private const int danoAlJugador = 1;

    private EnemigoAlien alien;
    private Transform jugador;
    private Vector3 posicionBase; // Sin la oscilación.
    private Vector3 destinoSalida;
    private bool isSaliendo;
    private Vector2 velocidadEmpuje;
    private float finRetroceso;
    private float tiempoOscilacion;
    private float semillaOscilacion;

    private void Awake()
    {
        alien = GetComponent<EnemigoAlien>();

        if (alien == null)
        {
            Debug.LogError("Esbirro: falta el componente EnemigoAlien.");
            enabled = false;
        }
    }

    // La llama el boss al sacarlo del pool. lado: -1 sale a la izquierda, 1 a la derecha.
    public void Iniciar(Vector3 origen, float lado, Transform jugador)
    {
        this.jugador = jugador;
        transform.position = posicionBase = origen;
        destinoSalida = origen + Vector3.right * lado * distanciaSalida;
        isSaliendo = true;
        finRetroceso = 0f;
        tiempoOscilacion = 0f;
        semillaOscilacion = Random.value * 100f;
    }

    private void Update()
    {
        // Muriendo: se queda donde está mientras corre la animación.
        if (alien.GetSalud() <= 0f)
            return;

        if (isSaliendo)
        {
            posicionBase = Vector3.MoveTowards(posicionBase, destinoSalida, velocidadSalida * Time.deltaTime);
            isSaliendo = posicionBase != destinoSalida;
            transform.position = posicionBase;
            return;
        }

        if (Time.time < finRetroceso)
            posicionBase += (Vector3)(velocidadEmpuje * ((finRetroceso - Time.time) / duracionRetroceso) * Time.deltaTime);
        else if (jugador != null)
            posicionBase = Vector3.MoveTowards(posicionBase, jugador.position, velocidadPersecucion * Time.deltaTime);

        tiempoOscilacion += Time.deltaTime;
        transform.position = posicionBase + (Vector3)(Oscilacion.Desvio(semillaOscilacion, tiempoOscilacion * frecuenciaOscilacion) * amplitudOscilacion);
    }

    // Se aleja del jugador; si están encimados, para arriba.
    public static Vector2 DireccionRetroceso(Vector2 esbirro, Vector2 jugador)
    {
        Vector2 alejarse = esbirro - jugador;
        return alejarse.sqrMagnitude < 0.0001f ? Vector2.up : alejarse.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || alien.GetSalud() <= 0f || Time.time < finRetroceso)
            return;

        // Aunque el jugador esté invulnerable rebota igual, así no se le queda encima.
        if (collision.TryGetComponent(out VidaJugador vida))
            vida.actualizarVida(-danoAlJugador);

        velocidadEmpuje = DireccionRetroceso(transform.position, collision.transform.position) * velocidadRetroceso;
        finRetroceso = Time.time + duracionRetroceso;
        isSaliendo = false;
    }

    // Si al terminar el rebote sigue tocando al jugador, vuelve a golpear.
    private void OnTriggerStay2D(Collider2D collision) => OnTriggerEnter2D(collision);
}
