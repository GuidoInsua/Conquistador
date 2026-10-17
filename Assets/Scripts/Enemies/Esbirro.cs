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
// Desc: Esbirro del boss: sale por un costado, baja oscilando hacia el jugador, lo golpea al tocarlo y rebota; entre esbirros se empujan y al rato se desarman solos.

using UnityEngine;

public class Esbirro : MonoBehaviour
{
    [Header("Salida")]
    [Tooltip("Cuánto se corre hacia el costado del boss antes de ir por el jugador.")]
    [SerializeField] private float distanciaSalida = 6f;
    [SerializeField] private float velocidadSalida = 5f;

    [Header("Persecución")]
    [SerializeField] private float velocidadPersecucion = 1.2f;
    [SerializeField] private Vector2 amplitudOscilacion = new Vector2(0.6f, 0.4f);
    [SerializeField] private float frecuenciaOscilacion = 0.8f;

    [Tooltip("Segundos desde que sale hasta que se desarma solo, sin soltar power-up.")]
    [SerializeField] private float tiempoVida = 5f;

    [Header("Golpe")]
    [Tooltip("Velocidad con la que rebota al tocar al jugador. Se frena de a poco.")]
    [SerializeField] private float velocidadRetroceso = 7f;
    [SerializeField] private float duracionRetroceso = 0.6f;
    [Tooltip("Empujón al chocar con otro esbirro, para que no se encimen. Mucho más chico que el rebote contra el jugador.")]
    [SerializeField] private float velocidadChoque = 1.5f;
    [SerializeField] private float duracionChoque = 0.3f;

    // Igual que BalaEnemiga: cada golpe saca una ficha.
    private const int danoAlJugador = 1;
    // Distancia mínima al borde de la cámara al terminar la salida.
    private const float margenBorde = 1f;

    private EnemigoAlien alien;
    private Transform jugador;
    private Vector3 posicionBase; // Sin la oscilación.
    private Vector3 destinoSalida;
    private bool isSaliendo;
    private Vector2 velocidadEmpuje;
    private float finRetroceso;
    private float duracionEmpuje;
    private float tiempoOscilacion;
    private float semillaOscilacion;
    private float finVida;

    public bool EstaVivo
    {
        get { return alien.GetSalud() > 0f; }
    }

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

        // Si el boss está cerca de un borde, la salida no lo cruza.
        Camera camara = Camera.main;
        if (camara != null)
        {
            float mitadAncho = camara.orthographicSize * camara.aspect - margenBorde;
            destinoSalida.x = Mathf.Clamp(destinoSalida.x, camara.transform.position.x - mitadAncho, camara.transform.position.x + mitadAncho);
        }
        isSaliendo = true;
        finRetroceso = 0f;
        tiempoOscilacion = 0f;
        semillaOscilacion = Random.value * 100f;
        finVida = Time.time + tiempoVida;
    }

    private void Update()
    {
        // Muriendo: se queda donde está mientras corre la animación.
        if (alien.GetSalud() <= 0f)
        {
            return;
        }

        if (Time.time >= finVida)
        {
            alien.Expirar();
            return;
        }

        if (isSaliendo)
        {
            posicionBase = Vector3.MoveTowards(posicionBase, destinoSalida, velocidadSalida * Time.deltaTime);
            isSaliendo = posicionBase != destinoSalida;
            transform.position = posicionBase;
            return;
        }

        if (Time.time < finRetroceso)
        {
            // El empujón se frena de a poco: al principio va a toda velocidad y al final casi quieto.
            float fuerzaRestante = (finRetroceso - Time.time) / duracionEmpuje;
            Vector2 movimiento = velocidadEmpuje * fuerzaRestante * Time.deltaTime;
            posicionBase += new Vector3(movimiento.x, movimiento.y, 0f);
        }
        else if (jugador != null)
        {
            posicionBase = Vector3.MoveTowards(posicionBase, jugador.position, velocidadPersecucion * Time.deltaTime);
        }

        tiempoOscilacion += Time.deltaTime;
        Vector2 desvio = Oscilacion.Desvio(semillaOscilacion, tiempoOscilacion * frecuenciaOscilacion) * amplitudOscilacion;
        transform.position = posicionBase + new Vector3(desvio.x, desvio.y, 0f);
    }

    // Se aleja del jugador; si están encimados, para arriba.
    public static Vector2 DireccionRetroceso(Vector2 esbirro, Vector2 jugador)
    {
        Vector2 alejarse = esbirro - jugador;
        // Casi en el mismo punto no hay una dirección clara para alejarse.
        if (alejarse.sqrMagnitude < 0.0001f)
        {
            return Vector2.up;
        }

        // normalized deja la dirección con largo 1, para que la velocidad la ponga quien la usa.
        return alejarse.normalized;
    }

    // Los choques entre esbirros llegan porque el Rigidbody2D cinemático tiene "Use Full Kinematic Contacts".
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alien.GetSalud() <= 0f || Time.time < finRetroceso)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            // Aunque el jugador esté invulnerable rebota igual, así no se le queda encima.
            // TryGetComponent busca el componente: si lo encuentra devuelve true y lo deja en la variable de "out".
            if (collision.TryGetComponent(out VidaJugador vida))
            {
                vida.actualizarVida(-danoAlJugador);
            }

            Rebotar(collision.transform.position, velocidadRetroceso, duracionRetroceso);
            isSaliendo = false;
        }
        // Mientras salen del boss no se empujan, así no se cruzan de lado.
        else if (!isSaliendo && collision.TryGetComponent(out Esbirro otro) && !otro.isSaliendo && otro.EstaVivo)
        {
            Rebotar(otro.transform.position, velocidadChoque, duracionChoque);
        }
    }

    private void Rebotar(Vector3 desde, float velocidad, float duracion)
    {
        velocidadEmpuje = DireccionRetroceso(transform.position, desde) * velocidad;
        duracionEmpuje = duracion;
        finRetroceso = Time.time + duracion;
    }

    // Si al terminar el rebote sigue tocando al jugador u otro esbirro, vuelve a reaccionar.
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
}
