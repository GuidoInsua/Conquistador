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
// Desc: Vida, movimiento, oscilación en su lugar, daño y muerte del alien; vuelve al pool al morir.

using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class EnemigoAlien : MonoBehaviour, IDamageable
{
    [SerializeField] float vidaMaxima = 4f;
    [SerializeField] private OpcionPowerUp[] powerUps;
    [SerializeField] private GameObject particulasGolpePrefab;
    [Tooltip("Suena una vez, al morir.")]
    [FormerlySerializedAs("sonidoGolpe")]
    [SerializeField] private AudioClip sonidoMuerte;

    [SerializeField, Range(0f, 1f)]
    private float powerUpSpawnProbability = 0.2f;

    [Header("Oscilación")]
    [Tooltip("Cuánto se aleja de su posición en x e y mientras espera en ella.")]
    [SerializeField] private Vector2 amplitudOscilacion = new Vector2(1.6f, 0.8f);
    [SerializeField] private float frecuenciaOscilacion = 0.4f;
    [Tooltip("Cuánto se abre de costado la onda que hace mientras va a su lugar.")]
    [SerializeField] private float amplitudCamino = 0.6f;
    [Tooltip("Distancia que recorre en cada onda del camino. Más chico, más ondas.")]
    [SerializeField] private float largoOndaCamino = 4f;

    //

    public const string animIdle = "Alien_Idle";
    public const string animDisparo = "Alien_Disparo";
    public const string animMuerte = "Alien_Muerte";

    private float vidaActual;
    private Animator animator;
    private string animacionActual;
    private IObjectPool<EnemigoAlien> enemyPool;
    private int movimientoId;
    private bool isOscilando;
    private bool isSinPowerUp;
    private Vector3 posicionBase;
    private float tiempoOscilacion;
    private float semillaOscilacion;

    public float GetSalud()
    {
        return vidaActual;
    }

    public void SetPool(IObjectPool<EnemigoAlien> pool)
    {
        enemyPool = pool;
    }

    public void ResetEnemy()
    {
        // Invalida cualquier movimiento anterior que siga en marcha.
        movimientoId++;
        isOscilando = false;
        isSinPowerUp = false;

        vidaActual = vidaMaxima;

        animator = GetComponent<Animator>();
        cambiarAnimacion(animIdle);
    }

    public async Awaitable MoveToPositionAsync(
    Vector3 target,
    float speedMove)
    {
        // Si el enemigo vuelve al pool y se reusa, ResetEnemy cambia el id y este bucle termina.
        int id = movimientoId;

        Vector3 inicio = transform.position;
        float distanciaTotal = Vector3.Distance(inicio, target);
        Vector3 costado = Vector3.Cross(target - inicio, Vector3.forward).normalized;
        int ondas = Mathf.Max(1, Mathf.RoundToInt(distanciaTotal / largoOndaCamino));
        float recorrido = 0f;

        // this == null: el alien fue destruido (por ejemplo al recargar la escena).
        while (this != null && id == movimientoId && gameObject.activeInHierarchy && recorrido < distanciaTotal)
        {
            recorrido = Mathf.Min(distanciaTotal, recorrido + speedMove * Time.deltaTime);
            float avance = recorrido / distanciaTotal;
            transform.position = Vector3.Lerp(inicio, target, avance) + costado * OndaCamino(avance, ondas, amplitudCamino);

            await Awaitable.NextFrameAsync();
        }

        if (this == null || id != movimientoId || !gameObject.activeInHierarchy)
            return;

        // La onda termina en 0 justo al llegar, y la oscilación al azar también arranca en 0: no salta.
        transform.position = target;
        posicionBase = target;
        tiempoOscilacion = 0f;
        semillaOscilacion = Random.value * 100f;
        isOscilando = true;
    }

    // Desvío de costado según cuánto recorrió (0 a 1): ondas parejas que se apagan al salir y al llegar.
    public static float OndaCamino(float avance, int ondas, float amplitud) =>
        Mathf.Sin(avance * ondas * 2f * Mathf.PI) * Mathf.Sin(avance * Mathf.PI) * amplitud;

    private void Update()
    {
        if (!isOscilando)
            return;

        tiempoOscilacion += Time.deltaTime;
        transform.position = posicionBase + (Vector3)(Oscilacion.Desvio(semillaOscilacion, tiempoOscilacion * frecuenciaOscilacion) * amplitudOscilacion);
    }

    public void TakeDamage(float cantidad)
    {
        bool estabaVivo = vidaActual > 0;
        if (estabaVivo)
        {
            vidaActual = Mathf.Max(0f, vidaActual - cantidad);
        }

        // Vivo es vida > 0, igual que en DisparoEnemigo: si no, el disparo pisa la animación de muerte.
        if (vidaActual > 0)
        {
            MostrarFeedbackGolpe();
        }
        else
        {
            // Las balas que pegan mientras corre la animación de muerte no lo vuelven a sonar.
            if (estabaVivo)
                EfectosSonido.Reproducir(sonidoMuerte);

            cambiarAnimacion(animMuerte);
        }
    }

    private void MostrarFeedbackGolpe()
    {
        if (particulasGolpePrefab != null)
        {
            Instantiate(
                particulasGolpePrefab,
                transform.position,
                Quaternion.identity
            );
        }
    }

    public void cambiarAnimacion(string animacion, float crossFade = 0.2f)
    {
        if (animacionActual != animacion)
        {
            animacionActual = animacion;
            animator.CrossFade(animacion, crossFade);
        }
    }

    // Muere sin soltar power-up: un esbirro que se desarma solo no es un premio.
    public void Expirar()
    {
        if (vidaActual <= 0)
            return;

        vidaActual = 0;
        isSinPowerUp = true;
        EfectosSonido.Reproducir(sonidoMuerte);
        cambiarAnimacion(animMuerte);
    }

    // Este método se llama desde la animación de muerte
    private void Morir()
    {
        if (!isSinPowerUp)
            PowerUp.SpawnPowerUp(powerUps, transform.position, powerUpSpawnProbability);

        if (enemyPool != null)
        {
            enemyPool.Release(this);
        }
    }
}
