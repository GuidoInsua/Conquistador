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
// Desc: Saca los aliens de cada oleada desde un pool y los manda a una posición libre; al terminar las oleadas arranca la pelea con el jefe o la victoria.

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
    [Tooltip("Si está asignada, al terminar las oleadas empieza el boss y la victoria llega al vencerlo.")]
    [SerializeField] private FaseBoss faseBoss;

    [Header("Escena")]
    [Tooltip("Sus hijos son los puntos fuera de pantalla por donde entran los aliens.")]
    [SerializeField] private Transform contenedorSpawns;
    [Tooltip("Sus hijos son las posiciones donde se quedan los aliens. Hay un alien como máximo por posición.")]
    [SerializeField] private Transform contenedorPosiciones;

    [Header("Configuración")]
    [Tooltip("Cada alien entra por uno de estos N puntos de spawn más cercanos a su posición, elegido al azar.")]
    [SerializeField, Min(1)] private int puntosSpawnCercanos = 2;
    [SerializeField] private float velocidadMovimiento = 3f;
    [SerializeField] private float tiempoEntreSpawns = 2f;
    [Tooltip("En pantallas más anchas que 16:9 los puntos de entrada quedarían a la vista: se corren afuera. Vacía, no los corre.")]
    [SerializeField] private Camera camara;

    private Transform[] puntosSpawn;
    private Transform[] posiciones;
    private EnemigoAlien[] ocupantes; // ocupantes[i] es el alien que está en posiciones[i], o null si está libre
    private Dictionary<EnemigoAlien, IObjectPool<EnemigoAlien>> pools;
    private int oleadaActual;
    private int spawneadosEnOleada;
    private int totalSpawneados;
    private int enemigosEliminados;
    private float tiempoProximoSpawn;

    public int TotalSpawneados
    {
        get { return totalSpawneados; }
    }

    public int EnemigosEliminados
    {
        get { return enemigosEliminados; }
    }

    private void Awake()
    {
        if (!ValidarOleadas())
        {
            enabled = false;
            return;
        }

        puntosSpawn = Hijos(contenedorSpawns);
        posiciones = Hijos(contenedorPosiciones);

        if (puntosSpawn.Length == 0 || posiciones.Length == 0)
        {
            Debug.LogError("Spawner: contenedorSpawns y contenedorPosiciones tienen que tener al menos un hijo cada uno.");
            enabled = false;
            return;
        }

        ocupantes = new EnemigoAlien[posiciones.Length];

        // Un pool por tipo de enemigo: cada alien vuelve al pool de su propio prefab.
        pools = new Dictionary<EnemigoAlien, IObjectPool<EnemigoAlien>>();
        foreach (Oleada oleada in oleadas)
        {
            foreach (OpcionEnemigo opcion in oleada.Enemigos)
            {
                if (opcion.Prefab != null && !pools.ContainsKey(opcion.Prefab))
                {
                    pools[opcion.Prefab] = CrearPool(opcion.Prefab);
                }
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

        for (int i = 0; i < oleadas.Length; i++)
        {
            // Con valor 0 devuelve la primera opción válida; null solo si no hay ninguna.
            if (oleadas[i] == null || oleadas[i].Cantidad <= 0 || oleadas[i].ElegirPrefab(0f) == null)
            {
                Debug.LogError("Spawner: la oleada " + i + " no tiene cantidad o enemigos válidos.");
                return false;
            }
        }

        return true;
    }

    private static Transform[] Hijos(Transform contenedor)
    {
        if (contenedor == null)
        {
            return new Transform[0];
        }

        Transform[] hijos = new Transform[contenedor.childCount];
        for (int i = 0; i < hijos.Length; i++)
        {
            hijos[i] = contenedor.GetChild(i);
        }

        return hijos;
    }

    private IObjectPool<EnemigoAlien> CrearPool(EnemigoAlien prefab)
    {
        ObjectPool<EnemigoAlien> pool = null;

        // El pool necesita saber cómo crear un alien nuevo cuando no le quedan guardados.
        // "() => CrearEnemigo(prefab, pool)" es una función sin nombre que hace eso: cada pool la
        // guarda con su propio prefab, por eso no puede ser un método común de la clase.
        // Las otras tres acciones (sacar, devolver y destruir) son métodos normales de abajo.
        pool = new ObjectPool<EnemigoAlien>(
            () => CrearEnemigo(prefab, pool),
            AlSacarDelPool,
            OnReleaseEnemy,
            AlDestruirDelPool,
            collectionCheck: true,
            defaultCapacity: posiciones.Length,
            maxSize: posiciones.Length);

        return pool;
    }

    private EnemigoAlien CrearEnemigo(EnemigoAlien prefab, IObjectPool<EnemigoAlien> pool)
    {
        EnemigoAlien enemy = Instantiate(prefab);
        enemy.SetPool(pool);
        enemy.gameObject.SetActive(false);
        return enemy;
    }

    // El pool la llama cada vez que se saca un alien para usarlo.
    private void AlSacarDelPool(EnemigoAlien enemy)
    {
        enemy.gameObject.SetActive(true);
        enemy.ResetEnemy();
    }

    // El pool la llama cuando ya tiene demasiados guardados y le sobra uno.
    private void AlDestruirDelPool(EnemigoAlien enemy)
    {
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void Start()
    {
        tiempoProximoSpawn = Time.time + tiempoEntreSpawns;
    }

    private void Update()
    {
        if (Time.time < tiempoProximoSpawn)
        {
            return;
        }

        tiempoProximoSpawn = Time.time + tiempoEntreSpawns;
        SpawnEnemy();
    }

    public bool SpawnEnemy()
    {
        // La oleada ya salió completa: la siguiente espera a que mueran todos sus enemigos.
        if (spawneadosEnOleada >= oleadas[oleadaActual].Cantidad)
        {
            return false;
        }

        int indice = ElegirPosicionLibre();
        if (indice < 0)
        {
            return false;
        }

        // ValidarOleadas ya comprobó que cada oleada tiene al menos una opción válida.
        EnemigoAlien prefabElegido = oleadas[oleadaActual].ElegirPrefab(Random.value);
        EnemigoAlien enemy = pools[prefabElegido].Get();
        Vector3 destino = posiciones[indice].position;

        enemy.transform.position = ElegirPuntoSpawn(destino);
        ocupantes[indice] = enemy;
        totalSpawneados++;
        spawneadosEnOleada++;

        // "_ =" indica que no esperamos a que termine: el alien viaja solo mientras el juego sigue.
        _ = enemy.MoveToPositionAsync(destino, velocidadMovimiento);
        return true;
    }

    private int ElegirPosicionLibre()
    {
        List<int> libres = new List<int>();
        for (int i = 0; i < ocupantes.Length; i++)
        {
            if (ocupantes[i] == null)
            {
                libres.Add(i);
            }
        }

        if (libres.Count == 0)
        {
            return -1;
        }

        return libres[Random.Range(0, libres.Count)];
    }

    // Entra por uno de los puntos más cercanos a su posición: la entrada varía, pero ninguno cruza toda la pantalla.
    private Vector3 ElegirPuntoSpawn(Vector3 destino)
    {
        // Copia de los puntos y la distancia de cada uno al destino.
        Transform[] cercanos = (Transform[])puntosSpawn.Clone();
        float[] distancias = new float[cercanos.Length];
        for (int i = 0; i < cercanos.Length; i++)
        {
            distancias[i] = Vector3.Distance(cercanos[i].position, destino);
        }

        // Ordena los puntos según su distancia, del más cercano al más lejano.
        System.Array.Sort(distancias, cercanos);

        int cantidadParaElegir = Mathf.Min(puntosSpawnCercanos, cercanos.Length);
        Vector3 punto = cercanos[Random.Range(0, cantidadParaElegir)].position;

        if (camara != null)
        {
            float mitadAnchoVista = camara.orthographicSize * camara.aspect;
            punto.x = FueraDeVista(punto.x, mitadAnchoVista);
        }

        return punto;
    }

    // Si x cae dentro de la vista (más un margen para que no asome el alien), lo lleva justo afuera, del mismo lado.
    public static float FueraDeVista(float x, float mitadAnchoVista)
    {
        const float margen = 1f;
        float borde = mitadAnchoVista + margen;

        if (Mathf.Abs(x) >= borde)
        {
            return x;
        }

        // Mathf.Sign devuelve 1 si x es positivo y -1 si es negativo: así queda del mismo lado.
        return Mathf.Sign(x) * borde;
    }

    private void OnReleaseEnemy(EnemigoAlien enemy)
    {
        int indice = System.Array.IndexOf(ocupantes, enemy);
        if (indice >= 0)
        {
            ocupantes[indice] = null;
        }

        enemy.gameObject.SetActive(false);

        enemigosEliminados++;

        // Quedan vivos o falta que salgan enemigos de esta oleada.
        if (enemigosEliminados < totalSpawneados || spawneadosEnOleada < oleadas[oleadaActual].Cantidad)
        {
            return;
        }

        if (oleadaActual < oleadas.Length - 1)
        {
            oleadaActual++;
            spawneadosEnOleada = 0;
            return;
        }

        if (faseBoss != null)
        {
            faseBoss.Iniciar(MostrarVictoria);
        }
        else
        {
            MostrarVictoria();
        }
    }

    private void MostrarVictoria()
    {
        // Si el jugador ya perdió, no se gana en el mismo momento.
        if (canvasDerrota != null && canvasDerrota.activeSelf)
        {
            return;
        }

        // Las balas que quedaron en el aire no pueden matar al jugador después de ganar.
        BalaEnemiga[] balas = FindObjectsByType<BalaEnemiga>(FindObjectsSortMode.None);
        foreach (BalaEnemiga bala in balas)
        {
            Destroy(bala.gameObject);
        }

        if (canvasVictoria != null)
        {
            canvasVictoria.SetActive(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        DibujarHijos(contenedorSpawns, Color.red);
        DibujarHijos(contenedorPosiciones, Color.green);
    }

    private static void DibujarHijos(Transform contenedor, Color color)
    {
        if (contenedor == null)
        {
            return;
        }

        Gizmos.color = color;
        foreach (Transform hijo in contenedor)
        {
            Gizmos.DrawWireSphere(hijo.position, 0.3f);
        }
    }
}
