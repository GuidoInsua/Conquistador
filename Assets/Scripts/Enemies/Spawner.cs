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
// Desc: Crea aliens con un pool y los lleva a sus posiciones en pantalla.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [Header("Oleadas")]
    [SerializeField] private Oleada[] oleadas;

    [Header("Fin del juego")]
    [SerializeField] private GameObject canvasVictoria;
    [SerializeField] private GameObject canvasDerrota;

    [Header("Puntos de Spawn - Fuera de pantalla")]
    [SerializeField] private Transform[] puntosSpawn;

    [Header("Posiciones finales - Dentro de pantalla")]
    [SerializeField] private Transform[] posicionesEnemigos;

    [Header("Configuración")]
    [SerializeField] private int capacidadMaxima = 10;

    [SerializeField] private float velocidadMovimiento = 3f;

    [SerializeField] private bool spawnAutomatico = true;

    [SerializeField] private float tiempoEntreSpawns = 2f;

    private Dictionary<EnemigoAlien, IObjectPool<EnemigoAlien>> pools;
    private int oleadaActual = 0;
    private int spawneadosEnOleada = 0;
    private int totalPlaneado = 0;
    private int totalSpawneados = 0;
    private int enemigosEliminados = 0;
    private bool[] posicionesOcupadas;
    private int enemigosActivos = 0;
    private float tiempoProximoSpawn;

    public int TotalSpawneados => totalSpawneados;
    public int EnemigosEliminados => enemigosEliminados;

    private void Awake()
    {
        if (!ValidarOleadas())
        {
            enabled = false;
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

            enabled = false;
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

            enabled = false;
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

            enabled = false;
            return;
        }

        posicionesOcupadas =
            new bool[posicionesEnemigos.Length];

        // Un pool por tipo de enemigo: cada alien vuelve al pool de su propio prefab.
        pools = new Dictionary<EnemigoAlien, IObjectPool<EnemigoAlien>>();

        foreach (Oleada oleada in oleadas)
        {
            foreach (OpcionEnemigo opcion in oleada.Enemigos)
            {
                if (opcion.Prefab != null && !pools.ContainsKey(opcion.Prefab))
                    pools[opcion.Prefab] = CrearPool(opcion.Prefab);
            }
        }
    }


    private bool ValidarOleadas()
    {
        if (oleadas == null || oleadas.Length == 0)
        {
            Debug.LogError("Spawner: no hay oleadas.");
            return false;
        }

        totalPlaneado = 0;

        for (int i = 0; i < oleadas.Length; i++)
        {
            // Con valor 0 devuelve la primera opción válida; null solo si no hay ninguna.
            if (oleadas[i] == null || oleadas[i].Cantidad <= 0 || oleadas[i].ElegirPrefab(0f) == null)
            {
                Debug.LogError("Spawner: la oleada " + i + " no tiene cantidad o enemigos válidos.");
                return false;
            }

            totalPlaneado += oleadas[i].Cantidad;
        }

        return true;
    }


    private IObjectPool<EnemigoAlien> CrearPool(EnemigoAlien prefab)
    {
        ObjectPool<EnemigoAlien> pool = null;

        pool =
            new ObjectPool<EnemigoAlien>(
                () => CreateEnemy(prefab, pool),
                OnGetEnemy,
                OnReleaseEnemy,
                OnDestroyEnemy,

                collectionCheck: true,

                defaultCapacity: capacidadMaxima,

                maxSize: capacidadMaxima
            );

        return pool;
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

    private EnemigoAlien CreateEnemy(
        EnemigoAlien prefab,
        IObjectPool<EnemigoAlien> pool)
    {
        EnemigoAlien enemy =
            Instantiate(prefab);


        enemy.SetPool(pool);


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

        enemigosEliminados++;

        if (enemigosEliminados >= totalPlaneado)
            MostrarVictoria();
    }


    private void MostrarVictoria()
    {
        // Si el jugador ya perdió, no se gana en el mismo momento.
        if (canvasDerrota != null && canvasDerrota.activeSelf)
            return;

        if (canvasVictoria != null)
            canvasVictoria.SetActive(true);
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

        // Nunca salen más enemigos que la suma de todas las oleadas.
        if (totalSpawneados >= totalPlaneado)
            return false;

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

        EnemigoAlien prefabElegido =
            oleadas[oleadaActual].ElegirPrefab(Random.value);

        if (prefabElegido == null)
        {
            Debug.LogError("Spawner: la oleada no tiene enemigos válidos.");
            return false;
        }

        posicionesOcupadas[positionIndex] =
            true;


        EnemigoAlien enemy =
            pools[prefabElegido].Get();


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

        ContarSpawn();

        _ = enemy.MoveToPositionAsync(
            posicionFinal.position,
            velocidadMovimiento
        );


        return true;
    }


    // Cuenta el enemigo recién salido y pasa a la siguiente oleada cuando se completa la actual.
    private void ContarSpawn()
    {
        totalSpawneados++;
        spawneadosEnOleada++;

        if (
            spawneadosEnOleada >= oleadas[oleadaActual].Cantidad &&
            oleadaActual < oleadas.Length - 1
        )
        {
            oleadaActual++;
            spawneadosEnOleada = 0;
        }
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