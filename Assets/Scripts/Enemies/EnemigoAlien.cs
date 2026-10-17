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

public class EnemigoAlien : MonoBehaviour, IDamageable
{
    [SerializeField] float vidaMaxima = 4f;
    [SerializeField] private OpcionPowerUp[] powerUps;
    [SerializeField] private GameObject particulasGolpePrefab;
    [SerializeField] private AudioClip sonidoGolpe;

    [SerializeField, Range(0f, 1f)]
    private float powerUpSpawnProbability = 0.2f;

    [Header("Oscilación")]
    [Tooltip("Cuánto se aleja de su posición en x e y mientras espera en ella.")]
    [SerializeField] private Vector2 amplitudOscilacion = new Vector2(0.4f, 0.25f);
    [SerializeField] private float frecuenciaOscilacion = 0.5f;

    //

    public const string animIdle = "Alien_Idle";
    public const string animDisparo = "Alien_Disparo";
    public const string animMuerte = "Alien_Muerte";

    private float vidaActual;
    private Animator animator;
    private string animacionActual;
    private IObjectPool<EnemigoAlien> enemyPool;
    private int movimientoId;
    private bool isEnPosicion;
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
        isEnPosicion = false;

        vidaActual = vidaMaxima;

        animator = GetComponent<Animator>();
        cambiarAnimacion(animIdle);
    }

    public async Awaitable MoveToPositionAsync(
    Vector3 target,
    float speedMove)
    {
        const float arrivalDistance = 0.1f;

        // Si el enemigo vuelve al pool y se reusa, ResetEnemy cambia el id y este bucle termina.
        int id = movimientoId;

        // this == null: el alien fue destruido (por ejemplo al recargar la escena).
        while (this != null && id == movimientoId && gameObject.activeInHierarchy
            && Vector3.Distance(transform.position, target) > arrivalDistance)
        {
            Vector3 direction = (target - transform.position).normalized;

            transform.position += direction * speedMove * Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }

        if (this == null || id != movimientoId || !gameObject.activeInHierarchy)
            return;

        transform.position = target;
        posicionBase = target;
        tiempoOscilacion = 0f;
        semillaOscilacion = Random.value * 100f;
        isEnPosicion = true;
    }

    private void Update()
    {
        if (!isEnPosicion)
            return;

        tiempoOscilacion += Time.deltaTime;
        transform.position = posicionBase + (Vector3)(Oscilacion.Desvio(semillaOscilacion, tiempoOscilacion * frecuenciaOscilacion) * amplitudOscilacion);
    }

    public void TakeDamage(float cantidad)
    {
        if (vidaActual > 0)
        {
            vidaActual = Mathf.Max(0f, vidaActual - cantidad);
            EfectosSonido.Reproducir(sonidoGolpe);
        }

        // Vivo es vida > 0, igual que en DisparoEnemigo: si no, el disparo pisa la animación de muerte.
        if (vidaActual > 0)
        {
            MostrarFeedbackGolpe();
        }
        else
        {
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

    // Este método se llama desde la animación de muerte
    private void Morir()
    {
        PowerUp.SpawnPowerUp(
            powerUps,
            transform.position,
            powerUpSpawnProbability
        );

        if (enemyPool != null)
        {
            enemyPool.Release(this);
        }
    }
}
