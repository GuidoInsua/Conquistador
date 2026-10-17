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
// Desc: El boss cangrejo: entra desde arriba, sigue al jugador despacio, ataca, y al destruirse el cuerpo se hincha y explota.

using System.Collections;
using UnityEngine;

public class JefeCangrejo : MonoBehaviour
{
    [Tooltip("Si se destruye, muere el boss. Las pinzas se destruyen solas sin matarlo.")]
    [SerializeField] private ParteBoss cuerpo;

    [Header("Entrada")]
    [Tooltip("Dónde se queda el boss después de bajar.")]
    [SerializeField] private Vector3 posicionPelea = new Vector3(0f, 3.5f, 0f);
    [SerializeField] private float velocidadEntrada = 1.5f;

    [Header("Movimiento")]
    [Tooltip("Retraso al seguir al jugador en x. Más alto = más lento.")]
    [SerializeField] private float tiempoSeguimiento = 1.5f;
    [SerializeField] private float velocidadMaxima = 2.5f;
    [Tooltip("Hasta dónde se aleja del centro en x, para no salirse de la pantalla.")]
    [SerializeField] private float limiteX = 8f;
    [Tooltip("Cuánto se desvía al azar de lado (x) y hacia arriba y abajo (y).")]
    [SerializeField] private Vector2 amplitudDesvio = new Vector2(2.5f, 1.2f);
    [SerializeField] private float frecuenciaDesvio = 0.35f;

    [Header("Ataques")]
    [Tooltip("Si queda vacío, el boss no lanza el rayo.")]
    [SerializeField] private RayoBoss rayo;
    [SerializeField] private float tiempoEntreRayos = 4f;
    [Tooltip("Disparan por turnos; una pinza destruida pierde su turno.")]
    [SerializeField] private DisparoPinza[] pinzas = new DisparoPinza[0];
    [SerializeField] private float tiempoEntreDisparosPinzas = 2f;
    [Tooltip("Si queda vacío, no saca esbirros.")]
    [SerializeField] private InvocadorEsbirros invocador;

    [Header("Muerte")]
    [Tooltip("Lo que dura la animación Muerte del cuerpo (se hincha) antes de explotar.")]
    [SerializeField] private float duracionHinchado = 0.8f;
    [SerializeField] private GameObject explosionPrefab;
    [Tooltip("Tiempo entre la explosión y la pantalla de victoria.")]
    [SerializeField] private float esperaVictoria = 2.5f;

    public Transform Jugador
    {
        get { return jugador; }
    }

    private System.Action alMorir;
    // Lo pasa FaseBoss al crearlo. Si es null o el jugador muere, el boss se queda en el centro.
    private Transform jugador;
    private bool isEnPosicion;
    private bool isAtacando;
    private float xSeguida;
    private float velocidadX;
    private float tiempoDesvio;
    private float semillaDesvio;

    private void Awake()
    {
        if (cuerpo == null)
        {
            Debug.LogError("JefeCangrejo: falta asignar el cuerpo.");
            enabled = false;
            return;
        }

        cuerpo.Destruida += Morir;
    }

    // La llama FaseBoss justo después de crearlo desde el prefab, arriba de la pantalla.
    public void Aparecer(System.Action alMorir, Transform jugador)
    {
        this.alMorir = alMorir;
        this.jugador = jugador;

        if (enabled)
        {
            StartCoroutine(Entrar());
        }
    }

    private IEnumerator Entrar()
    {
        while (transform.position != posicionPelea)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionPelea, velocidadEntrada * Time.deltaTime);
            yield return null;
        }

        xSeguida = posicionPelea.x;
        semillaDesvio = Random.value * 100f;
        isEnPosicion = true;

        if (rayo != null)
        {
            StartCoroutine(Atacar());
        }

        if (pinzas.Length > 0)
        {
            StartCoroutine(AtacarConPinzas());
        }

        if (invocador != null)
        {
            invocador.Comenzar();
        }
    }

    private IEnumerator AtacarConPinzas()
    {
        int turno = 0;
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreDisparosPinzas);

            // Durante el rayo no dispara: ya es bastante para esquivar.
            if (!isAtacando)
            {
                // Se turnan: el resto de dividir por la cantidad de pinzas da 0, 1, 0, 1...
                int indicePinza = turno % pinzas.Length;
                yield return pinzas[indicePinza].Disparar();
            }

            turno++;
        }
    }

    private IEnumerator Atacar()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreRayos);

            // Quieto mientras dispara: si se mueve, el rayo se desfasa de la boca.
            isAtacando = true;
            cuerpo.SetEnojada(true);
            yield return rayo.Disparar();
            cuerpo.SetEnojada(false);
            isAtacando = false;
        }
    }

    private void Update()
    {
        if (!isEnPosicion || isAtacando)
        {
            return;
        }

        // Va hacia la x del jugador, sin pasarse de los límites. Sin jugador, vuelve al centro.
        float objetivoX = posicionPelea.x;
        if (jugador != null)
        {
            objetivoX = Mathf.Clamp(jugador.position.x, -limiteX, limiteX);
        }
        xSeguida = Mathf.SmoothDamp(xSeguida, objetivoX, ref velocidadX, tiempoSeguimiento, velocidadMaxima);

        // El tiempo propio no avanza mientras ataca, así retoma el desvío donde lo dejó.
        tiempoDesvio += Time.deltaTime;
        Vector2 desvio = Oscilacion.Desvio(semillaDesvio, tiempoDesvio * frecuenciaDesvio) * amplitudDesvio;

        transform.position = new Vector3(xSeguida + desvio.x, posicionPelea.y + desvio.y, posicionPelea.z);
    }

    private void Morir()
    {
        StopAllCoroutines();
        isEnPosicion = false;
        StartCoroutine(Explotar());
    }

    private IEnumerator Explotar()
    {
        if (rayo != null)
        {
            rayo.gameObject.SetActive(false);
        }

        // Mientras se hincha ya no recibe golpes.
        foreach (Collider2D zona in GetComponentsInChildren<Collider2D>())
        {
            zona.enabled = false;
        }

        yield return new WaitForSeconds(duracionHinchado);

        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.enabled = false;
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, cuerpo.transform.position, Quaternion.identity);
        }

        // Sigue activo (sin verse) para que esta corrutina termine la espera.
        yield return new WaitForSeconds(esperaVictoria);

        gameObject.SetActive(false);
        if (alMorir != null)
        {
            alMorir();
        }
    }
}
