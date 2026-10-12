using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [Header("Enemigo")]
    [SerializeField] private EnemigoAlien enemyPrefab;

    [Header("Puntos de Spawn - Fuera de pantalla")]
    [SerializeField] private Transform[] puntosSpawn;

    [Header("Posiciones finales - Dentro de pantalla")]
    [SerializeField] private Transform[] posicionesEnemigos;

    [Header("Configuración")]
    [SerializeField] private int capacidadMaxima = 10;

    [SerializeField] private float velocidadMovimiento = 3f;

    [SerializeField] private bool spawnAutomatico = true;

    [SerializeField] private float tiempoEntreSpawns = 2f;

    private IObjectPool<EnemigoAlien> enemyPool;
    private bool[] posicionesOcupadas;
    private int enemigosActivos = 0;
    private float tiempoProximoSpawn;

    private void Awake()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(
                "Spawner: falta asignar Enemy Prefab."
            );

            return;
        }

        if (
            puntosSpawn == null ||
            puntosSpawn.Length == 0
        )
        {
            Debug.LogError(
                "Spawner: no hay puntos de spawn."
            );

            return;
        }

        if (
            posicionesEnemigos == null ||
            posicionesEnemigos.Length == 0
        )
        {
            Debug.LogError(
                "Spawner: no hay posiciones finales."
            );

            return;
        }

        if (
            puntosSpawn.Length !=
            posicionesEnemigos.Length
        )
        {
            Debug.LogError(
                "Spawner: puntosSpawn y " +
                "posicionesEnemigos deben tener " +
                "la misma cantidad de elementos."
            );

            return;
        }

        posicionesOcupadas =
            new bool[posicionesEnemigos.Length];

        enemyPool =
            new ObjectPool<EnemigoAlien>(
                CreateEnemy,
                OnGetEnemy,
                OnReleaseEnemy,
                OnDestroyEnemy,

                collectionCheck: true,

                defaultCapacity: capacidadMaxima,

                maxSize: capacidadMaxima
            );
    }


    private void Start()
    {
        tiempoProximoSpawn =
            Time.time +
            tiempoEntreSpawns;
    }


    private void Update()
    {
        if (!spawnAutomatico)
            return;


        if (Time.time >= tiempoProximoSpawn)
        {
            tiempoProximoSpawn =
                Time.time +
                tiempoEntreSpawns;


            SpawnEnemy();
        }
    }


    // =========================================================
    // CREAR ENEMIGO
    // =========================================================

    private EnemigoAlien CreateEnemy()
    {
        EnemigoAlien enemy =
            Instantiate(enemyPrefab);


        enemy.SetPool(enemyPool);


        enemy.gameObject.SetActive(false);


        return enemy;
    }


    // =========================================================
    // GET DEL POOL
    // =========================================================

    private void OnGetEnemy(
        EnemigoAlien enemy)
    {
        enemy.gameObject.SetActive(true);

        enemy.ResetEnemy();
    }


    // =========================================================
    // RELEASE DEL POOL
    // =========================================================

    private void OnReleaseEnemy(
        EnemigoAlien enemy)
    {
        int index =
            enemy.GetPositionIndex();

        if (
            index >= 0 &&
            index < posicionesOcupadas.Length
        )
        {
            posicionesOcupadas[index] = false;
        }

        enemigosActivos--;


        if (enemigosActivos < 0)
        {
            enemigosActivos = 0;


            Debug.LogError(
                "Spawner: enemigosActivos < 0. " +
                "Posible doble Release."
            );
        }

        enemy.gameObject.SetActive(false);
    }



    private void OnDestroyEnemy(
        EnemigoAlien enemy)
    {
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }


    // =========================================================
    // SPAWN
    // =========================================================

    public bool SpawnEnemy()
    {

        if (
            enemigosActivos >=
            capacidadMaxima
        )
        {
            return false;
        }

        int positionIndex =
            BuscarPuntoLibre();


        if (positionIndex == -1)
        {

            return false;
        }

        posicionesOcupadas[positionIndex] =
            true;


        EnemigoAlien enemy =
            enemyPool.Get();


        if (enemy == null)
        {
            // Rollback de seguridad.
            posicionesOcupadas[positionIndex] =
                false;


            Debug.LogError(
                "Spawner: el pool devolvió null."
            );


            return false;
        }


        Transform puntoSpawn =
            puntosSpawn[positionIndex];



        Transform posicionFinal =
            posicionesEnemigos[positionIndex];



        enemy.transform.position =
            puntoSpawn.position;


        enemy.SetPositionIndex(
            positionIndex
        );


        enemigosActivos++;

        _ = MoverEnemigoAsync(
            enemy,
            posicionFinal.position
        );


        return true;
    }


    private async Awaitable MoverEnemigoAsync(
        EnemigoAlien enemy,
        Vector3 destino)
    {
        if (enemy == null)
            return;


        await enemy.MoveToPositionAsync(
            destino,
            velocidadMovimiento
        );
    }


    private int BuscarPuntoLibre()
    {
        List<int> posicionesLibres =
            new List<int>();


        for (
            int i = 0;
            i < posicionesOcupadas.Length;
            i++
        )
        {
            if (!posicionesOcupadas[i])
            {
                posicionesLibres.Add(i);
            }
        }


        if (posicionesLibres.Count == 0)
            return -1;


        int randomIndex =
            Random.Range(
                0,
                posicionesLibres.Count
            );


        return posicionesLibres[randomIndex];
    }


    private void OnDrawGizmosSelected()
    {
        // -----------------------------------------------------
        // Puntos de entrada
        // -----------------------------------------------------

        if (puntosSpawn != null)
        {
            Gizmos.color = Color.red;


            foreach (
                Transform punto in puntosSpawn)
            {
                if (punto == null)
                    continue;


                Gizmos.DrawWireSphere(
                    punto.position,
                    0.3f
                );


                Gizmos.DrawLine(
                    punto.position,
                    punto.position +
                    Vector3.down * 0.5f
                );
            }
        }


        // -----------------------------------------------------
        // Posiciones finales
        // -----------------------------------------------------

        if (posicionesEnemigos != null)
        {
            Gizmos.color = Color.green;


            foreach (
                Transform posicion in posicionesEnemigos)
            {
                if (posicion == null)
                    continue;


                Gizmos.DrawWireSphere(
                    posicion.position,
                    0.3f
                );


                Gizmos.DrawLine(
                    posicion.position,
                    posicion.position +
                    Vector3.up * 0.5f
                );
            }
        }
    }
}