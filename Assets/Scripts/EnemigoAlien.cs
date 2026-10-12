using System;
using System.Threading;
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
    private CancellationTokenSource movementCancellationSource;
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

    private void OnEnable()
    {
        CrearTokenMovimiento();
    }

    public void ResetEnemy()
    {
        CrearTokenMovimiento();

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


        if (movementCancellationSource == null)
        {
            CrearTokenMovimiento();
        }


        CancellationToken token =
            movementCancellationSource.Token;


        try
        {
            while (
                Vector3.Distance(
                    transform.position,
                    target
                ) > arrivalDistance)
            {
                if (token.IsCancellationRequested)
                    return;

                if (!gameObject.activeInHierarchy)
                    return;

                Vector3 direction =
                    (target - transform.position).normalized;


                transform.position +=
                    direction *
                    speedMove *
                    Time.deltaTime;


                await Awaitable.NextFrameAsync(token);
            }


            if (token.IsCancellationRequested)
                return;


            transform.position = target;
        }
        catch (OperationCanceledException)
        {
            // Es normal cuando el enemigo vuelve al pool.
        }
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

    private void CrearTokenMovimiento()
    {
        if (movementCancellationSource != null)
            return;

        movementCancellationSource =
            new CancellationTokenSource();
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

        LiberarAlPool();
    }

    public void LiberarAlPool()
    {

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
