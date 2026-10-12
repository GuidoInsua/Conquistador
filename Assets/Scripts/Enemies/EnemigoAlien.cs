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
// Desc: Vida, movimiento, daño y muerte del alien; vuelve al pool al morir.

using UnityEngine;
using UnityEngine.Pool;

public class EnemigoAlien : MonoBehaviour, IDamageable
{
    [SerializeField] float vidaMaxima = 10f;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private GameObject particulasGolpePrefab;

    [SerializeField, Range(0f, 1f)]
    private float powerUpSpawnProbability = 0.2f;

    //

    private float vidaActual;
    private Animator animator;
    private string animacionActual;
    private IObjectPool<EnemigoAlien> enemyPool;
    private int movimientoId;
    private int positionIndex = -1;

    public float GetSalud()
    {
        return vidaActual;
    }

    public int GetPositionIndex()
    {
        return positionIndex;
    }

    public void SetPositionIndex(int index)
    {
        positionIndex = index;
    }

    public void SetPool(IObjectPool<EnemigoAlien> pool)
    {
        enemyPool = pool;
    }

    public void ResetEnemy()
    {
        // Invalida cualquier movimiento anterior que siga en marcha.
        movimientoId++;

        vidaActual = vidaMaxima;

        positionIndex = -1;

        animator = GetComponent<Animator>();
        cambiarAnimacion("Alien01_Idle");
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
    }

    public void TakeDamage(float cantidad)
    {
        if (vidaActual > 0)
        {
            vidaActual -= cantidad;
        }

        if (vidaActual >= 1)
        {
            MostrarFeedbackGolpe();
        }
        else
        {
            cambiarAnimacion("Alien01_Muerte");
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
        if (powerUpPrefab != null)
        {
            PowerUp.SpawnPowerUp(
                powerUpPrefab,
                transform.position,
                powerUpSpawnProbability
            );
        }

        if (enemyPool != null)
        {
            enemyPool.Release(this);
        }
    }

    private void VolverIdle()
    {
        cambiarAnimacion("Alien01_Idle");
    }

}
